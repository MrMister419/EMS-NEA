using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DispatchSystem;

/// <summary>
/// Computer-Aided Dispatch system for sending test events to server
/// </summary>
class DispatchSystem
{
    private static readonly Random Random = new Random();
    private static readonly HttpClient Client = new HttpClient();

    private static List<Dictionary<string, object>> eventsList;
    private static Dictionary<string, object> currentEvent;
    private static List<Dictionary<string, object>> sentEvents;

    // Entry point for CAD application
    // Loads sample events and sends test event to server
    // Returns: Task representing the async operation
    public static async Task Main(string[] args)
    {
        while (true)
        {
            eventsList = DeserializeEvents();
            currentEvent = GetRandomEvent(eventsList);
            sentEvents = new List<Dictionary<string, object>>();

            Console.WriteLine("Waiting to send. Press Enter to send event.");
            Console.ReadLine();

            StartCurrentEvent();
            await SendCurrentEvent();
        }    
    }

    // Loads sample events from JSON file
    // Returns:
    // List<Dictionary<string, object>>: list of event objects
    private static List<Dictionary<string, object>> DeserializeEvents()
    {
        using (StreamReader reader = new StreamReader("SampleEvents.json"))
        {
            string json = reader.ReadToEnd();
            Console.WriteLine(json);

            List<Dictionary<string, object>>
                events = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json);

            return events;
        }
    }

    // Serializes event dictionary to JSON string
    // Parameters:
    // - Dictionary<string, object> eventToSerialize: event data to serialize
    // Returns:
    // string: JSON representation of event
    private static string Serialize(Dictionary<string, object> eventToSerialize)
    {
        return JsonSerializer.Serialize(eventToSerialize);
    }

    // Selects random event from list
    // Parameters:
    // - List<Dictionary<string, object>> events: available events
    // Returns:
    // Dictionary<string, object>: randomly selected event
    private static Dictionary<string, object> GetRandomEvent(List<Dictionary<string, object>> events)
    {
        int index = Random.Next(0, events.Count - 1);
        return events[index];
    }

    // Initializes event with starting fields and timestamp
    private static void StartCurrentEvent()
    {
        currentEvent["eventID"] = sentEvents.Count;
        currentEvent["startTimestamp"] = DateTime.Now.ToLongTimeString();
    }

    // Sends current event to server via HTTP POST
    // Returns:
    // Task for the asyncronous operation with no return value
    private static async Task SendCurrentEvent()
    {
        Dictionary<string, object> wrappedValues = new Dictionary<string, object>();

        wrappedValues.Add("type", "NewEvent");
        wrappedValues.Add("payload", currentEvent);
        
        string json = Serialize(wrappedValues);
        Console.WriteLine(json);

        try
        {
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await Client.PostAsync("http://localhost:50000", content);

            response.EnsureSuccessStatusCode();
            Console.WriteLine("POST request successful.");
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        sentEvents.Add(currentEvent);
        eventsList.Remove(currentEvent);
    }

    // Marks current event as resolved with timestamp
    private static void ResolveCurrentEvent()
    {
        currentEvent["resolvedTimestamp"] = DateTime.Now.ToLongTimeString();
    }
}