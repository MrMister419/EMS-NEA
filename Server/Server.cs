using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.Text;

namespace EMS_NEA;

class Server
{
    private static DatabaseManager database;
    private static HttpClient httpClient;
    private static Listener listener;


    public static async Task Main(string[] args)
    {
        InitialSetup();
        await listener.Listen();
    }

    private static void InitialSetup()
    {
        httpClient = new HttpClient();
        database = new DatabaseManager();
        listener = new Listener();
        listener.RequestReceived = ProcessRequest;
    }
    
    private static async Task<Dictionary<string, string>> ProcessRequest(string receivedJson)
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
        
        switch (requestType)
        {
            case "NewUser":
                CreateNewUser(payload);
                break;
            case "NewEvent":
                HandleNewEvent(payload);
                break;
            case "UpdateUser":
                UpdateUser(payload);
                break;
            case "UpdateEvent":
                UpdateEvent(payload);
                break;
            case "Authenticate":
                completionStatus = await Authenticate(payload);
                break;
            case "ToggleAlertChoice":
                completionStatus = ToggleAlertChoice(payload);
                break;
            case "GetAccountDetails":
                completionStatus = await GetAccountDetails(payload);
                break;
            case "ModifyAccountDetails":
                completionStatus = await ModifyAccountDetails(payload);
                break;
            case "GetReceivingStatus":
                completionStatus = GetReceivingStatus(payload);
                break;
            default:
                completionStatus["outcome"] = "Unknown request type.";
                completionStatus["successful"] = "false";
                break;
        }
        
