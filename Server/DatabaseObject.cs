using System.Text.Json;

namespace Server;

/// <summary>
/// Base class for database objects
/// </summary>
abstract class DatabaseObject
{
    public int id { get; set; }
    protected static JsonSerializerOptions jsonConfig = 
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    // Serializes object to JSON string
    // Returns:
    // string: JSON representation of object
    public string Serialize()
    {
        return JsonSerializer.Serialize(this);
    }
}

/// <summary>
/// Represents a user account with credentials and preferences
/// </summary>
class User : DatabaseObject
{
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string email { get; set; }
    public string phoneNumber { get; set; }
    public string password { get; set; }
    public Location location { get; set; }
    public bool isReceiving { get; set; }
    
    // Factory method to create a User instance by deserializing JSON
    // Parameters:
    // - string json: JSON string containing User data
    // Returns:
    // User: deserialized User object
    public static User DeserializeUser(string json)
    {
        return JsonSerializer.Deserialize<User>(json, jsonConfig);
    }
}

/// <summary>
/// Represents an emergency incident event
/// </summary>
class Event : DatabaseObject
{
    public string type { get; set; }
    public string description { get; set; }
    public Location location { get; set; }
    public string startTimestamp { get; set; }
    public string resolvedTimestamp { get; set; }
    public string status { get; set; }
    
    // Factory method to create an Event instance by deserializing JSON
    // Parameters:
    // - string json: JSON string containing Event data
    // Returns:
    // Event: deserialized Event object
    public static Event DeserializeEvent(string json)
    {
        return JsonSerializer.Deserialize<Event>(json, jsonConfig);
    }
}

/// <summary>
/// Represents a geographic location with address and coordinates
/// </summary>
class Location
{
    public string address { get; set; }
    public double latitude { get; set; }
    public double longitude { get; set; }
}