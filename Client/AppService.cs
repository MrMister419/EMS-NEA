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

    public async Task<string> LogIn(Dictionary<string, string> formValues)
    {
        formValues["Password"] = Hash(formValues["Password"]);
        string wrappedJson = PackageJson(formValues, "CheckLogin");
        
        string response = await httpService.SendPOSTrequest(wrappedJson);
        return response;
    }

    public void ToggleAlertChoice()
    {
        
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
    
    public async Task<string> SendPOSTrequest(string json)
    {
        string responseMessage = "";
        try
        {
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await HttpClient.PostAsync("http://localhost:50000", content);

            responseMessage = await response.Content.ReadAsStringAsync();

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
        
        return responseMessage;
    }
}

class User
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public User(string username, string email, string hashedPassword)
    {
        Username = username;
        Email = email;
        Password = hashedPassword;
    }
}
