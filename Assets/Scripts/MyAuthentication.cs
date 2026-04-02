using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Mono.Data.Sqlite;
using System;
using System.IO;


//  Class to manage authentication on server 
public class MyAuthentication : MonoBehaviour {
    SqliteConnection m_dbConnection;  // the connection for sql

    // Funct to connect to the sql database
    public void ConnectToSql() {

        // register handlres for sign up and sign in requests from clients
        NetworkServer.RegisterHandler<SignUpMessage>(OnSignUp);
        NetworkServer.RegisterHandler<SignInMessage>(OnSignIn);

        // Load .env variables first
        EnvLoader.Load();

        // Get DATA_PATH env var or fallback to persistent data path
        string dataPath = Environment.GetEnvironmentVariable("FUN_RUN_DATA_PATH")
                          ?? Application.persistentDataPath;

        // Build database path
        string dbFile = Path.Combine(dataPath, "MyDatabase.sqlite");

        m_dbConnection = new SqliteConnection($"Data Source={dbFile};Version=3;");
        m_dbConnection.Open();

        CreateUsersTable();

        Debug.Log($"Connected to SQLite DB successfully at: {dbFile}");
    }

    private void CreateUsersTable() {
        using (var cmd = m_dbConnection.CreateCommand()) {
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS users (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT UNIQUE,
                    Password TEXT
                );
            ";
            cmd.ExecuteNonQuery();
            Debug.Log("Users table ensured.");
        }
    }

    // Called on server on sign up request from clients
    private void OnSignUp(NetworkConnection conn, SignUpMessage sum) {
        string sql = "INSERT INTO users (Username, Password) VALUES (@username, @password)";  // create user in db using parameterized query

        try {
            // using ensures proper memory release for the command
            using (SqliteCommand command = new SqliteCommand(sql, m_dbConnection)) {
                // protection against sql injection
                command.Parameters.AddWithValue("@username", sum.username);
                command.Parameters.AddWithValue("@password", sum.password);

                command.ExecuteNonQuery();
            }
            conn.Send(new SignUpSuccessMessage());  // sending to client that user created
        } catch (SqliteException ex) {
            // will fail and reach here if username is taken because of the UNIQUE keyword
            conn.Send(new SignUpFailMessage());  // sending to client that user wasn't created     
        }
    }

    // Called on server on sign in request from clients
    private void OnSignIn(NetworkConnection conn, SignInMessage sim) {

        string sql = "SELECT ID FROM users WHERE Username = @username AND Password = @password";  // checking if user is in the db

        using (SqliteCommand command = new SqliteCommand(sql, m_dbConnection)) {
            // protection against sql injection
            command.Parameters.AddWithValue("@username", sim.username);
            command.Parameters.AddWithValue("@password", sim.password);

            System.Object reader = command.ExecuteScalar();

            if (reader == null)  // if reader is null then the user isn't in db (not created)
            {
                conn.Send(new SignInFailMessage());  // sending fail message to client
            } else {
                conn.Send(new SignInSuccessMessage());  // sending succes message to client
            }
        }
    }

    private void OnApplicationQuit() {
        if (m_dbConnection != null) {
            m_dbConnection.Close();
            m_dbConnection.Dispose();
            m_dbConnection = null;
            Debug.Log("Database connection closed.");
        }
    }
}