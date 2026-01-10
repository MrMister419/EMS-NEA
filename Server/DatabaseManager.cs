using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Server;

/// <summary>
/// Manages all database operations for the server
/// </summary>
internal class DatabaseManager
{
    private string connectionString;
    
    // Initializes and opens database connection
    public DatabaseManager()
    {
        string passkey = System.Environment.GetEnvironmentVariable("DBPasskey");
        connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=EMS_NEA_Database.accdb;Jet OLEDB:Database Password={passkey};";
    }


    // Inserts new event into database
    // Parameters:
    // - Event eventToInsert: Event object containing incident details
    // Returns:
    // Task for the asyncronous operation with no return value
    public async Task<Dictionary<string, string>> InsertEvent(Event eventToInsert)
    {
        const string query =
            "INSERT INTO Incident (incident_type, description, address, latitude, longitude, status, start_time, resolved_time) " +
            "VALUES (?, ?, ?, ?, ?, ?, ?, ?)";
        Dictionary<string, string> outcome;

        try
        {
            await using (OleDbConnection connection = new OleDbConnection(connectionString))
            await using (OleDbCommand command = new OleDbCommand(query, connection))
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

            outcome = FormulateOutcome(true, "Event inserted successfully.");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error inserting event into database: " + e);
            outcome = FormulateOutcome(false, "Error inserting event into database.");
        }

        return outcome;
    }

    // Inserts new user into database with default settings
    // Parameters:
    // - User userToInsert: User object containing account details
    // Returns:
    // Task for the asyncronous operation with no return value
    public async Task<Dictionary<string, string>> InsertUser(User userToInsert)
    {
        const string query =
            "INSERT INTO [User] (first_name, last_name, phone_number, email_address, password_hash, latitude, longitude, is_receiving) " +
            "VALUES (?, ?, ?, ?, ?, ?, ?, ?)";
        Dictionary<string, string> outcome;

        try
        {
            await using (OleDbConnection connection = new OleDbConnection(connectionString))
            await using (OleDbCommand command = new OleDbCommand(query, connection))
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
            outcome = FormulateOutcome(true, "User added successfully.");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error inserting user into database: " + e);
            outcome = FormulateOutcome(false, "Error inserting user into database.");
        }

        return outcome;
    }

    // Verifies user login credentials against database
    // Parameters:
    // - string email: user email address
    // - string passwordHash: hashed password to verify
    // Returns:
    // Task<string>: outcome message describing authentication result
    public async Task<Dictionary<string, string>> CheckLoginDetails(string email, string passwordHash)
    {
        const string query = 
            "SELECT password_hash FROM [User] WHERE email_address = ?";
        Dictionary<string, string> outcome;

        try
        {
            await using (OleDbConnection connection = new OleDbConnection(connectionString))
            await using (OleDbCommand command = new OleDbCommand(query, connection))
            {
                command.Parameters.AddWithValue("@email_address", email);

                // Returns null if email is not found
                string? retrievedHash = (string?) await command.ExecuteScalarAsync();

                if (retrievedHash == null)
                {
                    outcome = FormulateOutcome(false, "User not found.");
                } else if (retrievedHash != passwordHash)
                {
                    outcome = FormulateOutcome(false, "Incorrect password.");
                }
                else
                {
                    outcome = FormulateOutcome(true, "Correct logins.");
                }
            }

        }
        catch (Exception e)
        {
            Console.WriteLine("Error checking login details: " + e);
            outcome = FormulateOutcome(false, "Error checking login details.");
        }
        
        return outcome;
    }

    // Updates user alert preference in database
    // Parameters:
    // - bool alertChoice: new alert-receiving preference
    // - string email: user email address
    // Returns:
    // Task for the asyncronous operation with no return value
    public async Task<Dictionary<string, string>> ChangeAlertChoice(bool alertChoice, string email)
    {
        const string query = 
            "UPDATE [User] SET is_receiving = ? WHERE email_address = ?";
        Dictionary<string, string> outcome;

        try
        {
            await using (OleDbConnection connection = new OleDbConnection(connectionString))
            await using (OleDbCommand command = new OleDbCommand(query, connection))
            {
                command.Parameters.AddWithValue("@is_receiving", alertChoice);
                command.Parameters.AddWithValue("@email_address", email);
                await command.ExecuteNonQueryAsync();
            }
            outcome = FormulateOutcome(true, "Alert preference updated successfully.");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error updating alert choice: " + e);
            outcome = FormulateOutcome(false, "Error updating alert choice.");
        }
        
        return outcome;
    }
    
