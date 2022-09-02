using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// A class that runs on server for managing the game
public class GameManagerServer : NetworkBehaviour
{
    private const int MAX_PLAYERS_IN_GAME = 4;
    private const int MIN_PLAYERS_IN_GAME = 2;
    private const int TIME_TO_WAIT_FOR_MORE_PLAYERS = 6;  // the time to wait after the minimum players count achived
    
    private Dictionary<string,string> playerNameToSkinName; // dict to get the skin name of each player
    // players names that will be ordered by their place (1st place will be at the 0 index)   
    private string[] playersNamesOrdered;
    // players skins names that will be ordered by their place (1st place skin will be at the 0 index)
    private string[] playersSkinsNamesOrdered; 
    private List<NetworkConnection> connsInGame;  // list of net connections that are in the game
    private int orderedPlayersFinishIndex;  // an index to know where to place player's data on the array above
    private int playersInGame;  // the number of players in the game 
   
    [SerializeField] private MyNetworkManager myNetworkManager;
     
    private bool gameStarted;  // flag to know if game started
    private bool gameFinished;  // flag to know if game finished
    private bool checkedForMorePlayers;  // flag to know if after minimum players joined the search for more players started
    float[] startPositionsX = {-1f, -0.5f, 0f, 0.5f};  // different positions for each player (so they won't spawn on eachother)
    int spawnIndexPosition;  

    // Start is called before the first frame update
    void Start()
    { 
        // initializing all variables

        playerNameToSkinName = new Dictionary<string, string>(MAX_PLAYERS_IN_GAME);
        checkedForMorePlayers = false;
        playersNamesOrdered = new string[MAX_PLAYERS_IN_GAME];
        playersSkinsNamesOrdered=new string[MAX_PLAYERS_IN_GAME];
        connsInGame = new List<NetworkConnection>(MAX_PLAYERS_IN_GAME); 
        orderedPlayersFinishIndex = 0;
        playersInGame =0;
        spawnIndexPosition= 0;
        gameStarted = false;
        gameFinished = false;

        NetworkServer.RegisterHandler<PlayRequestMessage>(OnCheckCanPlay);  
    }

    void Update(){
        if(gameFinished){   // In case game finished 
            // Finish game and reset variables 
            gameFinished= false;
            FinishGame();
            ResetGameManager();            
        }
    }
    // A func to that take a network meassage and send the message to all client in the game
    void SendToAllPlayers<T>(T networkMessage)
    where T:struct, NetworkMessage{
        foreach(NetworkConnection conn in connsInGame){
            conn.Send<T>(networkMessage); 
        }
    }

    #region Game Start Management
    
    // A callback from a client to check if he can play
    // recive net connection of client and the play request message
    private void OnCheckCanPlay(NetworkConnection conn,  PlayRequestMessage prm){
        if (playersInGame >= MAX_PLAYERS_IN_GAME || gameStarted){  // if game started or there's max players
            NetworkServer.SetClientNotReady(conn);  // not allowing client to play
            CanNotPlayMessage cnpm = new CanNotPlayMessage();
            if(gameStarted)
                cnpm.message = "Game has started";
            else
                cnpm.message = "There are maximum players in the game";
            conn.Send<CanNotPlayMessage>(cnpm);  // Sending to client that he can't play
            return;
        }
        else{  // user can play
            connsInGame.Add(conn);  // adding his conn to conn list
            connsInGame[playersInGame] = conn;  // adding his conn to conns list
            playersInGame ++;
            string username = prm.username;
            string skinName = prm.skinName;
            SpawnPlayer(conn, username, skinName);  // spawning the player
            playerNameToSkinName.Add(username,skinName);  // adding user to dict so we'll know user skin (usen in podium creation)
            if (playersInGame >=MIN_PLAYERS_IN_GAME && !checkedForMorePlayers){
                checkedForMorePlayers = true; 
                StartCoroutine(StartGame());  // starting game in 6 second while searching for more players

            }
        }
    }

    // A function to spawn a player on server and client of the game
    // conn - connection to the client that own the player 
    // username - client username
    // skinName - client skinName of player
    private void SpawnPlayer(NetworkConnection conn ,string username, string skinName){
        // instantiating player prefab on server
        GameObject player = Instantiate(myNetworkManager.playerPrefab, new Vector3(startPositionsX[spawnIndexPosition],0,0),Quaternion.identity);
        spawnIndexPosition ++;
        GameManagerClient gmc = player.GetComponent<GameManagerClient>();
        gmc.username = username;  // setting username on the player
        ChangeSkin cs =player.GetComponent<ChangeSkin>();
        cs.skinName = skinName;  // setting skin name on the player
        NetworkServer.AddPlayerForConnection(conn, player);  // adding the player to the game world (means that it will be spawned on all clients and on the server)
    }

    // A coroutine to start the game while searching for more players
    IEnumerator StartGame(){
        yield return new WaitForSeconds(TIME_TO_WAIT_FOR_MORE_PLAYERS - 1);  // waiting for more players to come
        gameStarted = true;
        yield return new WaitForSeconds(1);
        // sending message to users in the game to start the countdown (3 2 1 GO)
        SendToAllPlayers<StartCountdownMessege>(new StartCountdownMessege());   
        yield return new WaitForSeconds(3);
        SendToAllPlayers<StartGameMessage>(new StartGameMessage());  // Sending the start game message( allowing players to move)
        yield break;
    }
    #endregion

    #region Game Ending Management
    
    // A function called from a player (on server) when he reaches the finish line
    // gets username to know who is the player 
    public void AddPlayerToFinishedPlayers(string username){
        // placing data of player on an ordered arrays
        // orderedPlayersFinishIndex counts the place that the player finishes (0 index means 1st place, 1 means 2nd ...)
        playersNamesOrdered[orderedPlayersFinishIndex] = username;  
        playersSkinsNamesOrdered[orderedPlayersFinishIndex] = playerNameToSkinName[username];
        orderedPlayersFinishIndex++; 
        if (orderedPlayersFinishIndex == playersInGame){  // if all players have finishes then finish game
            gameFinished = true;
        } 
    }

    // Func to finish the game
    private void FinishGame(){
        // sending number of player, players' usernames and their skins ordered by their finish place  
        GameFinishedMessage gfm = new GameFinishedMessage(playersNamesOrdered, playersSkinsNamesOrdered, playersInGame);        
        foreach(NetworkConnection conn in connsInGame){
                conn.Send<GameFinishedMessage>(gfm);   
                NetworkServer.SetClientNotReady(conn);  // removing client from the game world
        }

        // destroying all players in server
        GameObject[] players =  GameObject.FindGameObjectsWithTag("Player"); 
        foreach(GameObject player in players){
            NetworkServer.Destroy(player);
        } 
       
    }

    // Func to reset all of the game manager variables
    private void ResetGameManager(){
        orderedPlayersFinishIndex = 0;
        playersInGame =0;
        spawnIndexPosition =0;
        gameStarted = false;
        gameFinished = false;
        checkedForMorePlayers = false;
        for(int i =0; i<playersNamesOrdered.Length; i++){
            playersNamesOrdered[i] = "";
            playersSkinsNamesOrdered[i] = "";
        }
        playerNameToSkinName.Clear();
        connsInGame.Clear();
    }
    #endregion

}
