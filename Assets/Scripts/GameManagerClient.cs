using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// A class to manage client's game related actions  
public class GameManagerClient : NetworkBehaviour
{
    public string username ="" ;  // username of the player - only initialized in server
    [SerializeField] private Player playerMovmentScript; // player movment script( to ebanle movment)

    private GameManagerServer gms;
    void Start()
    {
        if(isLocalPlayer)
            NetworkClient.RegisterHandler<StartGameMessage>(OnStartGame); // register handler on the player client for the start of the game
        if(isServer)
            gms = GameObject.Find("GameManagerServer").GetComponent<GameManagerServer>();  // having refrence to the server game manager (only on player's server)

    }

    // Callback from server to client to enable the player movement
    // gets StartGameMessage from server which tells that game started and the player should move
    public void OnStartGame(StartGameMessage scdm){
        playerMovmentScript.SetGameStarted(true);  // enable player to move 
    }
    
    // This function is built in Unity and called when the player collides with an object
    // If the object that collided is the finish object then the server is informed that the player has reached the finished line
    private void OnTriggerEnter2D(Collider2D other)
    {      
        if(isServer) // if the player is on the server
        {   
            if(other.gameObject.layer == LayerMask.NameToLayer("Finish")){  // if the object collided is the finish line
                gms.AddPlayerToFinishedPlayers(username);  // telling game manger (server) the player has reached the end
            }   
        }
		
    }
}
