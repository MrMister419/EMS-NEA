using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client;

/// <summary>
/// Handles all client-server communication and business logic
/// Manages authentication, account operations, and event notifications
/// </summary>
public class AppService
{
    // TODO: Use token for authentication
    private static HttpService httpService;
    private Action<string> eventReceivedHandler;

    public AppService()
    {
        httpService = new HttpService();
    }
    
    private async Task<Dictionary<string, string>?> POSTandDeserialize(string wrappedJson)
    {
        string? responseString = await httpService.SendPOSTrequest(wrappedJson);
        Dictionary<string, string>? response = AppContext.DeserializeToDictionary(responseString);
        
        return response;
    }
    
    // Sends user signup request to server
    // Parameters:
    // - Dictionary<string, string> formValues: user registration data
    public async Task<Dictionary<string, string>?> SignUp(Dictionary<string, string> formValues)
    {
        formValues["Password"] = AppContext.Hash(formValues["Password"]);
        string wrappedJson = PackageJson(formValues, "NewUser");
        
        return await POSTandDeserialize(wrappedJson);
    }

    // Authenticates user credentials with server
    // Parameters:
    // - Dictionary<string, string> formValues: email and password
    // Returns:
    // Task<Dictionary<string, string>>: authentication result with outcome and success status
    public async Task<Dictionary<string, string>?> Authenticate(Dictionary<string, string> formValues)
    {
        formValues["Password"] = AppContext.Hash(formValues["Password"]);
        string wrappedJson = PackageJson(formValues, "Authenticate");
        
        Dictionary<string, string>? response = await POSTandDeserialize(wrappedJson);
        return response;
    }
    
    // Retrieves user alert-receiving preference from server
    // Returns:
    // Task for the asyncronous operation with no return value
    public async Task GetReceivingStatus()
    {
        // Creates POST request string
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload.Add("Email", AppContext.email);
        string wrappedJson = PackageJson(payload, "GetReceivingStatus");
        
        Dictionary<string, string>? response = await POSTandDeserialize(wrappedJson);
        
        if (response != null && response.ContainsKey("isReceiving"))
        {
            bool.TryParse(response["isReceiving"], out bool choice);
            AppContext.isReceiving = choice;
        }
    }

    // Initiates long-polling request to wait for event notifications
    public void RequestEvent()
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload.Add("Email", AppContext.email);
        string wrappedJson = PackageJson(payload, "RequestEvent");
        
