using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Mono.Data.Sqlite;

//  Class to manage authentication on server 
public class MyAuthentication : MonoBehaviour
{
    SqliteConnection m_dbConnection;  // the connection for sql
   
    // Funct to connect to the sql database
    public void ConnectToSql(){

        // register handlres for sign up and sign in requests from clients
        NetworkServer.RegisterHandler<SignUpMessage>(OnSignUp);
        NetworkServer.RegisterHandler<SignInMessage>(OnSignIn);
      
        m_dbConnection = new SqliteConnection("Data Source=MyDatabase.sqlite;Version=3;");
        m_dbConnection.Open();

        print("connected to sql");            
    }

    // Called on server on sign up request from clients
    private void OnSignUp(NetworkConnection conn, SignUpMessage sum){
        string username = sum.username;
        string password = sum.password;
        string usernameInDbString = "SELECT * FROM 'users' WHERE Username = '"+username+"'";  // checking if username is taken
        SqliteCommand usernameInDbCommand = new SqliteCommand(usernameInDbString, m_dbConnection);
        System.Object reader = usernameInDbCommand.ExecuteScalar();
        if (reader==null){  // if username not taken
            string sql = "INSERT INTO users (Username, Password) VALUES ('"+username+"', '"+password+"')";  // create user in db
            SqliteCommand command = new SqliteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
            
            conn.Send<SignUpSuccessMessage>(new SignUpSuccessMessage());  // sending to client that user created
        }
        else
            conn.Send<SignUpFailMessage>(new SignUpFailMessage());  // sending to client that user wasn't created     

    }

    // Called on server on sign in request from clients
    private void OnSignIn(NetworkConnection conn, SignInMessage sim){
        string username = sim.username;
        string password = sim.password;
        string userInDbString = "SELECT * FROM 'users' WHERE Username = '"+username+"'" +" AND password = '"+password+"'" ;  // checking if user is in the db
        SqliteCommand usernameInDbCommand = new SqliteCommand(userInDbString, m_dbConnection);
        System.Object reader = usernameInDbCommand.ExecuteScalar();
        if (reader==null){  // if reader is null then the user isn't in db (not created)
            conn.Send<SignInFailMessage>(new SignInFailMessage());  // sending fail message to client
        }
        else
            conn.Send<SignInSuccessMessage>(new SignInSuccessMessage());  // sending succes message to client
    } 
}
