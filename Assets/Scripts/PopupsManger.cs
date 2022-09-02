using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum Popup{
    None,
    Welcome, // 1
    IceExplanation, // 2  
    ShowUpArrow, // 3
    WaitingForFirstJump, // 4 
    ShowWallJumping, // 5
    WaitingForWallJumping, // 6
    ShowDownArrow, // 7
    FreezeScreen, // 8
    WaitingForSlide, // 9
    ShowExit // 10
}

// A class to handle all popups on the the game's tutorial
public class PopupsManger : MonoBehaviour
{
    private int popUpIndex = 1;
    
    [SerializeField] private Animator popupsAnim;
    
    [SerializeField] private GameObject WelcomeGameObject;
    [SerializeField] private GameObject IceExplanationGameObject;
    [SerializeField] private GameObject UpArrowExplanationGameObject;
    [SerializeField] private GameObject WallJumpingExplanationGameObject;
    [SerializeField] private GameObject DownArrowExplanationGameObject;
    [SerializeField] private GameObject ExitGameObject;

    [SerializeField] private PlayerTutorial playerMovment;  // the player tutorial script
    
   // [SerializeField] private GameObject tutorialPlayer;  // the tutorial player object
    //[SerializeField] private Vector3 startPlayerPos;  // starting position of the player
    private string[] FirstPopupSentences = {
    "Welcome to Fun Run's tutorial", 
    "Here you will learn how to use the controls",
    "But be aware",
    "In the game, you will race to the finish line",
    "With real people!"};
    

   
    void Start(){
       // startPlayerPos = tutorialPlayer.transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        CheckPopups();
    }

    // Function to check if certians popups were completed
    private void CheckPopups()
    {
        if( popUpIndex == (int)Popup.WaitingForFirstJump){ // waiting for the first jump
            if(Input.GetButtonDown("Jump")){  // if user completed the first jump
                playerMovment.EnableJump();  // jump the player
                FirstJumpCompleted();
            }

        }
        else if (popUpIndex == (int)Popup.WaitingForWallJumping){  // waiting for finishig wall jumping
            if(Input.GetButtonDown("Jump")){
               playerMovment.EnableJump();  // allowing player to jump
            }
        }
        else if (popUpIndex == (int)Popup.WaitingForSlide){   // waiting for sliding
            if(Input.GetButtonDown("Slide")){
               Time.timeScale = 1;  // unfreezing the game
               playerMovment.EnableSlide();  // slide the player
               SlideCompleted();
            }
        }
    }

    // Showing the welcome sentences (first popup)
    // index - the index of the sentence 
    void ShowWelcomeSentences(int index){
        WelcomeGameObject.GetComponent<Text>().text = FirstPopupSentences[index];
        if(index == 3){
            playerMovment.EnableMovment();
        }
        if(index == 4)
            PopupCompleted(); // 1 -> 2
       

    }

    //  Showing the ice explanation
    void ShowIceExplanation(){
        WelcomeGameObject.SetActive(false);
        IceExplanationGameObject.SetActive(true);
        popupsAnim.SetBool("IceExplanation", true);
        PopupCompleted(); // 2 -> 3
        
    }

    // Showing the up arrow (telling the user where the jump button is and that he can jump )
    void ShowUpArrow(){
       
        IceExplanationGameObject.SetActive(false);
        UpArrowExplanationGameObject.SetActive(true);
        popupsAnim.SetBool("ShowUpArrow", true);
        PopupCompleted(); // 3 -> 4
        
    }

    // Func to fade the up arrow because the user completed the jump
    void FirstJumpCompleted(){
        
        popupsAnim.SetBool("UpArrowCompleted",true);
        PopupCompleted();  // 4 -> 5    
        
    }

    // Func to tell the user he can jump on walls
    void ShowWallJumping(){
        UpArrowExplanationGameObject.SetActive(false);
        WallJumpingExplanationGameObject.SetActive(true);
        popupsAnim.SetBool("ShowWallJumping", true);
        PopupCompleted(); // 5 -> 6
    }
    
    // func called when user finished wall jumping 
    void WallJumpingCompleted(){
        popupsAnim.SetBool("WallJumpingCompleted",true);  // fading the wall jumping text
        PopupCompleted(); // 6 -> 7 
    }

    // Func called to show the user where the slide button and that he can slide mid air
    void ShowDownArrow(){
        WallJumpingExplanationGameObject.SetActive(false);
        DownArrowExplanationGameObject.SetActive(true);
        popupsAnim.SetBool("ShowDownArrow", true);
        PopupCompleted(); // 7 -> 8
    }

    // Func that freezes screen (to make sure the user pressed the slide button)
    void FreezeScreen(){
        Time.timeScale = 0;
        PopupCompleted();  // 8 -> 9
    }

    // Fun called when player slided on the first time
    void SlideCompleted(){
        popupsAnim.SetBool("DownArrowCompleted",true);  // fading the slide text
        PopupCompleted();  // 9 -> 10   
    }

    // Func to show user he finished the tutorial
    void ShowExit(){
        DownArrowExplanationGameObject.SetActive(false);
        ExitGameObject.SetActive(true);
        playerMovment.EnableConrolls();
        popupsAnim.SetBool("ShowExit", true);
        PopupCompleted(); // 10 -> 11
    }

    // Func to change the exit sentence (called from exit animation)
    void ChangeExitSentectce(){
        
        ExitGameObject.GetComponent<Text>().text = "Keep practicing or go back to the lobby";
    }

    // Callded from tutorial player when he collides on a tutorial popup
    // Collisions of player and popups colliders on the map allow to know where the player is 
    // That way, we know what popup should be shown  
    public void AddCollision(){
        if(popUpIndex  == (int)Popup.IceExplanation){
            ShowIceExplanation();
        }
        else if(popUpIndex == (int)Popup.ShowUpArrow){
            ShowUpArrow();
        }
        else if(popUpIndex == (int)Popup.ShowWallJumping){
            ShowWallJumping();
        }
        else if(popUpIndex == (int)Popup.WaitingForWallJumping){  // player finished the wall jumping
            WallJumpingCompleted();
        }
         else if(popUpIndex == (int)Popup.ShowDownArrow){
            ShowDownArrow();
        }
        else if( popUpIndex == (int)Popup.FreezeScreen)
        {
             FreezeScreen();
        }
        else if(popUpIndex == (int)Popup.ShowExit){
            ShowExit();
        }
    }
    
    // Func to increase the popup index
    private void PopupCompleted(){
        popUpIndex++;
    }
}
