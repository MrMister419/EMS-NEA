using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EMS_NEA;

internal class DispatchSystem
{
    private static readonly Random Random = new Random();
    private static readonly HttpClient Client = new HttpClient();

    private static List<Dictionary<string, object>> eventsList;
    private static Dictionary<string, object> currentEvent;
    private static List<Dictionary<string, object>> sentEvents;

    public static async Task Main(string[] args)
    {
        eventsList = DeserializeEvents();
        currentEvent = GetRandomEvent(eventsList);
        sentEvents = new List<Dictionary<string, object>>();
        Console.WriteLine(currentEvent);
        StartCurrentEvent();

        await SendCurrentEvent();
    }

    private static List<Dictionary<string, object>> DeserializeEvents()
    {
        using (StreamReader reader = new StreamReader("SampleEvents.json"))
        {
            string json = reader.ReadToEnd();
            Console.WriteLine(json);

            List<Dictionary<string, object>>
                events = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json);
            Console.WriteLine(events);

            return events;
        }
    }

    private static string Serialize(Dictionary<string, object> eventToSerialize)
    {
        return JsonSerializer.Serialize(eventToSerialize);
    }

    private static Dictionary<string, object> GetRandomEvent(List<Dictionary<string, object>> events)
    {
        int index = Random.Next(0, events.Count - 1);
        return events[index];
    }

    private static void StartCurrentEvent()
    {
        var obj = sentEvents.Count;
        currentEvent["eventID"] = sentEvents.Count;
        currentEvent["startTimestamp"] = DateTime.Now.ToLongTimeString();
    }

    private static async Task SendCurrentEvent()
    {
        // TODO: move to networking class?
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

    private static void ResolveCurrentEvent()
    {
        currentEvent["resolvedTimestamp"] = DateTime.Now.ToLongTimeString();
    }

    // Listen for client requests for events
    private static void ListenToClient()
    {
    }
}