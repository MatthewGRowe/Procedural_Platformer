using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringPlacer : MonoBehaviour
{
 
    //Stores a list of prefabs that can be placed into the scene
    public List<GameObject> springOrLaunchPrefabs = new List<GameObject>();

    public void PlaceSpring(int itemReference, Vector2Int coords)
    {
        //Instantiate the object
        GameObject newThing = Instantiate(springOrLaunchPrefabs[itemReference], new Vector3(coords.x, coords.y, 0), Quaternion.identity);
        //if (newThing.tag == MyTags.DISCOBALL_TAG)
        //{
        //    Vector3 myTrans = newThing.transform.position;
        //    myTrans.y = myTrans.y + 1;
        //    newThing.transform.position = myTrans;
        //}
        //Could add furhter instructions here if you want (add new param)
        newThing.transform.parent = gameObject.transform;



    }





    public void ClearItems()
    {

        //Remove all previously set items
        for (int i = this.transform.childCount; i > 0; --i)
            DestroyImmediate(this.transform.GetChild(0).gameObject);
    }

}

