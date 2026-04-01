using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// A class that runs on server for managing the game
public class GameManagerServer : NetworkBehaviour {
    private const int MAX_PLAYERS_IN_GAME = 4;
    private const int MIN_PLAYERS_IN_GAME = 2;
    private const int TIME_TO_WAIT_FOR_MORE_PLAYERS = 6;  // the time to wait after the minimum players count achived
    private const float MAX_GAME_TIME = 90f;

    private Dictionary<string, string> playerNameToSkinName; // dict to get the skin name of each player
    // players names that will be ordered by their place (1st place will be at the 0 index)   
    private string[] playersNamesOrdered;
    // players skins names that will be ordered by their place (1st place skin will be at the 0 index)
    private string[] playersSkinsNamesOrdered;
    private List<NetworkConnection> connsInGame;  // list of net connections that are in the game
    private int orderedPlayersFinishIndex;  // an index to know where to place player's data on the array above
    private int disconnectedPlayersMidGame = 0; // counter for DNF players (disconnected mid game)
    private int playersInGame;  // the number of players in the game 
    private Coroutine startGameCoroutine; // Holds the reference to the countdown coroutine so we can stop it if needed
    [SerializeField] private MyNetworkManager myNetworkManager;

    private bool gameStarted;  // flag to know if game started
    private bool gameFinished;  // flag to know if game finished
    private bool checkedForMorePlayers;  // flag to know if after minimum players joined the search for more players started
    private float gameTimer; // Total time in seconds for a game
    private bool timerRunning = false; // Flag to indicate if the timer is currently active
    float[] startPositionsX = { -1f, -0.5f, 0f, 0.5f };  // different positions for each player (so they won't spawn on eachother)
    int spawnIndexPosition;


    // Start is called before the first frame update
    void Start() {
        // initializing all variables

        playerNameToSkinName = new Dictionary<string, string>(MAX_PLAYERS_IN_GAME);
        checkedForMorePlayers = false;
        playersNamesOrdered = new string[MAX_PLAYERS_IN_GAME];
        playersSkinsNamesOrdered = new string[MAX_PLAYERS_IN_GAME];
        connsInGame = new List<NetworkConnection>(MAX_PLAYERS_IN_GAME);
        orderedPlayersFinishIndex = 0;
        playersInGame = 0;
        spawnIndexPosition = 0;
        gameStarted = false;
        gameFinished = false;
        gameTimer = MAX_GAME_TIME;

        NetworkServer.RegisterHandler<PlayRequestMessage>(OnCheckCanPlay);
    }

    void Update() {
        if (gameFinished) {   // In case game finished 
            // Finish game and reset variables 
            gameFinished = false;
            FinishGame();
            ResetGameManager();
            return;
        }

        if (!isServer || !timerRunning)
            return;

        gameTimer -= Time.deltaTime;
        if (gameTimer <= 0) {
            print("Force finishing game due to exceeded time limit");
            ForceFinishGame();
        }
    }
    // A func to that take a network meassage and send the message to all client in the game
    void SendToAllPlayers<T>(T networkMessage)
    where T : struct, NetworkMessage {
        foreach (NetworkConnection conn in connsInGame) {
            conn.Send<T>(networkMessage);
        }
    }

    #region Game Start Management

    // A callback from a client to check if he can play
    // recive net connection of client and the play request message
    private void OnCheckCanPlay(NetworkConnection conn, PlayRequestMessage prm) {

        string username = prm.username;

        // Prevent joining if user is already in the game
        if (playerNameToSkinName.ContainsKey(username)) {
            NetworkServer.SetClientNotReady(conn);

            CanNotPlayMessage cnpm = new CanNotPlayMessage();
            cnpm.message = "User is already in a game";
            conn.Send<CanNotPlayMessage>(cnpm);

            return;
        }

        if (playersInGame >= MAX_PLAYERS_IN_GAME || gameStarted) {  // if game started or there's max players
            NetworkServer.SetClientNotReady(conn);  // not allowing client to play
            CanNotPlayMessage cnpm = new CanNotPlayMessage();
            if (gameStarted)
                cnpm.message = "Game has started";
            else
                cnpm.message = "There are maximum players in the game";
            conn.Send<CanNotPlayMessage>(cnpm);  // Sending to client that he can't play
            return;
        } else {  // user can play
            connsInGame.Add(conn);  // adding his conn to conn list
            connsInGame[playersInGame] = conn;  // adding his conn to conns list
            playersInGame++;
            string skinName = prm.skinName;
            SpawnPlayer(conn, username, skinName);  // spawning the player
            playerNameToSkinName.Add(username, skinName);  // adding user to dict so we'll know user skin (usen in podium creation)
            if (playersInGame >= MIN_PLAYERS_IN_GAME && !checkedForMorePlayers) {
                checkedForMorePlayers = true;
                startGameCoroutine = StartCoroutine(StartGame());  // starting game in 6 second while searching for more players

            }
        }
    }

