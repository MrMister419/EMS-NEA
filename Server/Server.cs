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

class Server
{
    private static DatabaseManager database;
    private static readonly HttpListener listener = new HttpListener();
    private static Event receivedEvent;

    public static async Task Main(string[] args)
    {
        Event _event = await ListenForEvents();
        database = new DatabaseManager();
        Console.WriteLine(receivedEvent.Serialize());
        database.InsertEvent(receivedEvent);
    }
    

    private static async Task<Event> ListenForEvents()
    {
        listener.Prefixes.Add("http://localhost:50000/");
        listener.Start();
        while (true)
        {
            // Wait for a POST request
            HttpListenerContext context = await listener.GetContextAsync();
            HttpListenerRequest request = context.Request;
            
            // Get Event from POST request
            if (request.HttpMethod == "POST")
            {
                using (StreamReader reader = new StreamReader(request.InputStream))
                {
                    string json = await reader.ReadToEndAsync();
                    Console.WriteLine(json);
                    receivedEvent = Event.Deserialize(json);
                }
            }
            
            // Respond to POST request
            HttpListenerResponse response = context.Response;
            response.StatusCode = (int)HttpStatusCode.OK;
            response.Close();
            Console.WriteLine("POST request answered.");
            return receivedEvent;
        }
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
    public static Event Deserialize(string json)
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