using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EMS_NEA;

/// <summary>
/// Holds global server components and manages server initialization
/// </summary>
static class ServerContext
{
    public static RequestHandler requestHandler;
    public static DatabaseManager database;
    public static HttpClient httpClient;
    public static Listener listener;
    public static OleDbConnection connection;

    // Initializes all server components and starts listening for requests
    // Returns:
    // Task for the asyncronous operation with no return value
    public static async Task InitializeServer()
    {
        requestHandler = new RequestHandler();
        httpClient = new HttpClient();
        database = new DatabaseManager();
        listener = new Listener();
        listener.RequestReceived = requestHandler.ProcessRequest;
        await listener.Listen();
    }
}


/// <summary>
/// Handles HTTP listening and routing for incoming client requests
/// Manages long-polling connections for event notifications
/// </summary>
class Listener
{
    private readonly HttpListener listener = new HttpListener();
    public Func<string, Task<Dictionary<string, string>>> RequestReceived;
    private readonly List<PendingRequest> waitingClients = new List<PendingRequest>();
    
    // Listens for incoming HTTP requests and routes them appropriately
    // Returns:
    // Task for the asyncronous operation with no return value
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
    
    // Extracts email from JSON payload
    // Parameters:
    // - string payloadJson: JSON string containing Email field
    // Returns:
    // string: extracted email or empty string if not found
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
    
    // Notifies waiting clients whose emails match target list
    // Parameters:
    // - List<string> targetEmails: list of emails to notify
    // - string eventPayloadJson: JSON string containing event data to send
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
    
    // Nested class to store waiting client connection details
    private class PendingRequest
    {
        public HttpListenerContext Context { get; set; }
        public string Email { get; set; }
    }
}

/// <summary>
/// Base class for database objects
/// </summary>
abstract class DatabaseObject
{
    public int id { get; set; }
    protected static JsonSerializerOptions jsonConfig = 
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    // Serializes object to JSON string
    // Returns:
    // string: JSON representation of object
    public string Serialize()
    {
        return JsonSerializer.Serialize(this);
    }
}

/// <summary>
/// Represents an emergency incident event
/// </summary>
class Event : DatabaseObject
{
    public string type { get; set; }
    public string description { get; set; }
    public Location location { get; set; }
    public string startTimestamp { get; set; }
    public string resolvedTimestamp { get; set; }
    public string status { get; set; }
    
    // Factory method to create an Event instance by deserializing JSON
    // Parameters:
    // - string json: JSON string containing Event data
    // Returns:
    // Event: deserialized Event object
    public static Event DeserializeEvent(string json)
    {
        return JsonSerializer.Deserialize<Event>(json, jsonConfig);
    }
}

/// <summary>
/// Represents a geographic location with address and coordinates
/// </summary>
class Location
{
    public string address { get; set; }
    public double latitude { get; set; }
    public double longitude { get; set; }
}

/// <summary>
/// Represents a user account with credentials and preferences
/// </summary>
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
    // Parameters:
    // - string json: JSON string containing User data
    // Returns:
    // User: deserialized User object
    public static User DeserializeUser(string json)
    {
        return JsonSerializer.Deserialize<User>(json, jsonConfig);
    }
}