    // A function to spawn a player on server and client of the game
    // conn - connection to the client that own the player 
    // username - client username
    // skinName - client skinName of player
    private void SpawnPlayer(NetworkConnection conn, string username, string skinName) {
        // instantiating player prefab on server
        GameObject player = Instantiate(myNetworkManager.playerPrefab, new Vector3(startPositionsX[spawnIndexPosition], 0, 0), Quaternion.identity);
        spawnIndexPosition++;
        GameManagerClient gmc = player.GetComponent<GameManagerClient>();
        gmc.username = username;  // setting username on the player
        ChangeSkin cs = player.GetComponent<ChangeSkin>();
        cs.skinName = skinName;  // setting skin name on the player
        NetworkServer.AddPlayerForConnection(conn, player);  // adding the player to the game world (means that it will be spawned on all clients and on the server)
    }

    // A coroutine to start the game while searching for more players
    IEnumerator StartGame() {
        yield return new WaitForSeconds(TIME_TO_WAIT_FOR_MORE_PLAYERS - 1);  // waiting for more players to come
        gameStarted = true;
        yield return new WaitForSeconds(1);
        // sending message to users in the game to start the countdown (3 2 1 GO)
        SendToAllPlayers<StartCountdownMessege>(new StartCountdownMessege());
        yield return new WaitForSeconds(3);
        SendToAllPlayers<StartGameMessage>(new StartGameMessage());  // Sending the start game message( allowing players to move)
        timerRunning = true; // Start the global game timer when players can move
        yield break;
    }
    #endregion

    #region Game Ending Management

    // A function called from a player (on server) when he reaches the finish line
    // gets username to know who is the player 
    public void AddPlayerToFinishedPlayers(string username) {
        // placing data of player on an ordered arrays
        // orderedPlayersFinishIndex counts the place that the player finishes (0 index means 1st place, 1 means 2nd ...)
        playersNamesOrdered[orderedPlayersFinishIndex] = username;
        playersSkinsNamesOrdered[orderedPlayersFinishIndex] = playerNameToSkinName[username];
        orderedPlayersFinishIndex++;
        // Check if active finishers + mid game disconnected players = total players
        if (orderedPlayersFinishIndex + disconnectedPlayersMidGame == playersInGame) {
            gameFinished = true;
        }
    }

