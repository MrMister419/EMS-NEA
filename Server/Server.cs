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

namespace EMS_NEA;

class Server
{
    private static DatabaseManager database;

    public static async Task Main(string[] args)
    {
        database = new DatabaseManager();
        Listener listener = new Listener();
        // Subscribe ProcessMessage as event handler
        listener.RequestReceived = ProcessRequest;
        await listener.Listen();
        
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
                CreateNewEvent(payload);
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
                ToggleAlertChoice(payload);
                break;
            case "GetAccountDetails":
                completionStatus = await GetAccountDetails(payload);
                break;
            case "ModifyAccountDetails":
                completionStatus = await ModifyAccountDetails(payload);
                break;
            default:
                Console.WriteLine("Unknown request type.");
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

    private static void CreateNewEvent(string eventJson)
    {
        Event receivedEvent = Event.DeserializeEvent(eventJson);
        database.InsertEvent(receivedEvent);
    }

    private static void UpdateUser(string userUpdateJson)
    {
        
    }
    
    private static void UpdateEvent(string eventUpdateJson)
    {
        
    }
    
    // TODO: remove Task as not async? 
    private static Task<Dictionary<string, string>> Authenticate(string loginJson)
    {
        Console.WriteLine(loginJson);
        Dictionary<string, string> loginDetails = JsonSerializer.Deserialize<Dictionary<string, string>>(loginJson);
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
    
    private static void ToggleAlertChoice(string alertChoiceJson)
    {
        Dictionary<string, string> data = JsonSerializer.Deserialize<Dictionary<string, string>>(alertChoiceJson);
        string email = data["Email"];
        bool alertChoice = (data["AlertChoice"] == "True");
        
        database.ChangeAlertChoice(alertChoice, email);
    }

    private static Task<Dictionary<string, string>> GetAccountDetails(string accountDetailsJson)
    {
        Dictionary<string, string> data = JsonSerializer.Deserialize<Dictionary<string, string>>(accountDetailsJson);
        string email = data["Email"];
        
        Dictionary<string, string> userDetails = database.RetrieveUserDetails(email);
        
        return Task.FromResult(userDetails);
    }

    private static async Task<Dictionary<string, string>> ModifyAccountDetails(string accountDetailsJson)
    {
        Dictionary<string, string> outcome = await Authenticate(accountDetailsJson);

        if (outcome["successful"] == "true")
        {
            Dictionary<string, string> newAccountDetails = JsonSerializer.Deserialize<Dictionary<string, string>>(accountDetailsJson);
            newAccountDetails.Remove("Password");

            string oldEmail;
            using (JsonDocument doc = JsonDocument.Parse(accountDetailsJson))
            {
                oldEmail = doc.RootElement.GetProperty("Email").GetString();
            }

            string updateCompletion = database.UpdateUserDetails(newAccountDetails, oldEmail);
            
            if (updateCompletion == "Update successful.")
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
}

class Listener
{
    private readonly HttpListener listener = new HttpListener();
    public Func<string, Task<Dictionary<string, string>>> RequestReceived;
    
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
            
            Dictionary<string, string> responseMessage = await RequestReceived.Invoke(receivedJson);

            // Respond to POST request
            HttpListenerResponse response = context.Response;

            if (responseMessage.Count == 0)
            {
                response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
                response.ContentType = "application/json";
                string responseMessageString = JsonSerializer.Serialize(responseMessage);
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseMessageString);
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            response.Close();
            Console.WriteLine("POST request answered.");
        }
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