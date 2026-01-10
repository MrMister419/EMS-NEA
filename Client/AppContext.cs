using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace Client;

/// <summary>
/// Holds global client application state and shared services
/// </summary>
static class AppContext
{
    public static FormNavigation formNavigator;
    public static FormManager formManager;
    public static AppService appService;
    public static string email { get; set; }
    public static bool isReceiving { get; set; }
    
    // Initializes all shared application services
    public static void Initialize()
    {
        formNavigator = new FormNavigation();
        formManager = new FormManager();
        appService = new AppService();
    }

    public static Dictionary<string, string>? DeserializeToDictionary(string? json)
    {
        Dictionary<string, string> deserialized;
        
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }
        else
        {
             deserialized= JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        }
        
        return deserialized;
    }
    
    // Hashes password using SHA256
    // Parameters:
    // - string password: plaintext password
    // Returns:
    // string: hashed password in hexadecimal format
    public static string Hash(string password)
    {
        // TODO: Use salt?
        string hashedPassword = "";
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        foreach (byte b in bytes)
        {
            hashedPassword += b.ToString("x2");
        }
        // TODO: Use SecureString instead of string

        return hashedPassword;
    }
}

/// <summary>
/// Manages form switching and navigation between application windows
/// </summary>
class FormManager
{
    // Switches between Startup and MainForm windows
    // Parameters:
    // Form currentForm: the form to hide and switch from
    public void SwitchForm(Form currentForm)
    {
        currentForm.Hide();

        if (currentForm is Startup)
        {
            Form mainForm = new MainForm();
            mainForm.Show();
        }
        else
        {
            Form startupForm = new Startup();
            startupForm.Show();
        }
    }
}

/// <summary>
/// Provides form navigation and control extraction utilities
/// </summary>
class FormNavigation
{ 
    
    // Extracts user input values from form controls based on Tag property
    // Parameters:
    // - Panel formPanel: parent panel containing input fields
    // Returns:
    // Dictionary<string, string>: field names mapped to entered values
    public Dictionary<string, string> GetEnteredValues(Panel formPanel)
    {
        Dictionary<string, string> formValues = new Dictionary<string, string>();
        List<TextBoxBase> textBoxes = GetControlsByType<TextBoxBase>(formPanel);

        string fieldName = "";
        string fieldEntry = "";
        
        foreach (TextBoxBase textBox in textBoxes)
        {
            fieldName = textBox.Tag.ToString();
            if (fieldName == "")
            {
                continue;
            }

            fieldEntry = textBox.Text.Trim();

            if (textBox is MaskedTextBox)
            {
                textBox.Text = "";
                // TODO: Use SecureString instead of string
            }
            
            formValues.Add(fieldName, fieldEntry);
        }
        
        return formValues;
    }

    // Recursively finds all controls of specified type within parent control
    // Parameters:
    // - Control parentControl: parent control to search within
    // - bool getBaseControlsOnly: if true, does not check inside components of required type
    // Returns:
    // List<type>: list of controls matching the specified type
    public List<type> GetControlsByType<type>(Control parentControl, bool getBaseControlsOnly = true) where type : Control
    {
        List<type> controls = new List<type>();
        
        foreach (Control control in parentControl.Controls)
        {
            List<type> subControls = GetControlsByType<type>(control, getBaseControlsOnly);
            if (control is type)
            {
                
                if (subControls.Count > 0 && !getBaseControlsOnly)
                {
                    controls.AddRange(subControls);
                }
                else
                {
                    controls.Add((type) control);
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