        return completionStatus;
    }

    private static void CreateNewUser(string userJson)
    {
        Console.WriteLine(userJson);
        User receivedUser = User.DeserializeUser(userJson);
        Console.WriteLine("Values:");
        Console.WriteLine(receivedUser.email);
        Console.WriteLine(receivedUser);
        Console.WriteLine(receivedUser.GetType());
        database.InsertUser(receivedUser);
    }

    private static async Task HandleNewEvent(string eventJson)
    {
        Event receivedEvent = Event.DeserializeEvent(eventJson);
        database.InsertEvent(receivedEvent);
        
        // TODO: only check users that are online
        List<string> inVicinity = await database.FindUsersInVicinity(receivedEvent.location);
        listener.NotifyWaitingClients(inVicinity, eventJson);
        
        
    }

    private static void UpdateUser(string userUpdateJson)
    {
        
    }
    
    private static void UpdateEvent(string eventUpdateJson)
    {
        
    }
    
    // TODO: remove Task as not async? 
    private static Task<Dictionary<string, string>> Authenticate(Dictionary<string, string> loginDetails)
    {
        // TODO: deserialise function
        Dictionary<string, string> statusResult = new Dictionary<string, string>();
        
        string email = loginDetails["Email"];
        string password = loginDetails["Password"];
        string outcome = database.CheckLoginDetails(email, password);
        
        statusResult.Add("outcome", outcome);
        if (outcome == "Correct logins.")
        {
            statusResult.Add("successful", "true");
        }
        else
        {
            statusResult.Add("successful", "false");
        }
        
        return Task.FromResult(statusResult);
    }

    private static Task<Dictionary<string, string>> Authenticate(string loginJson)
    {
        Dictionary<string, string> loginDetails = DeserializeToDictionary(loginJson);

        return Authenticate(loginDetails);
    }
    
    private static Dictionary<string, string> ToggleAlertChoice(string alertChoiceJson)
    {
        Dictionary<string, string> data = DeserializeToDictionary(alertChoiceJson);
        string email = data["Email"];
        bool alertChoice = (data["AlertChoice"] == "true");
        
        database.ChangeAlertChoice(alertChoice, email);
        
        Dictionary<string, string> response = new Dictionary<string, string>();
        response.Add("successful", "true");
        return response;
    }

    private static Task<Dictionary<string, string>> GetAccountDetails(string accountDetailsJson)
    {
        Dictionary<string, string> data = DeserializeToDictionary(accountDetailsJson);
        string email = data["Email"];
        
        Dictionary<string, string> userDetails = database.RetrieveUserDetails(email);
        
        return Task.FromResult(userDetails);
    }

    private static async Task<Dictionary<string, string>> ModifyAccountDetails(string accountDetailsJson)
    {
        Dictionary<string, string> accountDetails = DeserializeToDictionary(accountDetailsJson);
        
        accountDetails.Add("Email", accountDetails["OldEmail"]);
        accountDetails.Remove("OldEmail");
        
        Dictionary<string, string> outcome = await Authenticate(accountDetails);

        if (outcome["successful"] == "true")
        {
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
            
            bool updateCompletion = database.UpdateUserDetails(accountDetails, oldEmail);
            
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

    public static async Task<bool> CheckDistances(double lat1, double long1, double lat2, double long2)
    {
        string apiKey = "AIzaSyBR0vC11aRcwkXbXPfGr2utiILnF9EpLRY";
        string origin = $"{lat1},{long1}";
        string destination = $"{lat2},{long2}";
        string url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={origin}&destinations={destination}&key={apiKey}";
        try
        {
            string json = await httpClient.GetStringAsync(url).ConfigureAwait(false);
            Console.WriteLine("DistanceMatrix response: " + json);
            
            JsonDocument response = JsonDocument.Parse(json);

            int seconds = response.RootElement
                    .GetProperty("rows")[0]
                    .GetProperty("elements")[0]
                    .GetProperty("duration")
                    .GetProperty("value")
                    .GetInt32();

            return (seconds / 60 < 7);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DistanceMatrix call failed: {ex}");
            return false;
        }
    }
    
    private static Dictionary<string, string> GetReceivingStatus(string requestJson)
    {
        Dictionary<string, string> data = DeserializeToDictionary(requestJson);
        string email = data["Email"];
        Dictionary<string, string> response = new Dictionary<string, string>();

        bool receiving = database.GetReceivingStatus(email);
        response.Add("isReceiving", receiving.ToString());
        response.Add("successful", "true");

        return response;
    }

    private static Dictionary<string, string> DeserializeToDictionary(string json)
    {
        return JsonSerializer.Deserialize<Dictionary<string, string>>(json);
    }
    
}

class Listener
{
    private readonly HttpListener listener = new HttpListener();
    public Func<string, Task<Dictionary<string, string>>> RequestReceived;
    private readonly List<PendingRequest> waitingClients = new List<PendingRequest>();
    
    public async Task Listen()
    {
        string receivedJson = "";
        listener.Prefixes.Add("http://localhost:50000/");
        listener.Start();
        
        while (true)
        {
            // Wait for a POST request
            HttpListenerContext context = await listener.GetContextAsync();
            // Accept the request
            HttpListenerRequest request = context.Request;

            // Extract data from POST request
            using (StreamReader reader = new StreamReader(request.InputStream))
            {
                receivedJson = await reader.ReadToEndAsync();
                Console.WriteLine(receivedJson);
            }
            
            string requestType = "";
            string payload = "";
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(receivedJson))
                {
                    requestType = doc.RootElement.GetProperty("type").GetString();
                    payload = doc.RootElement.GetProperty("payload").GetRawText();
                }
            }
            catch (JsonException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.Close();
                continue;
            }
            if (requestType == "RequestEvent")
            {
                string email = ExtractEmail(payload);
                if (string.IsNullOrWhiteSpace(email))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.Close();
                    continue;
                }
                lock (waitingClients)
                {
                    waitingClients.Add(new PendingRequest { Context = context, Email = email });
                }
                continue;
            }
            Dictionary<string, string> responseMessage = await RequestReceived.Invoke(receivedJson);
            HttpListenerResponse response = context.Response;
            if (responseMessage.Count == 0)
            {
                response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
                response.ContentType = "application/json";
                string responseMessageString = JsonSerializer.Serialize(responseMessage);
                byte[] buffer = Encoding.UTF8.GetBytes(responseMessageString);
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            }
            response.Close();
            Console.WriteLine("POST request answered.");
        }
    }
    
    private string ExtractEmail(string payloadJson)
    {
        try
        {
            using (JsonDocument doc = JsonDocument.Parse(payloadJson))
            {
                if (doc.RootElement.TryGetProperty("Email", out JsonElement emailElement))
                {
                    return emailElement.GetString();
                }
            }
        }
        catch (JsonException) {}
        
        return string.Empty;
    }
    
    public void NotifyWaitingClients(List<string> targetEmails, string eventPayloadJson)
    {
        if (targetEmails.Count == 0)
        {
            return;
        }
        List<PendingRequest> toRespond = new List<PendingRequest>();
        lock (waitingClients)
        {
            List<PendingRequest> remaining = new List<PendingRequest>();
            foreach (PendingRequest pending in waitingClients)
            {
                bool match = false;
                foreach (string target in targetEmails)
                {
                    if (pending.Email == target)
                    {
                        match = true;
                        break;
                    }
                }
                if (match)
                {
                    toRespond.Add(pending);
                }
                else
                {
                    remaining.Add(pending);
                }
            }
            waitingClients.Clear();
            waitingClients.AddRange(remaining);
        }
        foreach (PendingRequest pending in toRespond)
        {
            try
            {
                HttpListenerResponse response = pending.Context.Response;
                response.ContentType = "application/json";
                byte[] buffer = Encoding.UTF8.GetBytes(eventPayloadJson);
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to notify waiting client: {ex}");
            }
        }
    }
    
    private class PendingRequest
    {
        public HttpListenerContext Context { get; set; }
        public string Email { get; set; }
    }
}

abstract class DatabaseObject
{
    public int id { get; set; }
    protected static JsonSerializerOptions jsonConfig = 
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    public string Serialize()
    {
        return JsonSerializer.Serialize(this);
    }
}

class Event : DatabaseObject
{
    public string type { get; set; }
    public string description { get; set; }
    public Location location { get; set; }
    public string startTimestamp { get; set; }
    public string resolvedTimestamp { get; set; }
    public string status { get; set; }
    
    // Factory method to create an Event instance by deserializing JSON
    public static Event DeserializeEvent(string json)
    {
        return JsonSerializer.Deserialize<Event>(json, jsonConfig);
    }
}

class Location
{
    public string address { get; set; }
    public double latitude { get; set; }
    public double longitude { get; set; }
}

class User : DatabaseObject
{
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string email { get; set; }
    public string phoneNumber { get; set; }
    public string password { get; set; }
    public Location location { get; set; }
    public bool isReceiving { get; set; }
    
    // Factory method to create a User instance by deserializing JSON
    public static User DeserializeUser(string json)
    {
        return JsonSerializer.Deserialize<User>(json, jsonConfig);
    }
}