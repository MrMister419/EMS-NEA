using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client;

/// <summary>
/// Handles all client-server communication and business logic
/// Manages authentication, account operations, and event notifications
/// </summary>
public class AppService
{
    // TODO: Use token for authentication
    private static HttpService httpService;
    private Action<Dictionary<string, string>> eventHandler;

    public AppService()
    {
        httpService = new HttpService();
    }
    
    // Sends user signup request to server
    // Parameters:
    // - Dictionary<string, string> formValues: user registration data
    public void SignUp(Dictionary<string, string> formValues)
    {
        formValues["Password"] = Hash(formValues["Password"]);
        string wrappedJson = PackageJson(formValues, "NewUser");
        
        httpService.SendPOSTrequest(wrappedJson);
    }

    // Authenticates user credentials with server
    // Parameters:
    // - Dictionary<string, string> formValues: email and password
    // Returns:
    // Task<Dictionary<string, string>>: authentication result with outcome and success status
    public async Task<Dictionary<string, string>> Authenticate(Dictionary<string, string> formValues)
    {
        formValues["Password"] = Hash(formValues["Password"]);
        string wrappedJson = PackageJson(formValues, "Authenticate");
        
        Dictionary<string, string> response = await httpService.SendPOSTrequest(wrappedJson);
        return response;
    }

    // Registers handler for incoming event notifications
    // Parameters:
    // - Action<Dictionary<string, string>> handler: callback to invoke when event received
    public void RegisterEventHandler(Action<Dictionary<string, string>> handler)
    {
        eventHandler = handler;
    }
    
    // Retrieves user alert receiving preference from server
    // Returns:
    // Task for the asyncronous operation with no return value
    public async Task GetReceivingStatus()
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload.Add("Email", AppContext.email);
        string wrappedJson = PackageJson(payload, "GetReceivingStatus");
        Dictionary<string, string> response = await httpService.SendPOSTrequest(wrappedJson);
        if (response != null && response.ContainsKey("isReceiving"))
        {
            bool.TryParse(response["isReceiving"], out bool choice);
            AppContext.isReceiving = choice;
        }
    }

    // Initiates long-polling request to wait for event notifications
    public void RequestEvent()
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload.Add("Email", AppContext.email);
        string wrappedJson = PackageJson(payload, "RequestEvent");
        Task.Run(async () => { await ListenForEvent(wrappedJson); });
    }

    // Listens for event response and triggers handler when received
    // Parameters:
    // - string wrappedJson: packaged request JSON
    // Returns:
    // Task for the asyncronous operation with no return value
    private async Task ListenForEvent(string wrappedJson)
    {
        string responseJson = "";
        await httpService.SendPOSTrequest(wrappedJson, raw => responseJson = raw);
        if (!string.IsNullOrWhiteSpace(responseJson))
        {
            Dictionary<string, string> eventData = ParseEventJson(responseJson);
            if (eventData.Count > 0 && eventHandler != null)
            {
                eventHandler(eventData);
            }
        }
        if (AppContext.isReceiving)
        {
            RequestEvent();
        }
    }

    // Parses event JSON into dictionary format
    // Parameters:
    // - string json: raw event JSON
    // Returns:
    // Dictionary<string, string>: parsed event data
    private Dictionary<string, string> ParseEventJson(string json)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        try
        {
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                JsonElement root = doc.RootElement;
                if (root.TryGetProperty("type", out JsonElement type)) data["type"] = type.GetString();
                if (root.TryGetProperty("description", out JsonElement desc)) data["description"] = desc.GetString();
                if (root.TryGetProperty("status", out JsonElement status)) data["status"] = status.GetString();
                if (root.TryGetProperty("startTimestamp", out JsonElement start)) data["startTimestamp"] = start.GetString();
                if (root.TryGetProperty("resolvedTimestamp", out JsonElement resolved)) data["resolvedTimestamp"] = resolved.GetString();
                if (root.TryGetProperty("location", out JsonElement location))
                {
                    if (location.TryGetProperty("address", out JsonElement address)) data["location.address"] = address.GetString();
                    if (location.TryGetProperty("latitude", out JsonElement lat)) data["location.latitude"] = lat.ToString();
                    if (location.TryGetProperty("longitude", out JsonElement lon)) data["location.longitude"] = lon.ToString();
                }
            }
        }
        catch (JsonException) {}
        return data;
    }

    // Updates user alert preference on server
    // Parameters:
    // - bool newChoice: new alert receiving preference
    public void ToggleAlertChoice(bool newChoice)
    {
        string alertChoice = newChoice.ToString();
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload.Add("Email", AppContext.email);
        payload.Add("AlertChoice", alertChoice);
        string wrappedJson = PackageJson(payload, "ToggleAlertChoice");
        
        httpService.SendPOSTrequest(wrappedJson);
        AppContext.isReceiving = newChoice;
        if (newChoice)
        {
            RequestEvent();
        }
    }

    // Retrieves user account details from server
    // Returns:
    // Task<Dictionary<string, string>>: user account information
    public async Task<Dictionary<string, string>> GetAccountDetails()
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload.Add("Email", AppContext.email);
        
        string wrappedJson = PackageJson(payload, "GetAccountDetails");
        
        return await httpService.SendPOSTrequest(wrappedJson);
    }

    // Conditionally requests events if user is receiving
    // Parameters:
    // - bool isReceiving: current receiving status
    public void RequestEventIfReceiving(bool isReceiving)
    {
        if (isReceiving)
        {
            RequestEvent();
        }
    }

    // Sends updated account details to server after password verification
    // Parameters:
    // - Dictionary<string, string> formValues: updated account fields
    // Returns:
    // Task<Dictionary<string, string>>: update result with outcome and success status
    public async Task<Dictionary<string, string>> ModifyAccountDetails(Dictionary<string, string> formValues)
    {
        formValues = RemoveNullEntries(formValues);

        string passwordHash = Hash(formValues["ConfirmPassword"]);
        formValues.Remove("ConfirmPassword");
        formValues.Add("Password", passwordHash);
        formValues.Add("OldEmail", AppContext.email);
        
        string wrappedJson = PackageJson(formValues, "ModifyAccountDetails");
        
        return await httpService.SendPOSTrequest(wrappedJson);
    }
    
    // Removes empty entries from dictionary
    // Parameters:
    // - Dictionary<string, string> dictionary: dictionary to clean
    // Returns:
    // Dictionary<string, string>: dictionary with empty entries removed
    private Dictionary<string, string> RemoveNullEntries(Dictionary<string, string> dictionary)
    {
        List<string> keysToRemove = new List<string>();

        foreach (KeyValuePair<string, string> entry in dictionary)
        {
            if (entry.Value == "")
            {
                keysToRemove.Add(entry.Key);
            }
        }

        for (int index = 0; index < keysToRemove.Count; index++)
        {
            dictionary.Remove(keysToRemove[index]);
        }

        return dictionary;
    }

    // Wraps payload in JSON envelope with request type
    // Parameters:
    // - Dictionary<string, string> payload: data to send
    // - string type: request type identifier
    // Returns:
    // string: packaged JSON string
    private string PackageJson(Dictionary<string, string> payload, string type)
    {
        string packagedJson;
        Dictionary<string, object> wrappedValues = new Dictionary<string, object>();

        wrappedValues.Add("type", type);
        wrappedValues.Add("payload", payload);
        packagedJson = JsonSerializer.Serialize(wrappedValues);
        
        return packagedJson;
    }
    
    // Hashes password using SHA256
    // Parameters:
    // - string password: plaintext password
    // Returns:
    // string: hashed password in hexadecimal format
    private string Hash(string? password)
    {
        // TODO: Use salt?
        string hashedPassword = "";
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        foreach (byte b in bytes)
        {
            hashedPassword += b.ToString("x2");
        }
        password = null;
        // TODO: Use SecureString instead of string

        return hashedPassword;
    }
}

