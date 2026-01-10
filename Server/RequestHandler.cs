using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server;

/// <summary>
/// Processes incoming client requests and coordinates responses
/// Routes requests to appropriate database operations
/// </summary>
public class RequestHandler
{
    
    // Asyncronously processes incoming requests from clients by calling delegate methods for each request type
    // Parameters:
    // - string receivedJson: JSON string received from client
    // Returns:
    // Task<Dictionary<string, string>> completionStatus: outcome of request processing
    public async Task<string> ProcessRequest(string receivedJson)
    {
        string requestType = "";
        string payload = "";
        Dictionary<string, string> completionStatus = new Dictionary<string, string>();
        
        // Get request type from JSON
        using (JsonDocument doc = JsonDocument.Parse(receivedJson))
        {
            requestType = doc.RootElement.GetProperty("type").GetString();
            payload = doc.RootElement.GetProperty("payload").GetRawText();
        }
        
        // call corresponding method based on request type
        switch (requestType)
        {
            // TODO: add completion status to all methods
            case "NewUser":
                await CreateNewUser(payload);
                break;
            case "NewEvent":
                await HandleNewEvent(payload);
                break;
            case "UpdateEvent":
                await UpdateEvent(payload);
                break;
            case "Authenticate":
                completionStatus = await Authenticate(payload);
                break;
            case "ToggleAlertChoice":
                completionStatus = await ToggleAlertChoice(payload);
                break;
            case "GetAccountDetails":
                completionStatus = await GetAccountDetails(payload);
                break;
            case "ModifyAccountDetails":
                completionStatus = await ModifyAccountDetails(payload);
                break;
            case "GetReceivingStatus":
                completionStatus = await GetReceivingStatus(payload);
                break;
            default:
                completionStatus["outcome"] = "Unknown request type.";
                completionStatus["successful"] = "false";
                break;
        }
        
        string responseJson = JsonSerializer.Serialize(completionStatus);
        if (responseJson == "{}") responseJson = "OK";
        
        return responseJson;
    }

    // Calls delegates to deserialize JSON string into User object and insert into database
    // Parameters:
    // - string userJson: JSON string containing User object data
    // Returns:
    // Task: Task for the asyncronous operation with no return value
    private async Task CreateNewUser(string userJson)
    {
        User receivedUser = User.DeserializeUser(userJson);
        await ServerContext.database.InsertUser(receivedUser);
    }

    // Deserializes JSON string into Event object and insert into database
    // Parameters:
    // - string eventJson: JSON string containing Event object data
    // Returns:
    // Task: Task for the asyncronous operation with no return value
    private async Task HandleNewEvent(string eventJson)
    {
        Event receivedEvent = Event.DeserializeEvent(eventJson);
        
        // Insert new Event into the database
        // TODO: only check users that are online for efficiency
        await ServerContext.database.InsertEvent(receivedEvent);
        
        // Find and notify users in vicinity to this new event
        List<string> inVicinity = await ServerContext.database.FindUsersInVicinity(receivedEvent.location);
        Console.WriteLine($"Found {inVicinity.Count} users in vicinity of incident.");
        await ServerContext.listener.NotifyWaitingClients(inVicinity, eventJson);
    }
    
    // Updates event details in database
    // Parameters:
    // - string eventUpdateJson: JSON string containing updated Event data
    // Returns:
    // Task: Task for the asyncronous operation with no return value
    private async Task UpdateEvent(string eventUpdateJson)
    {
        throw new NotImplementedException();
    }
    
    // Checks login credentials against database
    // Parameters:
    // - Dictionary<string, string> loginDetails: dictionary containing Email and Password
    // Returns:
    // Task<Dictionary<string, string>>: outcome and successful status
    private async Task<Dictionary<string, string>> Authenticate(Dictionary<string, string> loginDetails)
    {
        Dictionary<string, string> statusResult = new Dictionary<string, string>();
        
        string email = loginDetails["Email"];
        string password = loginDetails["Password"];
        string outcome = await ServerContext.database.CheckLoginDetails(email, password);
        
        statusResult.Add("outcome", outcome);
        if (outcome == "Correct logins.")
        {
            statusResult.Add("successful", "true");
        }
        else
        {
            statusResult.Add("successful", "false");
        }
        
        return statusResult;
    }

    // Overload that deserializes JSON string and calls main Authenticate method
    // Parameters:
    // - string loginJson: JSON string containing login credentials
    // Returns:
    // Task<Dictionary<string, string>>: outcome and successful status
    private async Task<Dictionary<string, string>> Authenticate(string loginJson)
    {
        Dictionary<string, string> loginDetails = DeserializeToDictionary(loginJson);

        return await Authenticate(loginDetails);
    }
    
