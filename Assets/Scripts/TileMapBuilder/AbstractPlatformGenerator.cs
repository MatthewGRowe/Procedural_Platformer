using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPlatformGenerator : MonoBehaviour 
{
    //An abstract class is a template for other classes
    //It defines the methods that they must override 

    [SerializeField]
    protected TileMapPainter tileMapPainter = null;
    [SerializeField]
    protected Vector2Int startPosition = Vector2Int.zero;
    [SerializeField]
    protected ObjectPlacer objectPlacer = null;
    [SerializeField]
    protected EnemyPlacer enemyPlacer = null;
    [SerializeField]
    protected PlayerSelector playerSelector = null;
    [SerializeField]
    protected CheckpoiintPlacer checkPointPlacer = null;
    [SerializeField]
    protected LightPlacer lightPlacer = null;
    [SerializeField]
    protected SpringPlacer springPlacer = null; 

    public void GenerateLevel()
    {
        tileMapPainter.Clear();  //Will call the method from the tileMapVisualiser
        RunProceduralGeneration();
    }

  
    protected abstract void RunProceduralGeneration();
}
