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

class Client
{
    // TODO: Change to ApiClient
    /// <summary>
    ///  The main entry point for the client application.
    /// </summary>

    
    [STAThread]
    static void Main()
    {
        AppContext.Initialize();
        ApplicationConfiguration.Initialize();
        Application.Run(new Startup());
    }
    
}

