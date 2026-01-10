using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Text;
using System.Threading.Tasks;

namespace Server;

/// <summary>
/// Manages all database operations for the server
/// </summary>
internal class DatabaseManager
{
    // Initializes and opens database connection
    public DatabaseManager()
    {
        string passkey = System.Environment.GetEnvironmentVariable("DBPasskey");
        string connectionString =
            $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=EMS_NEA_Database.accdb;Jet OLEDB:Database Password={passkey};";
        ServerContext.connection = new OleDbConnection(connectionString);
        ServerContext.connection.Open();
        Console.WriteLine("Connected to database: " + ServerContext.connection.State);
    }


    // Inserts new event into database
    // Parameters:
    // - Event eventToInsert: Event object containing incident details
    // Returns:
    // Task for the asyncronous operation with no return value
    public async Task InsertEvent(Event eventToInsert)
    {
        const string query =
            "INSERT INTO Incident (incident_type, description, address, latitude, longitude, status, start_time, resolved_time) " +
            "VALUES (?, ?, ?, ?, ?, ?, ?, ?)";

        await using (OleDbCommand command = new OleDbCommand(query, ServerContext.connection))
        {
            command.Parameters.AddWithValue("@incident_type", eventToInsert.type);
            command.Parameters.AddWithValue("@description", eventToInsert.description);
            command.Parameters.AddWithValue("@address", eventToInsert.location.address);
            command.Parameters.AddWithValue("@latitude", eventToInsert.location.latitude);
            command.Parameters.AddWithValue("@longitude", eventToInsert.location.longitude);
            command.Parameters.AddWithValue("@status", eventToInsert.status);
            command.Parameters.AddWithValue("@start_time", eventToInsert.startTimestamp);
            command.Parameters.AddWithValue("@resolved_time", eventToInsert.resolvedTimestamp);

            await command.ExecuteNonQueryAsync();
        }
    }

    // Inserts new user into database with default settings
    // Parameters:
    // - User userToInsert: User object containing account details
    // Returns:
    // Task for the asyncronous operation with no return value
    public async Task InsertUser(User userToInsert)
    {
        const string query =
            "INSERT INTO [User] (first_name, last_name, phone_number, email_address, password_hash, latitude, longitude, is_receiving) " +
            "VALUES (?, ?, ?, ?, ?, ?, ?, ?)";

        await using (OleDbCommand command = new OleDbCommand(query, ServerContext.connection))
        {
            command.Parameters.AddWithValue("@first_name", userToInsert.firstName);
            command.Parameters.AddWithValue("@last_name", userToInsert.lastName);
            command.Parameters.AddWithValue("@phone_number", userToInsert.phoneNumber);
            command.Parameters.AddWithValue("@email_address", userToInsert.email);
            command.Parameters.AddWithValue("@password_hash", userToInsert.password);
            command.Parameters.AddWithValue("@latitude", 0);
            command.Parameters.AddWithValue("@longitude", 0);
            command.Parameters.AddWithValue("@is_receiving", false);
            
            await command.ExecuteNonQueryAsync();
            Console.WriteLine("Inserted user into database.");
        }
    }

    // Verifies user login credentials against database
    // Parameters:
    // - string email: user email address
    // - string passwordHash: hashed password to verify
    // Returns:
    // Task<string>: outcome message describing authentication result
    public async Task<string> CheckLoginDetails(string email, string passwordHash)
    {
        const string query = 
            "SELECT password_hash FROM [User] WHERE email_address = ?";

        await using (OleDbCommand command = new OleDbCommand(query, ServerContext.connection))
        {
            command.Parameters.AddWithValue("@email_address", email);

            string? retrievedHash = (string?) await command.ExecuteScalarAsync();

            if (retrievedHash == null)
            {
                return "Email not found.";
            } else if (retrievedHash != passwordHash)
            {
                return "Incorrect password.";
            }
            else
            {
                return "Correct logins.";
            }
        }
    }

    // Updates user alert preference in database
    // Parameters:
    // - bool alertChoice: new alert-receiving preference
    // - string email: user email address
    // Returns:
    // Task for the asyncronous operation with no return value
    public async Task ChangeAlertChoice(bool alertChoice, string email)
    {
        const string query = 
            "UPDATE [User] SET is_receiving = ?" +
            "WHERE email_address = ?";

        await using (OleDbCommand command = new OleDbCommand(query, ServerContext.connection))
        {
            command.Parameters.AddWithValue("@is_receiving", alertChoice);
            command.Parameters.AddWithValue("@email_address", email);
            await command.ExecuteNonQueryAsync();
            Console.WriteLine("Updated alert choice.");
        }
    }
    
