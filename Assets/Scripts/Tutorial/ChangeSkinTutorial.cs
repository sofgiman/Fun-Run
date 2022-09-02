using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// Class to change skin of a player
public class ChangeSkinTutorial : MonoBehaviour
{   
    
    // animators of skins
    [SerializeField] private AnimatorOverrideController templeRunnerAnim;
    [SerializeField] private AnimatorOverrideController zoeAnim;
    [SerializeField] private AnimatorOverrideController gingerAnim;
    [SerializeField] private AnimatorOverrideController blackberryAnim;
    private SkinHandler skinHandler;
    private SpriteRenderer playerSprite;

    // dictionary of a string(skin name) to animator(the animator of the skin name)
    private Dictionary<string, AnimatorOverrideController> skinAnimatorDict;
   
    private string skinName; // skin name 
    
    // Start is called before the first frame update
    void Awake(){
        skinHandler = GameObject.FindGameObjectWithTag("UIhandler").GetComponent<SkinHandler>();
        playerSprite = GetComponent<SpriteRenderer>(); // get the player sprite so we could activate it when skin is changed 
        skinAnimatorDict = new Dictionary<string, AnimatorOverrideController>(){  
            {SkinsNames.templeRunnerName,templeRunnerAnim},
            {SkinsNames.zoeName, zoeAnim},
            {SkinsNames.gingerName, gingerAnim},
            {SkinsNames.blackberryName, blackberryAnim}
        };

        skinName =  skinHandler.GetChosenSkin();
        GetComponent<Animator>().runtimeAnimatorController = skinAnimatorDict[skinName] as RuntimeAnimatorController;
    }

}
