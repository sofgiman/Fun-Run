using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

 
#region  Sign Up Network Messages


// Sign up request message from client to server
// username - user's username
// password - user's password
public struct SignUpMessage: NetworkMessage
{
    public string username;
    public string password;
    public SignUpMessage(string username, string password){
        this.username = username;
        this.password = password;
    }
}

// Message from server to a client to tell that sign up proccess worked
public struct SignUpSuccessMessage: NetworkMessage {
}

// Message from server to a client to tell that sign up proccess failed
public struct SignUpFailMessage: NetworkMessage{
}
#endregion

#region  Sign In Network Messages

// Sign in request message from client to server
// username - user's username
// password - user's password
public struct SignInMessage: NetworkMessage
{
    public string username;
    public string password;
    public SignInMessage(string username, string password){
        this.username = username;
        this.password = password;
    }
}

// Message from server to a client to tell that sign in proccess worked
public struct SignInSuccessMessage: NetworkMessage {
}

// Message from server to a client to tell that sign in proccess failed
public struct SignInFailMessage: NetworkMessage{
}
#endregion

//public struct SpawnPlayerMessage:NetworkMessage{
//}

#region  Adding Players To Game

// A message from client to server requesting to play the game
// skinName - the skin of the player the client wants to have
// username - client's username 
public struct PlayRequestMessage:NetworkMessage{
    public string skinName;
    public string username;
}

// A message from server to client telling a client he can't play (game full or game started)
// message - why the player can't join the game
public struct CanNotPlayMessage:NetworkMessage{
    public string message;
}
#endregion 


#region  Starting the game

// Message from server to clients in the game to start the countdown (3 2 1 GO) 
public struct StartCountdownMessege:NetworkMessage{
    
}

// Message from server to clients in the game to enable the players' movement -> starting the game
public struct StartGameMessage:NetworkMessage{

}
#endregion


// The message that clients (on the game) gets when the game ends 
// This message allows to show the podium on each client
// count - number of players on game
// playersNamesStats - the usernames of the users (ordered by their finish line)
// playersSkinsNamesStats - the skins of the players (ordered by their finish line)
public struct GameFinishedMessage:NetworkMessage{
    public int count;
    public string[] playersNamesStats;
    public string[] playersSkinsNamesStats;
    public GameFinishedMessage(string[] playersNamesStats, string[] playersSkinsNamesStats, int count){
        this.playersNamesStats = playersNamesStats;
        this.playersSkinsNamesStats = playersSkinsNamesStats;
        this.count = count;
    }
}

public class Messeagestest : MonoBehaviour
{
   
}
