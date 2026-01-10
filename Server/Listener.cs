using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server;

/// <summary>
/// Handles HTTP listening and routing for incoming client requests
/// Manages long-polling connections for event notifications
/// </summary>
class Listener
{
    private readonly HttpListener listener = new HttpListener();
    private readonly List<PendingRequest> waitingClients = new List<PendingRequest>();
    // Request handler delegate which takes a string argument and returns a string
    public Func<string, Task<string>> RequestHandler;

    
    // Listens for incoming HTTP requests and routes them appropriately
    // Returns:
    // Task for the asyncronous operation with no return value
    public async Task Listen()
    {
        listener.Prefixes.Add("http://localhost:50000/");
        listener.Start();
        
        while (true)
        {
            // Wait for a POST request
            HttpListenerContext context = await listener.GetContextAsync();
            _ = Task.Run(() => ProcessRequestAsyncWrapper(context));
        }
    }

    private async Task ProcessRequestAsyncWrapper(HttpListenerContext context)
    {
        try
        {
            await ProcessRequest(context);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error while processing POST request: " + e);
        }
    }

    private async Task ProcessRequest(HttpListenerContext context)
    {
        // Extract the request
        HttpListenerRequest request = context.Request;

        // Extract data from POST request
        string receivedJson = "";
        using (StreamReader reader = new StreamReader(request.InputStream))
        {
            receivedJson = await reader.ReadToEndAsync();
            Console.WriteLine("\nReceived request" + receivedJson);
        }
        
        // Parse JSON data to type and payload components
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
            return;
        }
        
        // Add to long-polled requests if it is an event request
        if (requestType == "RequestEvent")
        {
            LongPollRequest(context, payload);
        }
        else
        {
            string responseMessage = await RequestHandler.Invoke(receivedJson);
            HttpListenerResponse response = context.Response;
            
            if (responseMessage == "OK")
            {
                response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
                Console.WriteLine("Responding with: " + responseMessage);
                response.ContentType = "application/json";
                byte[] buffer = Encoding.UTF8.GetBytes(responseMessage);
                response.ContentLength64 = buffer.Length;
                
                await response.OutputStream.WriteAsync(buffer);
            }
            response.Close();
            Console.WriteLine("POST request answered.");
        }
    }
    
    private void LongPollRequest(HttpListenerContext context, string payload)
    {
        string? email = ExtractEmail(payload);
        
        // If email is missing, respond with bad request status
        if (string.IsNullOrWhiteSpace(email))
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.Close();
        }
        else
        {
            lock (waitingClients)
            {
                PendingRequest item = new PendingRequest();
                item.Context = context;
                item.Email = email;
                    
                waitingClients.Add(item);
                Console.WriteLine("Long polled request from " + email);
            }
        }
    }
    
    // Extracts email from JSON payload
    // Parameters:
    // - string payloadJson: JSON string containing Email field
    // Returns:
    // string: extracted email or empty string if not found
    private string? ExtractEmail(string payloadJson)
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
        catch (JsonException e)
        {
            Console.WriteLine("Failed to parse JSON payload: " + e);
        }
        return string.Empty;
    }
    
    // Asyncronously notifies waiting clients in vicinity of incident
    // Parameters:
    // - List<string> targetEmails: list of emails to notify
    // - string eventPayloadJson: JSON string containing event data to send
    public async Task NotifyWaitingClients(List<string> targetEmails, string eventPayloadJson)
    {
        // Check which clients are in vicinity and have placed an event request
        List<PendingRequest> toRespond = new List<PendingRequest>();
        lock (waitingClients)
        {
            List<PendingRequest> remaining = new List<PendingRequest>();
            foreach (string target in targetEmails)
            {
                foreach (PendingRequest pending in waitingClients)
                {
                    if (pending.Email == target)
                    {
                        toRespond.Add(pending);
                        break;
                    }
                    else
                    {
                        remaining.Add(pending);
                    }
                }
            }
            waitingClients.Clear();
            waitingClients.AddRange(remaining);
        }
        
        // Send event notification to waiting clients in vicinity
        foreach (PendingRequest pending in toRespond)
        {
            try
            {
                HttpListenerResponse response = pending.Context.Response;
                response.ContentType = "application/json";
                byte[] buffer = Encoding.UTF8.GetBytes(eventPayloadJson);
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                response.Close();
                Console.WriteLine("Notified waiting client: " + pending.Email);
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