    // Updates user alert choice preference in database
    // Parameters:
    // - string alertChoiceJson: JSON string containing Email and AlertChoice
    // Returns:
    // Task<Dictionary<string, string>>: successful status
    private async Task<Dictionary<string, string>> ToggleAlertChoice(string alertChoiceJson)
    {
        Dictionary<string, string> data = DeserializeToDictionary(alertChoiceJson);
        string email = data["Email"];
        bool alertChoice = (data["AlertChoice"] == "true");
        
        await ServerContext.database.ChangeAlertChoice(alertChoice, email);
        
        Dictionary<string, string> response = new Dictionary<string, string>();
        response.Add("successful", "true");
        return response;
    }

    // Retrieves user account details from database
    // Parameters:
    // - string accountDetailsJson: JSON string containing Email
    // Returns:
    // Task<Dictionary<string, string>>: user details
    private async Task<Dictionary<string, string>> GetAccountDetails(string accountDetailsJson)
    {
        Dictionary<string, string> data = DeserializeToDictionary(accountDetailsJson);
        string email = data["Email"];
        
        Dictionary<string, string> userDetails = await ServerContext.database.RetrieveUserDetails(email);
        
        return userDetails;
    }

    // Modifies user account details after authenticating
    // Parameters:
    // - string accountDetailsJson: JSON string containing new account details and login details
    // Returns:
    // Task<Dictionary<string, string>>: outcome and successful status
    private async Task<Dictionary<string, string>> ModifyAccountDetails(string accountDetailsJson)
    {
        Dictionary<string, string> accountDetails = DeserializeToDictionary(accountDetailsJson);
        
        accountDetails.Add("Email", accountDetails["OldEmail"]);
        accountDetails.Remove("OldEmail");
        
        Dictionary<string, string> outcome = await Authenticate(accountDetails);

        if (outcome["successful"] == "true")
        {
            // Renames NewEmail field to Email if present
            string oldEmail = accountDetails["Email"];
            if (accountDetails.ContainsKey("NewEmail"))
            {
                accountDetails["Email"] = accountDetails["NewEmail"];
                accountDetails.Remove("NewEmail");
            }
            else
            {
                accountDetails.Remove("Email");
            }
            accountDetails.Remove("Password");
            
            bool updateCompletion = await ServerContext.database.UpdateUserDetails(accountDetails, oldEmail);
            
            if (updateCompletion)
            {
                outcome["outcome"] = "Update successful.";
            }
            else
            {
                outcome["outcome"] = "Server error.";
                outcome["successful"] = "false";
            }
        }
        else
        {
            outcome["outcome"] = "Incorrect password.";
        }
        
        return outcome;
    }

    // Checks if two locations are within 7 minutes drive using Google Distance Matrix API
    // Parameters:
    // - double lat1: latitude of first location
    // - double long1: longitude of first location
    // - double lat2: latitude of second location
    // - double long2: longitude of second location
    // Returns:
    // Task<bool>: true if within 7 minutes, false otherwise
    public async Task<bool> CheckDistances(double lat1, double long1, double lat2, double long2)
    {
        const string apiKey = "AIzaSyBR0vC11aRcwkXbXPfGr2utiILnF9EpLRY";
        string origin = $"{lat1},{long1}";
        string destination = $"{lat2},{long2}";
        string url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={origin}&destinations={destination}&key={apiKey}";
        try
        {
            string json = await ServerContext.httpClient.GetStringAsync(url).ConfigureAwait(false);
            Console.WriteLine("DistanceMatrix response: " + json);
            
            JsonDocument response = JsonDocument.Parse(json);

            int seconds = response.RootElement
                    .GetProperty("rows")[0]
                    .GetProperty("elements")[0]
                    .GetProperty("duration")
                    .GetProperty("value")
                    .GetInt32();

            // is less than 7 minutes?
            return (seconds / 60 < 7);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DistanceMatrix call failed: {ex}");
            return false;
        }
    }
    
    // Retrieves user alert-receiving status from database
    // Parameters:
    // - string requestJson: JSON string containing Email
    // Returns:
    // Task<Dictionary<string, string>>: isReceiving status, outcome and successful status
    private async Task<Dictionary<string, string>> GetReceivingStatus(string requestJson)
    {
        Dictionary<string, string> data = DeserializeToDictionary(requestJson);
        string email = data["Email"];
        Dictionary<string, string> response = new Dictionary<string, string>();

        bool receiving = await ServerContext.database.GetReceivingStatus(email);
        response.Add("isReceiving", receiving.ToString());
        response.Add("outcome", "Done");
        response.Add("successful", "true");

        return response;
    }

    // Helper method to deserialize JSON string into dictionary
    // Parameters:
    // - string json: JSON string to deserialize
    // Returns:
    // Dictionary<string, string>: deserialized dictionary
    private Dictionary<string, string> DeserializeToDictionary(string json)
    {
        return JsonSerializer.Deserialize<Dictionary<string, string>>(json);
    }
    
}