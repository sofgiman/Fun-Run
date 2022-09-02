using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Class to handle the skin change from lobby
public class SkinHandler : MonoBehaviour
{
    [SerializeField] private Image imageContainer; // skin image container 
    [SerializeField] private Text nameContainer;  // name of the skin
    [SerializeField] private GameObject right;  // button to move to the right skin
    [SerializeField] private GameObject left;  // button to move to the left skin

    // each idle skin of all skins
    [SerializeField] private Sprite templeRunner;
    [SerializeField] private Sprite zoe;
    [SerializeField] private Sprite ginger;
    [SerializeField] private Sprite blackberry;
    private LinkedList<Skin> skinsList;  // linked list of skins
    private LinkedListNode<Skin> currSkin;  
    
   
    // Start is called before the first frame update
    void Start()
    {
        // initialize the skins (made of skin default image and its name)
        Skin[] skinsArray = {
            new Skin(templeRunner, SkinsNames.templeRunnerName),
            new Skin(zoe, SkinsNames.zoeName),
            new Skin(ginger, SkinsNames.gingerName),
            new Skin(blackberry,SkinsNames.blackberryName)
            };
        skinsList = new LinkedList<Skin>(skinsArray);  // initialize thre linked list
        currSkin = skinsList.First;  // curr skin choesn is the first node
        left.SetActive(false);  // left button should be unactive
        

    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    // function called when right button pressed, made so the righer skin will be chosen
    public void Right(){
        currSkin = currSkin.Next; // move to to right node
        imageContainer.sprite = currSkin.Value.SpriteIdle;  // change image to skin's idle
        nameContainer.text = currSkin.Value.Name;  // change name container text to skin's name
        if(currSkin.Next == null){  // unactive the right button if there is no more right skins
            right.SetActive(false);
        }
        if(!left.activeSelf){  // active the left button if it is unactive
            left.SetActive(true);
        }

    }
    // function called when left button pressed, made so the left skin will be chosen
     public void Left(){
        currSkin = currSkin.Previous; // move to to left node
        imageContainer.sprite = currSkin.Value.SpriteIdle;   // change image to skin's idle
        nameContainer.text = currSkin.Value.Name;  // change name container text to skin's name
       
        if(currSkin.Previous == null){  // unactive the left button if there is no more left skins
            left.SetActive(false);
        }
        if(!right.activeSelf){  // active the right button if it is unactive
            right.SetActive(true);
        }

    }
    public string GetChosenSkin(){
        return currSkin.Value.Name;
    }

}
