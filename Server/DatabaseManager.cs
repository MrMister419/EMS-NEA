using System;
using System.Data.OleDb;

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
            "INSERT INTO Incident (IncidentID, incident_type, description, address, latitude, longitude, status, start_time, resolved_time) " +
            "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)";

        using (OleDbCommand command = new OleDbCommand(query, connection))
        {
            command.Parameters.AddWithValue("@IncidentID", eventToInsert.eventID);
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
}