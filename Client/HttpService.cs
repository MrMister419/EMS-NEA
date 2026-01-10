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
        HttpClient.Timeout = TimeSpan.FromSeconds(60);
    }
    
    public async Task<Dictionary<string, string>> SendPOSTrequest(string json)
    {
        Console.WriteLine("\nSending POST request: " + json);
        Dictionary<string, string> responseJSON = new Dictionary<string, string>();
        
        try
        {
            StringContent content = new StringContent(json, Encoding.UTF8, mediaType:"application/json");
            HttpResponseMessage response = await HttpClient.PostAsync("http://localhost:50000", content);

            string responseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine("response received: " + responseString);
            
            if (!string.IsNullOrWhiteSpace(responseString))
            {
                try
                {
                    responseJSON = JsonSerializer.Deserialize<Dictionary<string, string>>(responseString);
                    Console.WriteLine("POST request successful.");
                }
                catch (JsonException e)
                {
                    Console.WriteLine("Error parsing JSON: " + e);
                }
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("Connection error: " + e);
        }
        catch (Exception e)
        {
            Console.WriteLine("POST failed: " + e);
        }
        
        return responseJSON;
    }
}