    // Retrieves user alert receiving status from database
    // Parameters:
    // - string email: user email address
    // Returns:
    // Task<bool>: true if user is receiving alerts, false otherwise
    public async Task<bool> GetReceivingStatus(string email)
    {
        const string query = "SELECT is_receiving FROM [User] WHERE email_address = ?";

        await using (OleDbCommand command = new OleDbCommand(query, ServerContext.connection))
        {
            command.Parameters.AddWithValue("@email_address", email);
            bool? result = (bool?) await command.ExecuteScalarAsync();
            return result ?? false;
        }
    }

    // Retrieves user account details from database
    // Parameters:
    // - string email: user email address
    // Returns:
    // Task<Dictionary<string, string>>: user details dictionary
    public async Task<Dictionary<string, string>> RetrieveUserDetails(string email)
    {
        const string query = 
            "SELECT first_name, last_name, phone_number FROM [User]" +
            "WHERE email_address = ?";

        Dictionary<string, string> userDetails = new Dictionary<string, string>();

        await using (OleDbCommand command = new OleDbCommand(query, ServerContext.connection))
        {
            command.Parameters.AddWithValue("@email_address", email);
            await using (OleDbDataReader reader = (OleDbDataReader)await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    userDetails.Add("First Name", reader.GetString(0));
                    userDetails.Add("Last Name", reader.GetString(1));
                    userDetails.Add("Phone Number", reader.GetString(2));
                }
                else
                {
                    throw new Exception("User not found.");
                }
            }
        }
        
        return userDetails;
    }
    
    // Updates user account details
    // Parameters:
    // - Dictionary<string, string> newDetails: dictionary of fields to update
    // - string oldEmail: current email address for user identification
    // Returns:
    // Task<bool>: true if update successful
    public async Task<bool> UpdateUserDetails(Dictionary<string, string> newDetails, string oldEmail)
    {
        string query;
        StringBuilder queryBuilder = new StringBuilder("UPDATE [User] SET ");

        foreach (string field in newDetails.Keys)
        {
            queryBuilder.Append(GetColumnName(field));
            queryBuilder.Append(" = ?, ");
        }
        queryBuilder.Remove(queryBuilder.Length - 2, 2);
        
        queryBuilder.Append(" WHERE email_address = ?");
        query = queryBuilder.ToString();
        Console.WriteLine(query);

        await using (OleDbCommand command = new OleDbCommand(query, ServerContext.connection))
        {
            foreach (string fieldValue in newDetails.Values)
            {
                command.Parameters.AddWithValue("", fieldValue);
            }
            command.Parameters.AddWithValue("", oldEmail);
            
            await command.ExecuteNonQueryAsync();
            Console.WriteLine("Updated user details.");
        }
        return true;
    }

    // Finds all users in vicinity of incident location
    // Currently returns first user for testing purposes
    // Parameters:
    // - Location locationDetails: incident location coordinates
    // Returns:
    // Task<List<string>>: list of user emails in vicinity
    public async Task<List<string>> FindUsersInVicinity(Location locationDetails)
    {
        const string query = "SELECT email_address FROM [User]";
        List<string> emailsInRange = new List<string>();

        await using (OleDbCommand command = new OleDbCommand(query, ServerContext.connection))
        {
            await using (OleDbDataReader reader = (OleDbDataReader)await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    string email = reader.GetString(0);
                    emailsInRange.Add(email);
                }
            }
        }
        return emailsInRange;
    }

    // Maps form field tags to database column names
    // Parameters:
    // - string fieldTag: form field tag identifier
    // Returns:
    // string: corresponding database column name
    private string GetColumnName(string fieldTag)
    {
        switch (fieldTag)
        {
            case "FirstName":
                return "first_name";
            case "LastName":
                return "last_name";
            case "PhoneNumber":
                return "phone_number";
            case "Email":
            case "NewEmail":
                return "email_address";
            case "Password":
                return "password_hash";
            default:
                throw new Exception($"Invalid field tag {fieldTag}");
        }
    }
}