using System.Threading.Tasks;

namespace EMS_NEA;

/// <summary>
/// Responsible for initializing and managing core components of the server application.
/// It sets up the database, HTTP listener, and processes incoming requests.
/// </summary>
class Server
{
    
    // Entry point to server application
    public static async Task Main(string[] args)
    {
        await ServerContext.InitializeServer();
    }
    
}
