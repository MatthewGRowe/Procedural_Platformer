using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    //Stores a list of prefabs that can be placed into the scene
    public List<GameObject> gameObjects = new List<GameObject>();
    
    public void PlaceObject(int itemReference, Vector2Int coords, int gapSize)
    {
        //Instantiate the object
        GameObject newThing = Instantiate(gameObjects[itemReference], new Vector3(coords.x, coords.y, 0), Quaternion.identity);

        //Could add further instructions here if you want (add new param)
        newThing.transform.parent = gameObject.transform;

        if (itemReference == MyTags.COIN_OBJECT_REF)
        {
            //We have put a coin down so add the gap length so that we know what the coin should do
            newThing.GetComponent<Coin>().gapLength = gapSize;
        }
        if (newThing.tag == MyTags.SAW_BLADE_TAG)
        {
            //Raise the saw blade to make it a bit harder
            int randomRaise = Random.Range(0, 5);
            Vector3 sawPos = newThing.transform.position;
            sawPos.y = sawPos.y + randomRaise;
            newThing.transform.position = sawPos;

        }

    }

    public void PlaceStairCollider(bool startOfStairs, Vector2Int coords)
    {   //This is used because we need to be able to turn the CameraNoJumpScript off if we are on the stairs.
        //Instantiate the object
        GameObject newStairCollider = Instantiate(gameObjects[MyTags.STAIR_OBJECT_REF], new Vector3(coords.x, coords.y, 0), Quaternion.identity);
        newStairCollider.transform.parent = gameObject.transform;
        newStairCollider.GetComponent<Stairs>().stairStart = startOfStairs; //Set the value so we know if we are at the start of the stairs or end

    }

    public void PlaceJumpingCoin(Vector2Int coords, int gapSize)
    {
        //Instantiate the object
        GameObject jumpingCoin = Instantiate(gameObjects[MyTags.COIN_OBJECT_REF], new Vector3(coords.x, coords.y, 0), Quaternion.identity);

        //Could add further instructions here if you want (add new param)
        jumpingCoin.GetComponentInChildren<Coin>().gapLength = gapSize;  //Set the gap size then the coin script knows what to do
        jumpingCoin.transform.parent = gameObject.transform;

        //Set the gap
        jumpingCoin.GetComponent<Coin>().gapLength = gapSize;
    }


    public void PlaceSecretTreasure(Vector2Int coords)
    {
        //Instantiate the object
        GameObject secretTreasure = Instantiate(gameObjects[MyTags.SECRETTREASURE_OBJECT_REF], new Vector3(coords.x, coords.y, 0), Quaternion.identity);

        //Could add further instructions here if you want (add new param)
        
        secretTreasure.transform.parent = gameObject.transform;

        
    }


    public void ClearItems()
    {
       //Remove all previously set items
        for (int i = this.transform.childCount; i > 0; --i)
            DestroyImmediate(this.transform.GetChild(0).gameObject);
    }

    
}
