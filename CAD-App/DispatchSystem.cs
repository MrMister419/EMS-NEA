using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EMS_NEA;

class DispatchSystem
{
    private static List<Event> sentEvents;
    private static readonly Random Random = new Random();
    private static readonly HttpClient client = new HttpClient();
    
    public static async Task Main(string[] args)
    {
        sentEvents = DeserializeEvents();
        Event randomEvent = GetRandomEvent(sentEvents);
        Console.WriteLine(randomEvent);
        Console.WriteLine(randomEvent.Serialize());
        //randomEvent.StartEvent(sentEvents.Count);
        //await SendEvent(randomEvent);
        
    }
    
    // Listen for client requests for events
    private static void ListenToClient()
    {
        
    }

    private static List<Event> DeserializeEvents()
    {
        using (StreamReader reader = new StreamReader("SampleEvents.json"))
        {
            string json = reader.ReadToEnd();
            Console.WriteLine(json);

            List<Event> events = JsonSerializer.Deserialize<List<Event>>(json);
            Console.WriteLine(events);

            return events;
        }
    }

    private static Event GetRandomEvent(List<Event> events)
    {
        int index = Random.Next(0, events.Count - 1);
        return events[index];
    }

    private static async Task SendEvent(Event eventToSend)
    {
        string json = eventToSend.Serialize();
        Console.WriteLine(json);
        
        try
        {
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("http://localhost:50000", content);
            
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
        
        sentEvents.Add(eventToSend);
    }
}

class Event
{
    public int eventID { get; set; }
    private string startTimestamp { get; set; }
    private string resolvedTimestamp { get; set; }

    public void StartEvent(int eventID)
    {
        this.eventID = eventID;
        startTimestamp = DateTime.Now.ToLongTimeString();
        Console.WriteLine("Event " + this.eventID + " started at " + startTimestamp);
    }
    
    public void ResolveEvent()
    {
        resolvedTimestamp = DateTime.Now.ToLongTimeString();
        Console.WriteLine("Event " + eventID + " resolved at " + resolvedTimestamp);
    }
    
    public string Serialize()
    {
        return JsonSerializer.Serialize(this);
    }
}