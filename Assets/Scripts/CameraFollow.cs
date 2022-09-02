using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// class to make the camera follow the player
public class CameraFollow : MonoBehaviour
{
    
    private Vector3 offset = new Vector3(0f,-0.4f,-1.6f);
    private float smoothTime = 0.1f;
    private Vector3 velocity = Vector3.zero;
    private bool isTargetAvaible = false;  // false if player has been instantiated;
    private bool resetCamera = false;
    private Player player;
    private GameObject[] playersObjects;  // array of all players
    [SerializeField] private GameObject tutorialPlayer;  // the tutorialPlayer

    
    [SerializeField] private Transform target;  // the target of the camera( should be the player)

    // Start is called before the first frame update
    void Start()
        
    {
        if (target!= null)
        {
            isTargetAvaible = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(target == null)  // if there is still no target(player)
        {   
            // in case player has been removed, reset the camera
            if(transform.position != new Vector3(0,0,0)) 
            {
                ResetCamera();
            }
            isTargetAvaible = false;
            playersObjects = GameObject.FindGameObjectsWithTag("Player"); // find all players in scene

            foreach (GameObject playerObject in playersObjects)
            {
                player = playerObject.GetComponent<Player>(); // get the player script
                if (player.isLocalPlayer) // if user owns the player, then it is the target
                {
                    target = playerObject.transform;
                    isTargetAvaible = true; 
                }
            }

            GameObject[] tutorialPlayers = GameObject.FindGameObjectsWithTag("TutorialPlayer"); // find tutorial player in scene (there is only one)
            foreach (GameObject tutorialPlayer in tutorialPlayers){
                if(tutorialPlayer.activeInHierarchy){ // make sure tutorial player active
                    target = tutorialPlayer.transform;
                    isTargetAvaible = true; 
                }
            }
        
        }
        if (isTargetAvaible)  // if there is target
        {
            // set transform of camera to follow player
            Vector3 targetPosition = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime); 
        }
    }
    // func to reset the camera when player has been removed (e.g. game ended)
    void ResetCamera()
    {
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(0f,0f,-1.5f), ref velocity, smoothTime);
    }
}
