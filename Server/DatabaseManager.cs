using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Text;
using System.Threading.Tasks;

namespace EMS_NEA;

internal class DatabaseManager
{
    private static OleDbConnection connection;

    public DatabaseManager()
    {
        string passkey = System.Environment.GetEnvironmentVariable("DBPasskey");
        string connectionString =
            $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=EMS_NEA_Database.accdb;Jet OLEDB:Database Password={passkey};";
        connection = new OleDbConnection(connectionString);
        connection.Open();
        Console.WriteLine("Connected to database." + connection.State);
    }


    public void InsertEvent(Event eventToInsert)
    {
        const string query =
            "INSERT INTO Incident (incident_type, description, address, latitude, longitude, status, start_time, resolved_time) " +
            "VALUES (?, ?, ?, ?, ?, ?, ?, ?)";

        using (OleDbCommand command = new OleDbCommand(query, connection))
        {
            command.Parameters.AddWithValue("@incident_type", eventToInsert.type);
            command.Parameters.AddWithValue("@description", eventToInsert.description);
            command.Parameters.AddWithValue("@address", eventToInsert.location.address);
            command.Parameters.AddWithValue("@latitude", eventToInsert.location.latitude);
            command.Parameters.AddWithValue("@longitude", eventToInsert.location.longitude);
            command.Parameters.AddWithValue("@status", eventToInsert.status);
            command.Parameters.AddWithValue("@start_time", eventToInsert.startTimestamp);
            command.Parameters.AddWithValue("@resolved_time", eventToInsert.resolvedTimestamp);

            Console.Write(command.ExecuteNonQuery());
            Console.WriteLine("Inserted event into database.");
            connection.Close();
        }
    }

    public void InsertUser(User userToInsert)
    {
        const string query =
            "INSERT INTO [User] (first_name, last_name, phone_number, email_address, password_hash, latitude, longitude, is_receiving) " +
            "VALUES (?, ?, ?, ?, ?, ?, ?, ?)";

        using (OleDbCommand command = new OleDbCommand(query, connection))
        {
            Console.WriteLine(userToInsert.password);
            Console.WriteLine(userToInsert.email);
            Console.WriteLine(userToInsert.phoneNumber);
            Console.WriteLine(userToInsert.firstName);
            Console.WriteLine(userToInsert.lastName);
            
            command.Parameters.AddWithValue("@first_name", userToInsert.firstName);
            command.Parameters.AddWithValue("@last_name", userToInsert.lastName);
            command.Parameters.AddWithValue("@phone_number", userToInsert.phoneNumber);
            command.Parameters.AddWithValue("@email_address", userToInsert.email);
            command.Parameters.AddWithValue("@password_hash", userToInsert.password);
            command.Parameters.AddWithValue("@latitude", 0);
            command.Parameters.AddWithValue("@longitude", 0);
            command.Parameters.AddWithValue("@is_receiving", false);
            
            command.ExecuteNonQuery();
            Console.WriteLine("Inserted user into database.");
            connection.Close();
        }
    }

    public string CheckLoginDetails(string email, string passwordHash)
    {
        const string query = 
            "SELECT password_hash FROM [User] WHERE email_address = ?";
        
        using (OleDbCommand command = new OleDbCommand(query, connection))
        {
            command.Parameters.AddWithValue("@email_address", email);
            
            string? retrievedHash =  (string?) command.ExecuteScalar();
            
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

    public void ChangeAlertChoice(bool alertChoice, string email)
    {
        Console.WriteLine("flag1");
        const string query = 
            "UPDATE [User] SET is_receiving = ?" +
            "WHERE email_address = ?";

        using (OleDbCommand command = new OleDbCommand(query, connection))
        {
            command.Parameters.AddWithValue("@is_receiving", alertChoice);
            command.Parameters.AddWithValue("@email_address", email);
            command.ExecuteNonQuery();
            Console.WriteLine("Updated alert choice.");
        }
    }
    
    public bool GetReceivingStatus(string email)
    {
        const string query = "SELECT is_receiving FROM [User] WHERE email_address = ?";
        
        using (OleDbCommand command = new OleDbCommand(query, connection))
        {
            command.Parameters.AddWithValue("@email_address", email);
            bool? result = (bool?) command.ExecuteScalar();
            return result ?? false;
        }
    }

    public Dictionary<string, string> RetrieveUserDetails(string email)
    {
        const string query = 
            "SELECT first_name, last_name, phone_number FROM [User]" +
            "WHERE email_address = ?";
        
        Dictionary<string, string> userDetails = new Dictionary<string, string>();

        using (OleDbCommand command = new OleDbCommand(query, connection))
        {
            command.Parameters.AddWithValue("@email_address", email);
            using (OleDbDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
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
    
    public bool UpdateUserDetails(Dictionary<string, string> newDetails, string oldEmail)
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

        using (OleDbCommand command = new OleDbCommand(query, connection))
        {
            foreach (string fieldValue in newDetails.Values)
            {
                command.Parameters.AddWithValue("", fieldValue);
            }
            command.Parameters.AddWithValue("", oldEmail);
            
            command.ExecuteNonQuery();
            Console.WriteLine("Updated user details.");
        }
        return true;
    }

    public async Task<List<string>> FindUsersInVicinity(Location locationDetails)
    {
        const string query = "SELECT email_address, latitude, longitude FROM [User]";
        double eventLatitude = locationDetails.latitude;
        double eventLongitude = locationDetails.longitude;
        bool inRange;
        List<string> emailsInRange = new List<string>();
        
        await using (OleDbCommand command = new OleDbCommand(query, connection))
        {
            connection.Open();
            
            await using (OleDbDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string email = reader.GetString(0);
                    double userLatitude = reader.GetDouble(1);
                    double userLongitude = reader.GetDouble(2);
                    Console.WriteLine($"Email: {email}, Latitude: {userLatitude}, Longitude: {userLongitude}");

                    inRange = await Server.CheckDistances(eventLatitude, eventLongitude, userLatitude, userLongitude);

                    if (inRange || true)
                    {
                        Console.WriteLine($"Email {email} is in range.");
                        emailsInRange.Add(email);
                    }
                    break;
                }
            }
        }
        return emailsInRange;
    }

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