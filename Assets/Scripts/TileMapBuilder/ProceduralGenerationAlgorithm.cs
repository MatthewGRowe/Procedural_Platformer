using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGenerationAlgorithm //Monobehavior was removed!
{

    //Algorithms avaliable to any classes that want to access them.
    public static HashSet<Vector2Int> GroundLevelPlatform(Vector2Int startPosition, int platformLength)
    {
        //Hashset is a collection of unique values with override Equals and GetHashCode
        //Params startPositon = Where we start walk
        //       platformLength = how many steps before returning vals

        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add(startPosition);
        var previousPosition = startPosition;

        for (int i = 0; i < platformLength; i++)
        {
            var newPosition = previousPosition + Direction2D.GetCardinalDirection("right");  //See code below, moves one step in a random direction
            path.Add(newPosition);
            previousPosition = newPosition;

        }
        return path;
    }


    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength)
    {
        //Hashset is a collection of unique values with override Equals and GetHashCode
        //Params startPositon = Where we start walk
        //       walkLength = how many steps before returning vals

        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add(startPosition);
        var previousPosition = startPosition;

        for (int i = 0; i < walkLength; i++)
        {
            var newPosition = previousPosition + Direction2D.GetRandomCardinalDirection();  //See code below, moves one step in a random direction
            path.Add(newPosition);
            previousPosition = newPosition;

        }
        return path;
    }


    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPosition, int corridorLength)
    {
        //Walk in a single direction for given length and return path created
        //Returns a list as there is no problem with repetition as there won't be any, also the list is in order
        List<Vector2Int> corridor = new List<Vector2Int>();
        var direction = Direction2D.GetRandomCardinalDirection(); //Choose a direction
        var currentPosition = startPosition;

        for (int i = 0; i < corridorLength; i++)
        {
            currentPosition += direction;  //Move 1 space in the direction
            corridor.Add(currentPosition); //Record the new space in our corridor definition

        }
        return corridor;
    }

    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        //Take the space to split and split it into rooms

        //BoundsInt is a struct which represent a bounding box (startpoint,endpoint) or 3 points
        //for 3D use

        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        //Split rooms to be held here
        List<BoundsInt> roomsList = new List<BoundsInt>();
        roomsQueue.Enqueue(spaceToSplit);

        while (roomsQueue.Count > 0)  //While we have rooms to split
        {
            var room = roomsQueue.Dequeue();  //Remove a room
            //Make sure it is big enought
            if (room.size.y >= minHeight && room.size.x >= minWidth)
            {
                //Room is big enough
                if (Random.value < 0.5f)  //Split horizontal
                {
                    if (room.size.y > minHeight * 2) //Can we split horizontally?
                    {
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }
                    else if (room.size.x >= minWidth * 2) //Can we split vertically
                    {
                        SplitVertically(minWidth, roomsQueue, room);
                    }
                    else
                    {
                        roomsList.Add(room);  //Simply add room to the list as we can't split it.
                    }
                }
                else //split vertically
                {
                    if (room.size.x >= minWidth * 2) //Can we split vertically
                    {
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }
                    else if (room.size.y > minHeight * 2) //Can we split horizontally?
                    {
                        SplitVertically(minWidth, roomsQueue, room);
                    }
                    else
                    {
                        roomsList.Add(room);  //Simply add room to the list as we can't split it.
                    }
                }
            }

        }

        return roomsList;

    }

    private static void SplitVertically(int minWidth, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var xSplit = Random.Range(1, room.size.x);  //Create the split
        //Define rooms x,y,z
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z),
            new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));
        //Add to room queue
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);

    }

    private static void SplitHorizontally(int minHeight, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var ySplit = Random.Range(1, room.size.y);  //Create the split
        //Define rooms x,y,z
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z),
            new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));
        //Add to room queue
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }
}
public static class Direction2D
{
    //Get a random direction
    //ALL THIS CAN PROBABLY BE DONE WITH A SINGLE RULE TILE!!
    //List of directions
    public static List<Vector2Int> cardinalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0,1), //Up
        new Vector2Int(0,-1), //Down
        new Vector2Int(1,0), //Left 
        new Vector2Int(-1,0) //Right
    };

    //List of diagonal directions
    public static List<Vector2Int> diagonalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(1,1), //Up right
        new Vector2Int(1,-1), //Down right
        new Vector2Int(-1,-1), //Down left
        new Vector2Int(1,1) //Up left 
        
    };

    public static List<Vector2Int> eightDirectionsList = new List<Vector2Int>
    {
        //Goes in clockwise direction starting at 12:00
        new Vector2Int(0,1), //Up
        new Vector2Int(1,1), //Up right
        new Vector2Int(-1,0), //Right
        new Vector2Int(1,-1), //Down right
        new Vector2Int(0,-1), //Down
        new Vector2Int(-1,-1), //Down left
        new Vector2Int(1,0), //Left 
        new Vector2Int(1,1) //Up left 
    };

    public static Vector2Int GetRandomCardinalDirection()
    {
        return cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];   //Return random direction
    }

    public static Vector2Int GetCardinalDirection(string direction)
    {
        if (direction.ToLower() == "up")
        {
            return new Vector2Int(0, 1); //Up
        
        }
        else if (direction.ToLower() == "down")
        {
            return new Vector2Int(0, -1); //Down
        }
        else if (direction.ToLower() == "left")
        {
            return new Vector2Int(1, 0); //Left 
        }
        else
        {
            return new Vector2Int(-1, 0); //Right (default)
        }
    }
}
