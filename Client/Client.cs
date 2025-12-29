using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualBasic.Logging;

namespace Client;

static class Client
{
    /// <summary>
    ///  The main entry point for the client application.
    /// </summary>

    private static NetworkClient networkClient;
    
    [STAThread]
    static void Main()
    {
        networkClient = new NetworkClient();
        ApplicationConfiguration.Initialize();
        Application.Run(new Startup());
    }
    
    public static async Task SignUp(Dictionary<string, string> formValues)
    {
        formValues["Password"] = Hash(formValues["Password"]);
        string wrappedJson = PackageJson(formValues, "NewUser");
        
        networkClient.SendPOSTrequest(wrappedJson);
    }

    public static async Task<string> LogIn(Dictionary<string, string> formValues)
    {
        formValues["Password"] = Hash(formValues["Password"]);
        string wrappedJson = PackageJson(formValues, "CheckLogin");
        
        string response = await networkClient.SendPOSTrequest(wrappedJson);
        return response;
    }

    private static string PackageJson(Dictionary<string, string> payload, string type)
    {
        string packagedJson;
        Dictionary<string, object> wrappedValues = new Dictionary<string, object>();

        wrappedValues.Add("type", type);
        wrappedValues.Add("payload", payload);
        packagedJson = JsonSerializer.Serialize(wrappedValues);
        
        return packagedJson;
    }
    
    private static string Hash(string? password)
    {
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

class NetworkClient
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
            Console.WriteLine("message" + responseMessage);

            Console.WriteLine("POST request successful.");
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

public class FormNavigation
{ // TODO: Move to partial class?
    
    private readonly Form form;
    
    public FormNavigation(Form form)
    {
        this.form = form;
    }
    
    public Dictionary<string, string> GetEnteredValues(Control formPanel)
    {
        Dictionary<string, string> formValues = new Dictionary<string, string>();
        List<Control> fieldPanels = GetControlByType<Panel>(formPanel);

        string fieldName = "";
        string fieldEntry = "";
        
        foreach (Control panel in fieldPanels)
        {
            foreach (Control childControl in panel.Controls)
            {
                switch (childControl)
                {
                    case Label:
                        fieldName = childControl.Text;
                        fieldName = fieldName.Replace("*", "").Replace(" ", "");
                        break;
                    case TextBox:
                    case MaskedTextBox:
                    {
                        fieldEntry = childControl.Text.Trim();

                        if (childControl is MaskedTextBox)
                        {
                            childControl.Text = "";
                            // TODO: Use SecureString instead of string
                        }

                        break;
                    }
                }
                
            }
            formValues.Add(fieldName, fieldEntry);
        }
        
        return formValues;
    }

    private List<Control> GetControlByType<type>(Control parentControl = null, bool getBaseControlsOnly = true) where type : Control
    {
        List<Control> controls = new List<Control>();
        
        if (parentControl == null)
        {
            parentControl = form;
        }
        
        foreach (Control control in parentControl.Controls)
        {
            List<Control> subControls = GetControlByType<type>(control, getBaseControlsOnly);
            if (control is type)
            {
                
                if (subControls.Count > 0 && !getBaseControlsOnly)
                {
                    controls.AddRange(subControls);
                }
                else
                {
                    controls.Add(control);
                }
            }
            else
            {
                controls.AddRange(subControls);
            }
        }
        
        return controls;
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
