using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Text;

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