using System;
using System.Windows.Forms;

namespace Client;

/// <summary>
/// Entry point for the client application
/// </summary>
class Client
{
    /// <summary>
    /// The main entry point for the client application.
    /// Initializes application context and starts with Startup form
    /// </summary>
    [STAThread]
    static void Main()
    {
        AppContext.Initialize();
        ApplicationConfiguration.Initialize();
        Application.Run(new Startup());
    }
}