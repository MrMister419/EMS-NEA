using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;

namespace EMS_NEA;

internal class Server
{
    private static DatabaseManager database;
    private static Event receivedEvent;

    public static async Task Main(string[] args)
    {
        database = new DatabaseManager();
        Listener listener = new Listener();
        // Subscribe ProcessMessage as event handler
        listener.MessageReceived += ProcessMessage;
        listener.ListenForEvents();
        
    }
    
    private static void ProcessMessage(object sender, string receivedJson)
    {
        string requestType = "";
        
        // Get request type from JSON
        using (JsonDocument doc = JsonDocument.Parse(receivedJson))
        {
            requestType = doc.RootElement.GetProperty("type").GetString();
        }
        
        switch (requestType)
        {
            case "NewUser":
                CreateNewUser(receivedJson);
                break;
            case "NewEvent":
                CreateNewEvent(receivedJson);
                break;
            case "UpdateUser":
                UpdateUser(receivedJson);
                break;
            case "UpdateEvent":
                UpdateEvent(receivedJson);
                break;
            default:
                Console.WriteLine("Unknown request type.");
                break;
        }
    }

    private static void CreateNewUser(string userJson)
    {
        
    }

    private static void CreateNewEvent(string eventJson)
    {
        receivedEvent = Event.DeserializeEvent(eventJson);
        database.InsertEvent(receivedEvent);
    }

    private static void UpdateUser(string userUpdateJson)
    {
        
    }
    
    private static void UpdateEvent(string eventUpdateJson)
    {
        
    }
    
}

class Listener
{
    private readonly HttpListener listener = new HttpListener();
    public event EventHandler<string> MessageReceived;
    
    public async Task ListenForEvents()
    {
        string receivedJson = "";
        while (true)
        {
            listener.Prefixes.Add("http://localhost:50000/");
            listener.Start();
            // Wait for a POST request
            HttpListenerContext context = await listener.GetContextAsync();
            // Accept the request
            HttpListenerRequest request = context.Request;

            // Extract data from POST request
            if (request.HttpMethod == "POST")
            {
                using (StreamReader reader = new StreamReader(request.InputStream))
                {
                    receivedJson = await reader.ReadToEndAsync();
                    Console.WriteLine(receivedJson);
                }
            }

            // Respond to POST request
            HttpListenerResponse response = context.Response;
            response.StatusCode = (int)HttpStatusCode.OK;
            response.Close();
            Console.WriteLine("POST request answered.");
            MessageReceived.Invoke(this, receivedJson);
        }
    }

    protected virtual void OnTransmissionReceivedEvent(string receivedJson)
    {
        MessageReceived?.Invoke(this, receivedJson);
    }
}

class Event
{
    public int eventID { get; set; }
    public string type { get; set; }
    public string description { get; set; }
    public Location location { get; set; }
    public string startTimestamp { get; set; }
    public string resolvedTimestamp { get; set; }
    public string status { get; set; }

    // Factory method to create Event by deserializing JSON
    public static Event DeserializeEvent(string json)
    {
        return JsonSerializer.Deserialize<Event>(json);
    }

    public string Serialize()
    {
        return JsonSerializer.Serialize(this);
    }
}

class Location
{
    public string address { get; set; }
    public double latitude { get; set; }
    public double longitude { get; set; }
}