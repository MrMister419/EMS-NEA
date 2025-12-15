using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace EMS_NEA;

class Server
{
    private static readonly HttpListener listener = new HttpListener();
    private static Event receivedEvent;

    public static async Task Main(string[] args)
    {
        Event _event = await ListenForEvents();
        Console.WriteLine(SerializeEvent(receivedEvent));
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
                    receivedEvent = DeserializeEvent(json);
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
    
    private static Event DeserializeEvent(string jsonEvent)
    {
        Event serializedEvent = JsonSerializer.Deserialize<Event>(jsonEvent);

        return serializedEvent;
    }

    private static string SerializeEvent(Event eventToSerialize)
    {
        string json = JsonSerializer.Serialize(eventToSerialize);
        return json;
    }
}

class Event
{
    private int eventID;
    private string startTimestamp;
    private string resolvedTimestamp;
    
}