    // Func to finish the game
    private void FinishGame() {
        // sending number of player, players' usernames and their skins ordered by their finish place  
        GameFinishedMessage gfm = new GameFinishedMessage(playersNamesOrdered, playersSkinsNamesOrdered, playersInGame);
        foreach (NetworkConnection conn in connsInGame) {
            conn.Send<GameFinishedMessage>(gfm);
            NetworkServer.SetClientNotReady(conn);  // removing client from the game world
        }

        // destroying all players in server
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            NetworkServer.Destroy(player);
        }

    }

    // A function to handle the scenario where the game time runs out 
    // before all players reach the finish line. It assigns the remaining 
    // players to the end of the arrays as DNF (Did Not Finish).
    private void ForceFinishGame() {
        timerRunning = false;

        // Iterate through all players who originally joined the game
        foreach (var entry in playerNameToSkinName) {
            bool alreadyFinished = false;

            // Check if this player is already in the finished players array
            for (int i = 0; i < orderedPlayersFinishIndex; i++) {
                if (playersNamesOrdered[i] == entry.Key) {
                    alreadyFinished = true;
                    break;
                }
            }

            // If the player hasn't finished, append them to the arrays
            if (!alreadyFinished) {
                playersNamesOrdered[orderedPlayersFinishIndex] = entry.Key;
                playersSkinsNamesOrdered[orderedPlayersFinishIndex] = entry.Value;
                orderedPlayersFinishIndex++;
            }
        }

        // Now that the arrays are full, trigger the gameFinished flag!
        // The original logic in Update() will catch this and finish the game.
        gameFinished = true;
    }

    // Func to reset all of the game manager variables
    private void ResetGameManager() {
        orderedPlayersFinishIndex = 0;
        playersInGame = 0;
        spawnIndexPosition = 0;
        gameStarted = false;
        gameFinished = false;
        checkedForMorePlayers = false;
        for (int i = 0; i < playersNamesOrdered.Length; i++) {
            playersNamesOrdered[i] = "";
            playersSkinsNamesOrdered[i] = "";
        }
        playerNameToSkinName.Clear();
        connsInGame.Clear();
        gameTimer = MAX_GAME_TIME; // Reset the timer for the next game
        timerRunning = false;
        disconnectedPlayersMidGame = 0;
    }

    private void ResetServerToIdle() {
        spawnIndexPosition = 0;
        playersInGame = 0;
        checkedForMorePlayers = false;
        connsInGame.Clear();
        playerNameToSkinName.Clear();
    }
    #endregion

    #region Disconnection Management
    public void HandlePlayerDisconnect(NetworkConnection conn) {
        // Validation: Only handle connections already registered in the game logic
        if (!connsInGame.Contains(conn)) return;

        string disconnectedUsername = "";
        if (conn.identity != null && conn.identity.TryGetComponent<GameManagerClient>(out var client)) {
            disconnectedUsername = client.username;
        }

        // Lobby state: Handling players leaving before the race starts
        if (!gameStarted) {
            connsInGame.Remove(conn);
            playersInGame = Mathf.Max(0, playersInGame - 1);

            // Abort the pre-game countdown if a player leaves and we drop below the required minimum
            if (checkedForMorePlayers && playersInGame < MIN_PLAYERS_IN_GAME) {
                if (startGameCoroutine != null) {
                    StopCoroutine(startGameCoroutine);
                    startGameCoroutine = null;
                }
                checkedForMorePlayers = false; // Reset flag so it can start again later
                Debug.Log($"[Waiting Room] Dropped below minimum players ({playersInGame}/{MIN_PLAYERS_IN_GAME})");
            }

            // Free up the skin
            if (!string.IsNullOrEmpty(disconnectedUsername)) {
                playerNameToSkinName.Remove(disconnectedUsername);
            }

            // Re-align remaining players to fill the gap and prevent spawn overlaps
            for (int i = 0; i < connsInGame.Count; i++) {
                if (connsInGame[i].identity != null) {
                    Vector3 newPos = new Vector3(startPositionsX[i], 0, 0);
                    GameManagerClient clientScript = connsInGame[i].identity.GetComponent<GameManagerClient>();
                    if (clientScript != null) {
                        clientScript.TargetUpdatePosition(connsInGame[i], newPos);
                    }
                }
            }

            spawnIndexPosition = playersInGame;
            Debug.Log($"[Waiting Room] {disconnectedUsername} left. Syncing {playersInGame} remaining players.");

            if (playersInGame == 0) ResetServerToIdle();
        }
        // Active game state: Handling mid-race disconnections
        else if (!gameFinished) {
            Debug.Log($"[Active Game] {disconnectedUsername} disconnected mid game");

            if (!string.IsNullOrEmpty(disconnectedUsername)) {
                // Calculate the last available spot (e.g., if 4 players, first DNF gets index 3, next gets 2)
                int dnfIndex = playersInGame - 1 - disconnectedPlayersMidGame;

                // Place them at the end of the array
                playersNamesOrdered[dnfIndex] = disconnectedUsername;
                playersSkinsNamesOrdered[dnfIndex] = playerNameToSkinName[disconnectedUsername];

                // Increment the DNF counter
                disconnectedPlayersMidGame++;

                // Check if the game should end NOW (Finished + DNF == Total Players)
                if (orderedPlayersFinishIndex + disconnectedPlayersMidGame == playersInGame) {
                    gameFinished = true;
                }
            }

            connsInGame.Remove(conn);
        }
    }
    #endregion
}
