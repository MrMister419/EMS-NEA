using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client;

public class AppService
{
    // TODO: Use token for authentication
    private static HttpService httpService;

    public AppService()
    {
        httpService = new HttpService();
    }
    
    public void SignUp(Dictionary<string, string> formValues)
    {
        // TODO: Output confirmation message / error if user already exists
        formValues["Password"] = Hash(formValues["Password"]);
        string wrappedJson = PackageJson(formValues, "NewUser");
        
        httpService.SendPOSTrequest(wrappedJson);
    }

    public async Task<Dictionary<string, string>> Authenticate(Dictionary<string, string> formValues)
    {
        formValues["Password"] = Hash(formValues["Password"]);
        string wrappedJson = PackageJson(formValues, "Authenticate");
        
        Dictionary<string, string> response = await httpService.SendPOSTrequest(wrappedJson);
        return response;
    }

    public void ToggleAlertChoice(bool newChoice)
    {
        string alertChoice = newChoice.ToString();
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload.Add("Email", AppContext.email);
        payload.Add("AlertChoice", alertChoice);
        string wrappedJson = PackageJson(payload, "ToggleAlertChoice");
        
        httpService.SendPOSTrequest(wrappedJson);
    }

    public async Task<Dictionary<string, string>> GetAccountDetails()
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload.Add("Email", AppContext.email);
        
        string wrappedJson = PackageJson(payload, "GetAccountDetails");
        
        return await httpService.SendPOSTrequest(wrappedJson);
    }

    public async Task<Dictionary<string, string>> ModifyAccountDetails(Dictionary<string, string> formValues)
    {
        RemoveNullEntries(formValues);
        string json = JsonSerializer.Serialize(formValues);
        Console.WriteLine(json);
        
        string passwordHash = Hash(formValues["ConfirmPassword"]);
        
        formValues.Remove("ConfirmPassword");
        formValues.Add("Password", passwordHash);
        formValues.Add("New Email", formValues["Email"]);
        formValues["Email"] = AppContext.email;
        
        string wrappedJson = PackageJson(formValues, "ModifyAccountDetails");
        
        return await httpService.SendPOSTrequest(wrappedJson);
    }
    
    private void RemoveNullEntries(Dictionary<string, string> dictionary)
    {
        List<string> keysToRemove = new List<string>();

        foreach (KeyValuePair<string, string> entry in dictionary)
        {
            if (entry.Value == "" && entry.Key != "Email")
            {
                keysToRemove.Add(entry.Key);
            }
        }

        foreach (string key in keysToRemove)
        {
            dictionary.Remove(key);
        }
    }

    private string PackageJson(Dictionary<string, string> payload, string type)
    {
        string packagedJson;
        Dictionary<string, object> wrappedValues = new Dictionary<string, object>();

        wrappedValues.Add("type", type);
        wrappedValues.Add("payload", payload);
        packagedJson = JsonSerializer.Serialize(wrappedValues);
        
        return packagedJson;
    }
    
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

class HttpService
{
    private readonly HttpClient HttpClient = new HttpClient();
    
    public async Task<Dictionary<string, string>> SendPOSTrequest(string json)
    {
        string responseString = "";
        Dictionary<string, string> responseJSON = new Dictionary<string, string>();
        
        try
        {
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await HttpClient.PostAsync("http://localhost:50000", content);

            responseString = await response.Content.ReadAsStringAsync();
            responseJSON = JsonSerializer.Deserialize<Dictionary<string, string>>(responseString);

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
