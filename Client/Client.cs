using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualBasic.Logging;

namespace Client;

static class Client
{
    /// <summary>
    ///  The main entry point for the client application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new Startup());
    }
    
    public static void SignUp(Dictionary<string, string> formValues)
    {
        foreach (var value in formValues)
        {
            Console.WriteLine(value.Key + " " + value.Value);
        }
        // Console.WriteLine(formValues["Password"]);
        formValues["Password"] = Hash(formValues["Password"]);

        // foreach (var value in formValues)
        // {
        //     Console.WriteLine(value.Key + " " + value.Value);
        // }

    }

    public static void Login()
    {
        
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
        Console.WriteLine(fieldPanels.Count);

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
                        fieldName = fieldName.Replace("*", "").Trim();
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
                
                Console.WriteLine(fieldName + " " + fieldEntry + " type: " + panel.GetType() + " Text: " +
                                  panel.Text + " Name: " + panel.Name);
                Console.WriteLine("----------------------------------------");
                Console.WriteLine(typeof(TextBox));
            }
            formValues.Add(fieldName, fieldEntry);
        }
        
        Console.WriteLine(formValues.Count);
        return formValues;
    }

    private List<Control> GetControlByType<type>(Control parentControl = null, bool getBaseControlsOnly = true) where type : Control
    {
        List<Control> controls = new List<Control>();
        
        if (parentControl == null)
        {
            parentControl = form;
        }
        
        Console.WriteLine(typeof(type));
        Console.WriteLine(parentControl.GetType());
        
        foreach (Control control in parentControl.Controls)
        {
            List<Control> subControls = GetControlByType<type>(control, getBaseControlsOnly);
            Console.WriteLine(control.Name);
            if (control is type)
            {
                
                if (subControls.Count > 0 && !getBaseControlsOnly)
                {
                    Console.WriteLine("flag1");

                    controls.AddRange(subControls);
                }
                else
                {
                    Console.WriteLine("flag2");

                    controls.Add(control);
                }
            }
            else
            {
                controls.AddRange(subControls);
            }
        }
        Console.WriteLine("found: " + controls.Count);
        
        return controls;
    }
}