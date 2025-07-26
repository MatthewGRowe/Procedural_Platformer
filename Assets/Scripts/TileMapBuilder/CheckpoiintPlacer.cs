using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CheckpoiintPlacer : MonoBehaviour
{
    //Stores a list of prefabs that can be placed into the scene
    public List<GameObject> gameObjects = new List<GameObject>();

    

    public void PlaceItem(int itemReference, Vector2Int coords)
    {
        //Instantiate the object (comes from an array with only 1 thing in it, allows you to add more checkpoint styles if required)
        //currently we only use the zero element as only 1 checkpoint style exists
        GameObject newCheckpoint = Instantiate(gameObjects[itemReference], new Vector3(coords.x, coords.y, 0), Quaternion.identity);

        //Note: Total checkpoints is held in GameManager
        GameManager.instance.totalCheckPoints++; //So we don't start at zero increment here
      
        //Read the latest checkpoint number and store into the checkpoint we have just placed
        newCheckpoint.GetComponent<checkPoint>().myNumber = GameManager.instance.totalCheckPoints;
        
        //Increment the checkpoint number
        
        
        //Could add furhter instructions here if you want (add new param)
        newCheckpoint.transform.parent = gameObject.transform;

    }

    public void ClearItems()
    {

        //Remove all previously set items
        for (int i = this.transform.childCount; i > 0; --i)
            DestroyImmediate(this.transform.GetChild(0).gameObject);
    }
}