    // Retrieves user alert receiving status from database
    // Parameters:
    // - string email: user email address
    // Returns:
    // Task<bool>: true if user is receiving alerts, false otherwise
    public async Task<Dictionary<string, string>> GetReceivingStatus(string email)
    {
        const string query = "SELECT is_receiving FROM [User] WHERE email_address = ?";
        Dictionary<string, string> outcome;
        Dictionary<string, string> receivingStatus = new Dictionary<string, string>();
        
        try
        {
            bool? result;
            await using (OleDbConnection connection = new OleDbConnection(connectionString))
            await using (OleDbCommand command = new OleDbCommand(query, connection))
            {
                command.Parameters.AddWithValue("@email_address", email);
                result = (bool?) await command.ExecuteScalarAsync();
            }
            
            if (result != null)
            {
                receivingStatus.Add("isReceiving", result.Value.ToString());
                outcome = FormulateOutcome(true, "Status retrieved.", receivingStatus);
            }
            else
            {
                outcome = FormulateOutcome(false, "Email not found.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            outcome = FormulateOutcome(false, "Error retrieving status.");
        }
        
        return outcome;
    }

    // Retrieves user account details from database
    // Parameters:
    // - string email: user email address
    // Returns:
    // Task<Dictionary<string, string>>: user details dictionary
    public async Task<Dictionary<string, string>> RetrieveUserDetails(string email)
    {
        const string query = 
            "SELECT first_name, last_name, phone_number FROM [User] WHERE email_address = ?";
        Dictionary<string, string> outcome;
        Dictionary<string, string> userDetails = new Dictionary<string, string>();

        try
        {
            await using (OleDbConnection connection = new OleDbConnection(connectionString))
            await using (OleDbCommand command = new OleDbCommand(query, connection))
            {
                command.Parameters.AddWithValue("@email_address", email);
                await using (OleDbDataReader reader = (OleDbDataReader)await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        userDetails.Add("First Name", reader.GetString(0));
                        userDetails.Add("Last Name", reader.GetString(1));
                        userDetails.Add("Phone Number", reader.GetString(2));
                        
                        outcome = FormulateOutcome(true, "User details retrieved.", userDetails);
                    }
                    else
                    {
                        outcome = FormulateOutcome(false, "Email not found.");
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error retrieving user details: " + e);
            outcome = FormulateOutcome(false, "Error retrieving user details.");
        }
        
        return outcome;
    }
    
    // Updates user account details
    // Parameters:
    // - Dictionary<string, string> newDetails: dictionary of fields to update
    // - string oldEmail: current email address for user identification
    // Returns:
    // Task<bool>: true if update successful
    public async Task<Dictionary<string, string>> UpdateUserDetails(Dictionary<string, string> newDetails, string oldEmail)
    {
        Dictionary<string, string> outcome;
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

        try
        {
            await using (OleDbConnection connection = new OleDbConnection(connectionString))
            await using (OleDbCommand command = new OleDbCommand(query, connection))
            {
                foreach (string fieldValue in newDetails.Values)
                {
                    command.Parameters.AddWithValue("", fieldValue);
                }
                command.Parameters.AddWithValue("", oldEmail);
            
                await command.ExecuteNonQueryAsync();
            }
            outcome = FormulateOutcome(true, "Details updated successfully.");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error updating details: " + e);
            outcome = FormulateOutcome(false, "Error updating details.");
        }
        
        return outcome;
    }

    // Finds all users in vicinity of incident location
    // Currently returns first user for testing purposes
    // Parameters:
    // - Location locationDetails: incident location coordinates
    // Returns:
    // Task<List<string>>: list of user emails in vicinity
    public async Task<List<string>> FindUsersInVicinity(Location locationDetails)
    {
        // TODO: optimise
        const string query = "SELECT email_address FROM [User]";
        List<string> emailsInRange = new List<string>();

        await using (OleDbConnection connection = new OleDbConnection(connectionString))
        await using (OleDbCommand command = new OleDbCommand(query, connection))
        await using (OleDbDataReader reader = (OleDbDataReader)await command.ExecuteReaderAsync())
        {
            if (await reader.ReadAsync())
            {
                string email = reader.GetString(0);
                emailsInRange.Add(email);
            }
        }
        return emailsInRange;
    }

    public async Task<Dictionary<string, string>> GetUserLocation(string email)
    {
        const string query = "SELECT latitude, longitude FROM [User] WHERE email_address = ?";
        Dictionary<string, string> location = new Dictionary<string, string>();
        Dictionary<string, string> outcome;

        try
        {
            await using (OleDbConnection connection = new OleDbConnection(connectionString))
            await using (OleDbCommand command = new OleDbCommand(query, connection))
            {
                command.Parameters.AddWithValue("@email_address", email);
                await using (OleDbDataReader reader = (OleDbDataReader)await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        string latitude = reader.GetDouble(0).ToString(CultureInfo.InvariantCulture);
                        string longitude = reader.GetDouble(1).ToString(CultureInfo.InvariantCulture);
                        location.Add("Latitude", latitude);
                        location.Add("Longitude", longitude);
                        outcome = FormulateOutcome(true, "Location retrieved.", location);
                    }
                    else
                    {
                        outcome = FormulateOutcome(false, "Email not found.");
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error retrieving location: " + e);
            outcome = FormulateOutcome(false, "Error retrieving location.");
        }
        
        return outcome;
    }

    private Dictionary<string, string> FormulateOutcome(bool success, string outcomeMessage, Dictionary<string, string>? data = null)
    {
        Dictionary<string, string> outcome = new Dictionary<string, string>();
        outcome.Add("Success", success.ToString());
        outcome.Add("Outcome", outcomeMessage);

        if (data != null)
        {
            foreach (KeyValuePair<string, string> entry in data)
            {
                outcome.Add(entry.Key, entry.Value);
            }
        }
        
        return outcome;
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