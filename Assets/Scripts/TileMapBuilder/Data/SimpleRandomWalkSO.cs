using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="SimpleRandomWalkParameters_",menuName = "PCG/SimpleRandomWalkData")]
public class SimpleRandomWalkSO : ScriptableObject //Means you can create it from the inspector (under Create Menu)
{
    //All params for simple platforms
    public int iterations = 10, maxPlatformLength = 10, maxGapLength = 2, maxHeightChange = 3;
    public bool startRandomlyEachIteration = true;
}
