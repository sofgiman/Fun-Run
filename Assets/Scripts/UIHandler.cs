using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;


// The main class for the client, handling the client's UI, all scenes and network requests
public class UIHandler : MonoBehaviour
{   
    [SerializeField] private SkinHandler skinHandler;
    [SerializeField] private PodiumUIhandler podiumHandler;
    public string userNameText;
    string passwordText;

    // All differents canvas
    [SerializeField] private GameObject lobbyCanvas;
    [SerializeField] private GameObject signUpCanvas;
    [SerializeField] private GameObject signInCanvas;
    [SerializeField] private GameObject searchingForPlayersCanvas;
    [SerializeField] private GameObject countdownCanvas;
    [SerializeField] private GameObject tutorialCanvas;
    [SerializeField] private GameObject gameMapObject;
    [SerializeField] private GameObject podiumCanvas;

    // Authentication UI objects
    [SerializeField] private InputField signUpUsername;
    [SerializeField] private InputField signUpPassword;
    [SerializeField] private InputField signInUsername;
    [SerializeField] private InputField signInPassword;
    [SerializeField] private Text alertUserTextSignUp; 
    [SerializeField] private Text alertUserTextSignIn; 
   
    [SerializeField] private Text countdownText; // countdown text
    [SerializeField] private Text informUserAboutGameStatus;  // Text object to tell user if he can't join the game 

    [SerializeField] private GameObject tutorialPrefab;  // the prefab tutorial
    private GameObject tutorialGameObject;  // game object for the tutorial prefab

    // Start is called before the first frame update
    void Start(){ 
        // initialing client handlers

        // authentication handlers
        NetworkClient.RegisterHandler<SignUpSuccessMessage>(OnSignUpSuccess); 
        NetworkClient.RegisterHandler<SignUpFailMessage>(OnSignUpFail); 
        NetworkClient.RegisterHandler<SignInSuccessMessage>(OnSignInSuccess); 
        NetworkClient.RegisterHandler<SignInFailMessage>(OnSignInFail); 


        NetworkClient.RegisterHandler<CanNotPlayMessage>(OnCannotPlayMessage);  
        
        NetworkClient.RegisterHandler<StartCountdownMessege>(OnStartCountdown);  // start countdown handler
        
        NetworkClient.RegisterHandler<GameFinishedMessage>(OnToPodium);
    }

   
    #region Scene/Ui Management
    // Going to sign in canvas (default)
    public void StartScreen(){
        signInCanvas.SetActive(true);
    }
    // Going from sign in canvas to sign up canvas
    public void OnToSignUpClick(){
        ResetSignInCanvas();
        signInCanvas.SetActive(false);
        signUpCanvas.SetActive(true);
    }
    // Going from sign up canvas to sign in canvas
    public void OnToSignInClick(){
        ResetSignUpCanvas();
        signUpCanvas.SetActive(false);
        signInCanvas.SetActive(true);
    }
    // Going from sign in canvas to lobby canvas
    private void ToLobbyFromSignIn(){
        ResetSignInCanvas();
        signInCanvas.SetActive(false);
        lobbyCanvas.SetActive(true);
    }

    // Going to podium (game finish)
    private void ToPodium(){
        podiumCanvas.SetActive(true);
    }

    // To game from searching for players 
    public void ToGame(){
       searchingForPlayersCanvas.SetActive(false);  
    }

    // Showing to the user that there is searching for more players
    public void ToSearchingForPlayers(){
        lobbyCanvas.SetActive(false);
        searchingForPlayersCanvas.SetActive(true);
    }

    // Going back to lobby from searching for players canvas (called only when player can't play) 
    public void ToLobbyFromSearchingForPlayers(){
        searchingForPlayersCanvas.SetActive(false);
        lobbyCanvas.SetActive(true);
    }
    
    // To tutorial from lobby
    public void ToTutorial(){
        lobbyCanvas.SetActive(false);
        gameMapObject.SetActive(false);
        tutorialCanvas.SetActive(true);
    }

    // To lobby from tutorial
    public void ToLobbyFromTutorial(){
        tutorialCanvas.SetActive(false);
        lobbyCanvas.SetActive(true);
        gameMapObject.SetActive(true);
    }
    
    #endregion
    
    #region Sign Up Management
    // Called when client pressed the sign up button
    public void OnSignUpClick(){
        userNameText = signUpUsername.text;
        passwordText = signUpPassword.text;
        
        if(userNameText != "" && passwordText != ""){  // checking that input is valid
            SignUpMessage sum = new SignUpMessage(userNameText, passwordText);
            NetworkClient.Send<SignUpMessage>(sum);  // sending a sign up request to server 
        }
        else 
            alertUserTextSignUp.text= "Enter Valid Username And Password";
    }

    // Called on client from server when a user created (sign up was successful)
    private void OnSignUpSuccess(SignUpSuccessMessage sucm){
        alertUserTextSignUp.text = "User Added";
    }