/// <summary>
/// Handles HTTP communication with server
/// </summary>
class HttpService
{
    private readonly HttpClient HttpClient = new HttpClient();
    
    public HttpService()
    {
        HttpClient.Timeout = TimeSpan.FromSeconds(120);
    }
    
    // Sends POST request to server and optionally captures raw response
    // Parameters:
    // - string json: JSON payload to send
    // - Action<string> rawHandler: optional callback to receive raw response
    // Returns:
    // Task<Dictionary<string, string>>: parsed response dictionary
    public async Task<Dictionary<string, string>> SendPOSTrequest(string json, Action<string> rawHandler = null)
    {
        Console.WriteLine(json);
        string responseString = "";
        Dictionary<string, string> responseJSON = new Dictionary<string, string>();
        
        try
        {
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await HttpClient.PostAsync("http://localhost:50000", content);

            responseString = await response.Content.ReadAsStringAsync();
            rawHandler?.Invoke(responseString);
            Console.WriteLine("response: " + responseString);
            if (!string.IsNullOrWhiteSpace(responseString))
            {
                try
                {
                    responseJSON = JsonSerializer.Deserialize<Dictionary<string, string>>(responseString);
                }
                catch (JsonException) {}
            }

            Console.WriteLine("\nPOST request successful.");
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("Connection error: " + e);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        return responseJSON;
    }
}

// class User
// {
//     public string Username { get; set; }
//     public string Email { get; set; }
//     public string Password { get; set; }
//
//     public User(string username, string email, string hashedPassword)
//     {
//         Username = username;
//         Email = email;
//         Password = hashedPassword;
//     }
// }
