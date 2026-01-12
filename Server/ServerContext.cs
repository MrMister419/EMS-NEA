using System.Net.Http;
using System.Threading.Tasks;

namespace Server;

/// <summary>
/// Holds global server components and manages server initialization
/// </summary>
static class ServerContext
{
    public static RequestHandler requestHandler;
    public static DatabaseManager database;
    public static HttpClient httpClient;
    public static Listener listener;

    // Initializes all server components and starts listening for requests
    // Returns:
    // Task: asyncronous operation with no return value
    public static async Task InitializeServer()
    {
        requestHandler = new RequestHandler();
        httpClient = new HttpClient();
        database = new DatabaseManager();
        listener = new Listener();
        // Adds the ProcessRequest method as the request handler for the listener
        listener.RequestHandler = requestHandler.ProcessRequest;
        
        await listener.Listen();
    }
}