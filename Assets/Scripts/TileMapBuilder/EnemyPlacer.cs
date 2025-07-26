using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class EnemyPlacer : MonoBehaviour
{
    //Stores a list of prefabs that can be placed into the scene
    public List<GameObject> gameObjects = new List<GameObject>();
    public GameObject boss;  //The mad doctor!

    private void Start()
    {
        StartCoroutine(DropBoss());
    }

    public void PlaceItem(int itemReference, Vector2Int coords)
    {
        //Instantiate the object
        GameObject newThing = Instantiate(gameObjects[itemReference], new Vector3(coords.x, coords.y, 0), Quaternion.identity);

        if (newThing.tag == MyTags.ENEMY_BIRD_TAG)
        {
            //Place in the air
            float newY = newThing.transform.position.y + 2.8f;
            newThing.transform.position = new Vector3(coords.x, newY, 0);
        }

        //Could add furhter instructions here if you want (add new param)
        newThing.transform.parent = gameObject.transform;
    }

    IEnumerator DropBoss()
    {
        // wait until the game has officially started
        yield return new WaitForSeconds(5);

        // NOW add the boss
       
        GameObject newThing = Instantiate(boss, GameManager.instance.bossDropLocation, Quaternion.identity);
        
        newThing.transform.parent = gameObject.transform;
    }
    public void ClearItems()
    {

        //Remove all previously set items
        for (int i = this.transform.childCount; i > 0; --i)
            DestroyImmediate(this.transform.GetChild(0).gameObject);
    }
}
