using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

// Class that make the podium on a client
public class PodiumUIhandler : MonoBehaviour
{  
    private Dictionary<int, string> placeIndexToString;  
    private Dictionary<string, Sprite> skinNameToIdleSprite;  // dict to convert skin name to its sprite
    [SerializeField] private GameObject placementText;  // game object to tell the user where he finished
    [SerializeField] private GameObject[] placements;  // arrays of all placements objects (0 is 1st place...)
    //[SerializeField] private  placementse;
	[SerializeField] private Sprite templeRunner;
    [SerializeField] private Sprite zoe;
    [SerializeField] private Sprite ginger;
    [SerializeField] private Sprite blackberry;

    void Awake(){
        placeIndexToString = new Dictionary<int, string>(){
            {1, "1ST"},
            {2, "2ND"},
            {3, "3RD"},
            {4, "4TH"}
        };

        skinNameToIdleSprite = new Dictionary<string, Sprite>(){  
            {SkinsNames.templeRunnerName,templeRunner},
            {SkinsNames.zoeName, zoe},
            {SkinsNames.gingerName, ginger},
            {SkinsNames.blackberryName, blackberry}
        };
    }
   
    // Function to set the podium 
    // username - client's username
    // playersNamesStats - the usernames of the users (ordered by their finish line)
    // playersSkinsNamesStats - the skins of the players (ordered by their finish line)
    // count - number of players
    public void SetPodium(string username, string[] playersNamesStats, string[] playersSkinsNamesStats, int count){
        for(int i =0 ;i<count;i++){  // looping the number of players
            
            string playerSkinName = playersSkinsNamesStats[i];  // getting the skin name of the user who finished (i + 1) place
            Transform skinImageHolder = placements[i].transform.GetChild(0);  // refrence to the sprite skin holder  
            skinImageHolder.transform.GetComponent<Image>().sprite = skinNameToIdleSprite[playerSkinName];
            
            string playerName = playersNamesStats[i];
            if(playerName == username){  // checking if local client finished i+1 place
                Transform arrowHolder = placements[i].transform.GetChild(1);
                arrowHolder.transform.GetComponent<Image>().enabled = true;  // enable the arrow so the user will know in what place he finished
                int place = i + 1;
                placementText.GetComponent<Text>().text = placeIndexToString[place]  + " PLACE";
            }

            placements[i].SetActive(true);
        }   
    }

    // Func to reset all variables of the podium
    public void ResetPodium(){
        foreach(GameObject placementHolder in placements){
            // resetting all images holders
            Transform skinImageHolder = placementHolder.transform.GetChild(0);
            skinImageHolder.transform.GetComponent<Image>().sprite = null;
            
            // disabling all arrows holders
            Transform arrowHolder = placementHolder.transform.GetChild(1);
            arrowHolder.transform.GetComponent<Image>().enabled = false;
            
            placementText.GetComponent<Text>().text = "";
            placementHolder.SetActive(false);  // deactivating all placement holders objects
        }
    }
}
