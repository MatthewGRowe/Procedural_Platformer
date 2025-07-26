using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerSelector : MonoBehaviour
{
    //Chooses a player object and sets parameters based on the tilemap properties
    
    public List<GameObject> playerPrefabs = new List<GameObject>();

    public void PlacePlayer()  //Returns the player selected (so we can choose appropriate lighting called from PlatformGenerator.cs)
    {
       
        //Find the camera
        var vcam = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();

        //Select Player (only needed if we are not reusing the same player)
        int playerIndex = Random.Range(0, playerPrefabs.Count);
        //Record index in case of resuse
        GameManager.instance.selectedPlayerIndex = playerIndex;
        
        //playerIndex = 3;   //Hardcoded player setting
        //Instantiate the player at 5,8,0
        GameObject player = Instantiate(playerPrefabs[GameManager.instance.selectedPlayerIndex], GameManager.playerStartPosition, Quaternion.identity);
       // GameObject player = Instantiate(playerPrefabs[playerIndex], vcam.transform.position, Quaternion.identity);
         
        //Set parent

        player.transform.SetParent(gameObject.transform,true);
        vcam.Follow = player.transform;



        //Read values from Platform Generator
        //Might need a GameManager to select the parameter set so we can all use the same params (ie the player generator and PlatformGenerator)
        //SimpleRandomWalkSO parameters = new SimpleRandomWalkSO(); // <<ORIGINAL LINE, REMOVED?!
        //SimpleRandomWalkSO parameters = SimpleRandomWalkSO.CreateInstance();

        //Ensure level is possible by adding power over the min
        //Took these out and set basic values in the GameManager.cs
        //GameManager.instance.alterJumpPower(parameters.maxGapLength + Random.Range(0, 5));
        //player.GetComponent<PlayerMovement>().speed = Random.Range(3, 10);

        //Set in camera

        vcam.Follow = player.transform;

        
    }

   

    public void ClearPlayer()
    {

        //Remove all previously set items
        for (int i = this.transform.childCount; i > 0; --i)
            DestroyImmediate(this.transform.GetChild(0).gameObject);
    }


}
