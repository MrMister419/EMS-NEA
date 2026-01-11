using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client;

/// <summary>
/// Handles HTTP communication with server
/// </summary>
class HttpService
{
    private readonly HttpClient HttpClient = new HttpClient();
    
    public HttpService()
    {
        HttpClient.Timeout = TimeSpan.FromSeconds(180);
    }
    
    public async Task<string?> SendPOSTrequest(string json)
    {
        Console.WriteLine("\nSending POST request: " + json);
        string responseString = "";

        try
        {
            StringContent content = new StringContent(json, Encoding.UTF8, mediaType: "application/json");
            HttpResponseMessage response = await HttpClient.PostAsync("http://localhost:50000", content);

            responseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine("response received: " + responseString);

            if (!string.IsNullOrWhiteSpace(responseString))
            {
                Console.WriteLine("POST request successful.");
            }
            else
            {
                Console.WriteLine("Invalid format received on POST response.");
                return null;
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("Connection error: " + e);
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("Request timed out");
        }
        catch (Exception e)
        {
            Console.WriteLine("POST failed: " + e);
        }
        
        return responseString;
    }
}