        Task.Run(() => ListenForEventAsyncWrapper(wrappedJson));
        Console.WriteLine("\nPlaced event request.");
    }

    private async Task ListenForEventAsyncWrapper(string wrappedJson)
    {
        try
        {
            await ListenForEvent(wrappedJson);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    // Listens for event response and triggers handler when received
    // Parameters:
    // - string wrappedJson: packaged request JSON
    // Returns:
    // Task for the asyncronous operation with no return value
    private async Task ListenForEvent(string wrappedJson)
    {
        // Waits here until long-poll is answered
        string? eventData = await httpService.SendPOSTrequest(wrappedJson);

        if (string.IsNullOrWhiteSpace(eventData))
        {
            Console.WriteLine("No event data received.");
        }
        else
        {
            eventReceivedHandler.Invoke(eventData);
        }
        
        if (AppContext.isReceiving)
        {
            RequestEvent();
        }
    }

    // Updates user alert preference on server
    // Parameters:
    // - bool newChoice: new alert receiving preference
    public async Task<Dictionary<string, string>?> ToggleAlertChoice(bool newChoice)
    {
        // Create POST request string
        Dictionary<string, string> payload = new Dictionary<string, string>();
        string alertChoice = newChoice.ToString();
        payload.Add("Email", AppContext.email);
        payload.Add("AlertChoice", alertChoice);
        
        string wrappedJson = PackageJson(payload, "ToggleAlertChoice");
        
        return await POSTandDeserialize(wrappedJson);
    }

    // Retrieves user account details from server
    // Returns:
    // Task<Dictionary<string, string>>: user account information
    public async Task<Dictionary<string, string>?> GetAccountDetails()
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload.Add("Email", AppContext.email);
        
        string wrappedJson = PackageJson(payload, "GetAccountDetails");
        
        Dictionary<string, string>? response = await POSTandDeserialize(wrappedJson);
        return response;
    }

    // Sends updated account details to server after password verification
    // Parameters:
    // - Dictionary<string, string> formValues: updated account fields
    // Returns:
    // Task<Dictionary<string, string>>: update result with outcome and success status
    public async Task<Dictionary<string, string>?> ModifyAccountDetails(Dictionary<string, string> formValues)
    {
        formValues = RemoveNullEntries(formValues);

        string passwordHash = AppContext.Hash(formValues["ConfirmPassword"]);
        formValues.Remove("ConfirmPassword");
        formValues.Add("Password", passwordHash);
        formValues.Add("OldEmail", AppContext.email);
        
        string wrappedJson = PackageJson(formValues, "ModifyAccountDetails");
        
        Dictionary<string, string>? response = await POSTandDeserialize(wrappedJson);
        return response;
    }
    
    // Removes empty entries from dictionary
    // Parameters:
    // - Dictionary<string, string> dictionary: dictionary to clean
    // Returns:
    // Dictionary<string, string>: dictionary with empty entries removed
    private Dictionary<string, string> RemoveNullEntries(Dictionary<string, string> dictionary)
    {
        List<string> keysToRemove = new List<string>();

        foreach (KeyValuePair<string, string> entry in dictionary)
        {
            if (entry.Value == "")
            {
                keysToRemove.Add(entry.Key);
            }
        }

        for (int index = 0; index < keysToRemove.Count; index++)
        {
            dictionary.Remove(keysToRemove[index]);
        }

        return dictionary;
    }

    public async Task<Dictionary<string, string>?> ChangePassword(Dictionary<string, string> formValues)
    {
        formValues = RemoveNullEntries(formValues);

        string oldPasswordHash = AppContext.Hash(formValues["OldPassword"]);
        string newPasswordHash = AppContext.Hash(formValues["NewPassword"]);
        formValues["OldPassword"] = oldPasswordHash;
        formValues["NewPassword"] = newPasswordHash;
        formValues.Add("Email", AppContext.email);
        
        string wrappedJson = PackageJson(formValues, "ChangePassword");
        
        return await POSTandDeserialize(wrappedJson);
    }

    public async Task<Dictionary<string, string>?> DeleteUser(Dictionary<string, string> formValues)
    {
        string passwordHash = AppContext.Hash(formValues["Password"]);
        formValues["Password"] = passwordHash;
        formValues.Add("Email", AppContext.email);
        
        string wrappedJson = PackageJson(formValues, "DeleteUser");
        
        return await POSTandDeserialize(wrappedJson);
    }


    public async Task<Dictionary<string, string>?> GetUserLocation()
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload.Add("Email", AppContext.email);
        
        string wrappedJson = PackageJson(payload, "GetUserLocation");
        
        Dictionary<string, string>? response = await POSTandDeserialize(wrappedJson);
        return response;
    }
    
    // Registers handler for incoming event notifications
    // Parameters:
    // - Action<Dictionary<string, string>> handler: callback to invoke when event received
    public void RegisterEventHandler(Action<string> handler)
    {
        eventReceivedHandler = handler;
    }

    // Wraps payload in JSON envelope with request type
    // Parameters:
    // - Dictionary<string, string> payload: data to send
    // - string type: request type identifier
    // Returns:
    // string: packaged JSON string
    private string PackageJson(Dictionary<string, string> payload, string type)
    {
        Dictionary<string, object> wrappedValues = new Dictionary<string, object>();

        wrappedValues.Add("type", type);
        wrappedValues.Add("payload", payload);
        string packagedJson = JsonSerializer.Serialize(wrappedValues);
        
        return packagedJson;
    }
}

// class User
// {
//     public string Username { get; set; }
//     public string Email { get; set; }
//     public string Password { get; set; }
//
//     public User(string username, string email, string hashedPassword)
//     {
//         Username = username;
//         Email = email;
//         Password = hashedPassword;
//     }
// }
