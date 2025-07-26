using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This creates a menu option that you can use in Unity Asset Folder right-click, it is very fancy (see the same folder as this script)!
[CreateAssetMenu(fileName = "TreasureSpriteDatabase", menuName = "Treasure/Treasure Sprite Database")]
public class TreasureSpriteDatabase : ScriptableObject //<-- Important change here!
{
    //This is used by the SecretTreasure.cs script
    public List<Sprite> treasureSprites;
}