    // Called on client from server when a user wasn't created (sign up process failed)
    private void OnSignUpFail(SignUpFailMessage sufm){
        alertUserTextSignUp.text = "Adding User Failed";
    }

    // Func to reset UI sign up objects  
    private void ResetSignUpCanvas(){
        signUpUsername.text = "";
        signUpPassword.text = "";
        alertUserTextSignUp.text = "";
    }
    #endregion

    #region Sign In Management
    public void OnSignInClick(){
        // Called when client pressed the sign in button
        userNameText = signInUsername.text;
        passwordText = signInPassword.text;
        if(userNameText != "" && passwordText != ""){    
            SignInMessage sim = new SignInMessage(userNameText, passwordText);
            NetworkClient.Send<SignInMessage>(sim);  // sending a sign up request to server 
        }
        else{
            alertUserTextSignIn.text = "Enter Valid Username And Password";
        }
    }

    // Called on client from server when the sign in process was successful
    private void OnSignInSuccess(SignInSuccessMessage sicm){
        alertUserTextSignIn.text = "Going To Lobby";
        ToLobbyFromSignIn();
    }

    // Called on client from server when the sign in process wasn't successful
    private void OnSignInFail(SignInFailMessage sifm){
        alertUserTextSignIn.text = "Incorrect Username Or Password";
    }

    // Func to reset UI sign in objects 
    private void ResetSignInCanvas(){
        signInUsername.text = "";
        signInPassword.text = "";
        alertUserTextSignIn.text = "";
    }
    #endregion

    #region Lobby Management

    // Function that handles a play click from user
    public void OnPlayClick()
    {
        ToSearchingForPlayers();  // moving to searching for players screen
    
        if(NetworkClient.localPlayer == null){  // checking that there is no local player
            NetworkClient.Ready();  // automatically join the user to the game (the server will remove the user from the game if needed)         
            PlayRequestMessage prm= new PlayRequestMessage();
            prm.skinName = skinHandler.GetChosenSkin();  // skin name of user's player
            prm.username = userNameText;  // user's username
            NetworkClient.Send<PlayRequestMessage>(prm);  
        }
    } 

    // Called on client from server when the user can't join the game (game started or is full)
    public void OnCannotPlayMessage(CanNotPlayMessage cnpm){
        ToLobbyFromSearchingForPlayers();  // going back to the lobby
        StartCoroutine(ShowCantPlayToUser(cnpm.message));  // showing to user why he can't play
    }

    // Coroutine that show the user why he can't join the game for 4 seconds
    public IEnumerator ShowCantPlayToUser(string message){
        informUserAboutGameStatus.text = message;
        yield return new WaitForSeconds(4);
        informUserAboutGameStatus.text =  "";
        yield break;
    } 

    // Func that called when the client want to go to the tutorial
    public void OnTutorialClick()
    {
        ToTutorial();
        tutorialGameObject =  Instantiate(tutorialPrefab, new Vector3(335,47, -9), Quaternion.identity);  // spawning the tutorial object
    } 
    #endregion
    
    #region Tutorial Management

    // Called when to lobby button was pressed
    public void OnLobbyFromTutorialClick()
    {
        if(Time.timeScale == 0){  // in case the screen is frozen (could happen because of tutorial)
            Time.timeScale = 1;
        }
        Destroy(tutorialGameObject);  // destroying the tutorial object
        ToLobbyFromTutorial();
       
    } 
    #endregion

    #region Game Management
    
    // Called on client from server when the countdown should start
    private void OnStartCountdown(StartCountdownMessege scdm){
        ToGame();  // moving to game
        StartCoroutine(StartCountdown());
    }

    // Coroutine of the countdown
    public IEnumerator StartCountdown(){
        countdownCanvas.SetActive(true);
        countdownText.text = "3";
        yield return new WaitForSeconds(1);
        countdownText.text = "2";
        yield return new WaitForSeconds(1);
        countdownText.text = "1";
        yield return new WaitForSeconds(1);
        countdownText.text = "GO";
        yield return new WaitForSeconds(1);
        countdownText.text =  "";
        countdownCanvas.SetActive(false);
        yield break;
    } 
    #endregion

    #region Podium Management

    // Called on client when the game is finished
    public void OnToPodium(GameFinishedMessage gsm){
        
        NetworkClient.DestroyAllClientObjects();  // destroying all of network objects
        NetworkClient.UnregisterHandler<StartGameMessage>();  // unregister the handler that were registered on the gameManagerClient script  
        string[] playersNamesStats = gsm.playersNamesStats;
        string[] playersSkinsNamesStats = gsm.playersSkinsNamesStats;
        int count = gsm.count;
        podiumHandler.SetPodium(userNameText, playersNamesStats, playersSkinsNamesStats, count);
        ToPodium();
    }

    // Called when client pressed on to lobby button (on podium sceen)
    public void OnBackToLobby(){
        podiumHandler.ResetPodium();
        podiumCanvas.SetActive(false);
        lobbyCanvas.SetActive(true);
    }
    #endregion
}
