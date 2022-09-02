using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// Class to change skin of a player
public class ChangeSkin : NetworkBehaviour
{   
    
    // animators of skins
    [SerializeField] private AnimatorOverrideController templeRunnerAnim;
    [SerializeField] private AnimatorOverrideController zoeAnim;
    [SerializeField] private AnimatorOverrideController gingerAnim;
    [SerializeField] private AnimatorOverrideController blackberryAnim;
    private SpriteRenderer playerSprite;

    // dictionary of a string(skin name) to animator(the animator of the skin name)
    private Dictionary<string, AnimatorOverrideController> skinAnimatorDict;
    //[SyncVar(hook = nameof(OnSkinNameChanged))]  // when the skin name variable change go to this function
    [SyncVar]
    public string skinName; // skin name sync between all clients so they know the other player skin 
    
    // Start is called before the first frame update
    void Awake(){
        playerSprite = GetComponent<SpriteRenderer>(); // get the player sprite so we could activate it when skin is changed 
        
    }
    void Start()
    {   
        // Initialize dictionary
        skinAnimatorDict = new Dictionary<string, AnimatorOverrideController>(){  
            {SkinsNames.templeRunnerName,templeRunnerAnim},
            {SkinsNames.zoeName, zoeAnim},
            {SkinsNames.gingerName, gingerAnim},
            {SkinsNames.blackberryName, blackberryAnim}
        };
        if(skinName!= null && !playerSprite.enabled){
            // change animator and enable player sprite to be shown    
            GetComponent<Animator>().runtimeAnimatorController = skinAnimatorDict[skinName] as RuntimeAnimatorController;
            playerSprite.enabled = true;
          }
    }
}
