using System;
using System.Collections.Generic;
using System.Globalization;
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

    // Wrapper for request processing to handle exceptions
    // Parameters:
    // - HttpListenerContext context: HTTP context for the request
    // Returns:
    // Task for the asyncronous operation with no return value
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

    // Extracts and parses incoming request data before routing
    // Parameters:
    // - HttpListenerContext context: HTTP context for the request
    // Returns:
    // Task for the asyncronous operation with no return value
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
            await SendResponse(context, responseMessage);
        }
    }
    
    // Stores request in waiting list for long-polling notification
    // Parameters:
    // - HttpListenerContext context: HTTP context for the request
    // - string payload: JSON payload containing user identity
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
    // - Event evt: incident data to send
    // Returns:
    // Task for the asyncronous operation with no return value
    public async Task NotifyWaitingClients(Event evt)
    {
        // Check which clients are in vicinity and have placed an event request
        List<string> emailsToRespond = new List<string>();
        lock (waitingClients)
        {
            foreach (PendingRequest pending in waitingClients)
            {
                emailsToRespond.Add(pending.Email);
            }
        }

        // The locations of the users with a pending event request
        List<Dictionary<string, string>> userLocations = await ServerContext.database.GetLocationOfUsers(emailsToRespond);

        // Send event notification to waiting clients in vicinity
        PendingRequest pendingRequest = new PendingRequest();
        string eventPayloadJson = JsonSerializer.Serialize(evt);
        foreach (Dictionary<string, string> location in userLocations)
        {
            // Check if user is in vicinity of event
            string userEmail = location["Email"];
            string userLat = location["Latitude"];
            string userLong = location["Longitude"];
            string eventLat = evt.location.latitude.ToString(CultureInfo.InvariantCulture);
            string eventLong = evt.location.longitude.ToString(CultureInfo.InvariantCulture);
            bool isInVicinity = await ServerContext.requestHandler.CheckDistance(userLat, userLong, eventLat, eventLong);

            // Answer and remove request if user is in vicinity
            if (isInVicinity)
            {
                // Find the pending request for this user
                lock (waitingClients)
                {
                    foreach (PendingRequest pending in waitingClients)
                    {
                        if (pending.Email == userEmail)
                        {
                            pendingRequest = pending;
                            break;
                        }
                    }
                }
                
                bool sent = await SendResponse(pendingRequest.Context, eventPayloadJson);
                
                if (sent)
                {
                    lock (waitingClients)
                    {
                        waitingClients.Remove(pendingRequest);
                    }
                }
            }        
        }
    }

    // Sends HTTP response with JSON payload
    // Parameters:
    // - HttpListenerContext requestContext: HTTP context to send response to
    // - string responseJson: JSON data to send
    // Returns:
    // Task<bool>: true if response sent successfully
    private async Task<bool> SendResponse(HttpListenerContext requestContext, string responseJson)
    {
        try
        {
            HttpListenerResponse response = requestContext.Response;
            response.ContentType = "application/json";
            byte[] buffer = Encoding.UTF8.GetBytes(responseJson);
            response.ContentLength64 = buffer.Length;
            
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            response.Close();
            
            Console.WriteLine("Sent response: " + responseJson);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to notify waiting client: {ex}");
            return false;
        }
    }
    
    // Nested class to store waiting client connection details
    private class PendingRequest
    {
        public HttpListenerContext Context { get; set; }
        public string Email { get; set; }
    }
}
