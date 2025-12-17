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
    
    private static void SignUp()
    {
        
    }

    private static void Login()
    {
        
    }
}