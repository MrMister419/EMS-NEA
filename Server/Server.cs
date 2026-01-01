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
    
    private static async Task<string> ProcessRequest(string receivedJson)
    {
        string requestType = "";
        string payload = "";
        string statusMessage = "";
        
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
            case "CheckLogin":
                statusMessage = await CheckLogin(payload);
                break;
            default:
                Console.WriteLine("Unknown request type.");
                break;
        }
        
        return statusMessage;
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
    
    private static Task<string> CheckLogin(string loginJson)
    {
        Console.WriteLine(loginJson);
        Dictionary<string, string> loginDetails = JsonSerializer.Deserialize<Dictionary<string, string>>(loginJson);
        string email = loginDetails["Email"];
        string password = loginDetails["Password"];
        string outcome = database.CheckLoginDetails(email, password);
        return Task.FromResult(outcome);
    }
}

class Listener
{
    private readonly HttpListener listener = new HttpListener();
    public Func<string, Task<string>> RequestReceived;
    
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
            
            string responseMessage = await RequestReceived.Invoke(receivedJson);

            // Respond to POST request
            HttpListenerResponse response = context.Response;

            if (responseMessage == "")
            {
                response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
                response.ContentType = "text/plain";
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseMessage);
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