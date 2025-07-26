using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class PlatformGenerator : AbstractPlatformGenerator
{ //Called SimpleRandomWalkGenerator in the video, uses a CUSTOM BUTTON, SEE https://youtu.be/U3Wr-sNnJNk?t=333

    //Used to generate a series of vector2's which store the location of where to paint our
    //platforms

    [SerializeField]
    protected SimpleRandomWalkSO platformParameters;  //Display the params from the SimpleRandomWalkSO script
                                                      //Protected enables us to see it from child scripts


    //Playerref can be found in GameManager.instance.selectedPlayerIndex

    Vector2 lastPlatformPosition;  //Required to build boss platform at the right level (set in GenerateRandomBaseLevelPlatforms)

    public GameObject waterfallSensorPrefab; //Needed to place Waterfall Proximity Sensor





    private void Start()
    {


        GenerateLevel();//Generate level on each new play (scene reload)
       

    }
    public void BuildLevel()
    {

        //RunProceduralGeneration();
        //print("Called from Buildlevel");
    }

    //Defined in AbstractPlatformGenerator!!!
    protected override void RunProceduralGeneration()  //Protected means you can only access it from the child classes and abstract class
    {                                                   //Called by button in Editor!
        //Called via GameManager/OnSceneLoaded(), and then AbstractPlatformGenerator.cs
        //Determine the theme (held in TileMapPainter):
        GetTheme();
        //print("Clearing all and sundry");
        tileMapPainter.Clear(); //Clear the tiles
        objectPlacer.ClearItems(); //Clear the items (sawblades etc...)
        enemyPlacer.ClearItems(); //Clear the enemies
        playerSelector.ClearPlayer();  //Clear player
        checkPointPlacer.ClearItems(); //Clear checkpoints
        lightPlacer.ClearItems(); //Clear the old lights
        springPlacer.ClearItems(); //Clear the old springs and wotnot
        HashSet<Vector2Int> floorPositions = GenerateRandomBaseLevelPlatforms(platformParameters, startPosition);



        tileMapPainter.PaintFloorTiles(floorPositions);



        //We need a player regardless
        //Last thing, place player
        playerSelector.PlacePlayer();

    }

    private void GetTheme()
    {
        //Sets theme for the level based on the time, this is stored in the TileMapPainter, called from method above
        int floorTheme = UnityEngine.Random.Range(0, tileMapPainter.getFloorTileListLength());

        /*if (DateTime.Now.Millisecond % 5 == 0)  //If datetime was even
        {
            theme = 1;
        }
        else if (DateTime.Now.Millisecond % 3 == 0)
        {
            theme = 2;
        }*/
        tileMapPainter.floorTheme = floorTheme;  //Theme is set  

        //Set secret theme
        tileMapPainter.SetSecretTileType();  //Random rotation from thme to give visibly different tiles for platforms

    }

    protected HashSet<Vector2Int> GenerateRandomBaseLevelPlatforms(SimpleRandomWalkSO parameters, Vector2Int position)
    {
        //print("Generate Base Level Platforms");
        bool stepOrLadderAddedRecently = false;
        bool springAddedRecently = false;
        var currentPosition = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();

        //If this is the first iteration draw 8 platforms so the player always has somewhere to start
        for (int j = 0; j < 8; j++)
        {
            currentPosition = new Vector2Int(currentPosition.x + 1, currentPosition.y);
            floorPositions.Add(currentPosition);

            //Add depth (below)
            for (int z = 1; z < Random.Range(8, 18); z++)
            {
                floorPositions.Add(new Vector2Int(currentPosition.x, currentPosition.y - z));
            }
        }

        int lightSelector = Random.Range(0, lightPlacer.lightPrefabs.Count);


        for (int i = 0; i < parameters.iterations; i++)
        {
            //If you want something every platform place it here!

            //Only place a checkpoint every 4 spaces
            if (i > 6 && i % 8 == 0)
            {
               
                checkPointPlacer.PlaceItem(0, new Vector2Int(currentPosition.x + 2, currentPosition.y + 5));
            }



            int platformLength = Random.Range(1, parameters.maxPlatformLength);
            //Draw a platform
            for (int x = 0; x <= platformLength; x++)
            {

                currentPosition = new Vector2Int(currentPosition.x + 1, currentPosition.y);
                floorPositions.Add(currentPosition);

                //Add a light, (1 light every 15 steps)
                if (x % 15 == 0)
                {
                    //Discoballs higher than candles!
                    if (lightSelector == 0) //Candle
                    {
                        lightPlacer.PlaceLight(lightSelector, new Vector2Int(currentPosition.x, currentPosition.y + Random.Range(4, 6)));
                    }
                    else //Discoball //extend as more lights go in
                    {
                        lightPlacer.PlaceLight(lightSelector, new Vector2Int(currentPosition.x, currentPosition.y + Random.Range(6, 8)));
                    }

                }

                //Add depth (below)
                for (int z = 1; z < Random.Range(8, 18); z++)
                {
                    floorPositions.Add(new Vector2Int(currentPosition.x, currentPosition.y - z));
                }

                //Do we need some steps or a ladder (to make it more interesting)
                if (DateTime.Now.Millisecond % 10 == 0 && stepOrLadderAddedRecently == false)  //If datetime was disible by 10
                {
                    stepOrLadderAddedRecently = true;  //Without this time moves too slowly and you end up with massive steps
                                                       //Draw steps

                    int stepHeight = Random.Range(2, 22);
                    /******** TEST ****/
                    objectPlacer.PlaceStairCollider(true, new Vector2Int(currentPosition.x, currentPosition.y)); //Place collider to indicate start of stairs
                    bool upStairs = Random.value > 0.5f;  //Either the stairs are ascending or descending
                    /***************/
                    for (int y = 0; y <= stepHeight; y++)
                    {
                        //Push the level up (max 22 steps)
                        if (upStairs)
                        {
                            currentPosition = new Vector2Int(currentPosition.x, currentPosition.y + 1);
                        }
                        else
                        {
                            currentPosition = new Vector2Int(currentPosition.x, currentPosition.y - 1);
                        }
                        int stepLength = Random.Range(1, 4);

                        for (int a = 0; a <= stepLength; a++)
                        {
                            //Draw a platform and move the level along (max 4 steps)
                            currentPosition = new Vector2Int(currentPosition.x + 1, currentPosition.y);

                            floorPositions.Add(new Vector2Int(currentPosition.x, currentPosition.y));


                            //Add depth (below)
                            for (int z = 1; z < Random.Range(2, 8); z++)
                            {
                                floorPositions.Add(new Vector2Int(currentPosition.x, currentPosition.y - z));
                            }
                        }
                    }
                    /******** TEST ****/
                    objectPlacer.PlaceStairCollider(false, new Vector2Int(currentPosition.x, currentPosition.y));  //Place collider to indicate end of stairs
                    /***************/
                }
                else if (DateTime.Now.Millisecond % 13 == 0)  //If datetime was disible by 13
                {
                    stepOrLadderAddedRecently = false;

                }
                else if (DateTime.Now.Millisecond % 11 == 0 && springAddedRecently == false)
                {
                    //We have decided not to add any stairs so let's add a spring to launch us to a secret level
                    springAddedRecently = true;

                    //Pick the launch mechanism
                    int launcher = Random.Range(0,springPlacer.springOrLaunchPrefabs.Count);

                    springPlacer.PlaceSpring(launcher, new Vector2Int(currentPosition.x + 2, currentPosition.y + 2));
                    
                    AddSecretLevel(new Vector2Int(currentPosition.x + 2, currentPosition.y + 1)); //This will place the secret level, above the spring
                }
                else if (DateTime.Now.Millisecond % 14 == 0 && springAddedRecently == true)
                {
                    springAddedRecently = false;
                }
            }

            //Decide whether or not to place an enemy on the platform
            if (platformLength > 2)
            {
                //When you have more than one object generate a random object selector
                int objectChosen = Random.Range(0, enemyPlacer.gameObjects.Count);  //Random.Range(0,whatever);
                //Random nonsence to decide if an enemy is to be placed
                if (DateTime.Now.Millisecond % 2 == 0)  //If datetime was disible by 3
                {
                    Vector2Int platformCentre = new Vector2Int(currentPosition.x - platformLength / 2, currentPosition.y + 5);
                    //Call the enemy placer
                    enemyPlacer.PlaceItem(objectChosen, platformCentre);
                }

            }




            //Calculate gap
            int gap = Random.Range(1, parameters.maxGapLength);

            //Place a random object every so often
            if (i > 5 && i % 5 == 0)
            {
                //This isn't called much because coins and saws are placed more frequently so you will get far fewer guns here.
                int rnd = Random.Range(0, objectPlacer.gameObjects.Count);

                objectPlacer.PlaceObject(rnd, new Vector2Int(currentPosition.x, currentPosition.y + 5), gap);
            }

            currentPosition = new Vector2Int(currentPosition.x + gap, currentPosition.y);

            //Calculate a rise or drop in level (the bigger the gap the smaller the height difference)
            int newY, levelChange = 0;  //New levels
            float formula = 1 / gap;  //Inverse range

            if (formula >= 0.2f)
            {
                levelChange = Random.Range(0, Convert.ToInt16(parameters.maxHeightChange / 2));   //Half max height
            }
            else if (formula >= 0)
            {
                levelChange = Random.Range(0, parameters.maxHeightChange); //Max allowable height change
            }

            newY = Random.Range(-levelChange, levelChange);

            currentPosition = new Vector2Int(currentPosition.x, currentPosition.y + newY);

            if (gap > 4)  //Any big gap!
            {
                //Put a bonus coin 3 places before the gap so that the player can actually jump it.
                Vector2Int gapEdge = new Vector2Int(currentPosition.x - gap - 3, currentPosition.y + 5);
                int objectChosen = MyTags.COIN_OBJECT_REF;

                //Call the object placer
                objectPlacer.PlaceJumpingCoin(gapEdge, gap);

            }


            if (newY == 0)  //If a gap is between two of the same level platforms, fill it with water
            {
                PaintAWaterfall(currentPosition, gap);
            }
            else if (gap >= 4)  //Put objects into big gaps  (Else as we don't want objects in the waterfalls)
            {


                //Calculate the centre of the gap
                int middleGap = Convert.ToInt16(gap / 2);
                Vector2Int gapCentre = new Vector2Int(currentPosition.x - middleGap, currentPosition.y);
                //When you have more than one object generate a random object selector
                int objectChosen = MyTags.SAW_OBJECT_REF;
                //Call the object placer
                objectPlacer.PlaceObject(objectChosen, gapCentre, gap);
            }


        }

        //At this stage the entire map has been built, the end position is "currentPosition"
        //so now we can place the boss platform.
        Vector2Int BossPlatformStart = new Vector2Int(currentPosition.x, currentPosition.y); //Vector2Int is ints not floats so better for accurate tile placement
        BuildBossPlatform(BossPlatformStart, ref floorPositions);







        return floorPositions;

    }

    private void AddSecretLevel(Vector2Int startPosition)
    {
        int height = Random.Range(18, 21);
        int xDiff = 8; // Fixed typo: Removed Random.Range(6,9) since it was unused
        HashSet<Vector2Int> secretFloorPositions = new HashSet<Vector2Int>();

        Vector2Int platformStart = new Vector2Int(startPosition.x + xDiff, startPosition.y + height);
        Vector2Int currentPosition = platformStart;
        Vector2Int ladderPosition = new Vector2Int();

        int platformLength = Random.Range(30, 80);
        bool ladderPlaced = true;

        while (ladderPlaced) // Simplified condition
        {
            ladderPlaced = false;
            for (int a = 0; a <= platformLength; a++)
            {
                currentPosition = new Vector2Int(currentPosition.x + 1, currentPosition.y);
                secretFloorPositions.Add(currentPosition);

                int rnd = Random.Range(1, 50);
                if (rnd > 9 && rnd < 11)
                    enemyPlacer.PlaceItem(Random.Range(0, enemyPlacer.gameObjects.Count), new Vector2Int(currentPosition.x, currentPosition.y + 4));
                else if (rnd > 30 && rnd < 32)
                    objectPlacer.PlaceObject(Random.Range(0, 3), new Vector2Int(currentPosition.x, currentPosition.y + 4), 1);
                else if (rnd > 45 && !ladderPlaced)
                {
                    objectPlacer.PlaceObject(4, new Vector2Int(currentPosition.x, currentPosition.y + 6), 1);
                    ladderPlaced = true;
                    ladderPosition = new Vector2Int(currentPosition.x, currentPosition.y + 1);
                }
            }

            if (ladderPlaced)
            {
                currentPosition = new Vector2Int(ladderPosition.x, ladderPosition.y + 8);
                platformLength = Random.Range(10, 80);
                if (Random.Range(1, 5) <= 2)
                    currentPosition.x -= platformLength - 4; // Build left
            }
        }

        // Place treasure on the FINAL platform position (after all ladders)
        objectPlacer.PlaceSecretTreasure(new Vector2Int(currentPosition.x, currentPosition.y +4));
        tileMapPainter.PaintSecretLevelTiles(secretFloorPositions);
    }

    private void BuildBossPlatform(Vector2Int platformStartPosition, ref HashSet<Vector2Int> floorPositions)
    {
        //Build the boss platform (a plain platform with a checkpoint at the start)
        //Calculate start position of boss platform
        int newX = Random.Range(1, 6);  //Not massive
        int newY = Random.Range(+6, -3);




        newX = (int)platformStartPosition.x + 3; // + newX;
        newY = (int)platformStartPosition.y; // + newY;

        //Place final checkpoint
        // checkPointPlacer.PlaceItem(999, platformStartPosition);
        checkPointPlacer.PlaceItem(0, new Vector2Int(platformStartPosition.x + 8, platformStartPosition.y + 5));


        for (int x = 0; x < 100; x++)
        {
            //Build a platform 100 wide x 8 deep
            for (int y = 0; y >= -8; y--)
            {
                int tempX = newX + x;  //Move the platform along

                int tempY = newY + y;
                //tileMapPainter.PaintSingleBasicWall(new Vector2Int(tempX, tempY), 6); //Paint with theme 7 (concrete)
                floorPositions.Add(new Vector2Int(tempX, tempY));
            }

        }

        //Boss platform is now built
        //Note where to place the boss
        GameManager.instance.bossDropLocation = new Vector3(newX + 40, newY + 20, 0);
    }

    private void PaintAWaterfall(Vector2Int currentPosition, int gap)
    {
        //Build the gap hashset
        HashSet<Vector2Int> gapCoords = new HashSet<Vector2Int>();

        //Calculate waterfall centre in order to locate the proximity sensor (which makes the noise)
        int startX = currentPosition.x - gap + 1;
        int endX = currentPosition.x;
        int middleX = (startX + endX) / 2;
        Vector2Int sensorPosition = new Vector2Int(middleX, currentPosition.y);


        //Draw waterfall
        //Waterfall depth hardcoded to 5
        int depth = Random.Range(3, 12);
        for (int x = currentPosition.x - gap + 1; x <= currentPosition.x; x++)
        {
            for (int y = 1; y < depth; y++)  //Draw downward (waterfall) //Depth
            {
                gapCoords.Add(new Vector2Int(x, currentPosition.y - y));
            }
        }

        //Send gap and paint water tiles
        tileMapPainter.PaintWaterTiles(gapCoords);
        //Calc the bottom of waterfall
        HashSet<Vector2Int> wfBottom = new HashSet<Vector2Int>();
        for (int cnt = currentPosition.x - gap + 1; cnt <= currentPosition.x; cnt++)
        {
            wfBottom.Add(new Vector2Int(cnt, currentPosition.y - depth));  //Draw row at bottom of waterfall
        }
        tileMapPainter.PaintWaterfallBottom(wfBottom);

        //Proximity Sensor
        GameObject sensor = GameObject.Instantiate(waterfallSensorPrefab, new Vector3(sensorPosition.x + 0.5f, sensorPosition.y + 0.5f, 0), Quaternion.identity);
        // If you know TileMapPainter is the first child
        Transform tMapPainter = transform.GetChild(0);
        sensor.transform.parent = tMapPainter; // keeps hierarchy tidy
    }
}
