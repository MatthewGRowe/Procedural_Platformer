using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class TileMapPainter : MonoBehaviour  //Paints tiles when given positions!
{
    // Puts tiles on the tilemap based on given positions
    [SerializeField]
    private Tilemap floorTileMap, waterTileMap, secretPlatforms;  //These are the actual grids (they have descriptive names like "Platforms")

    //Which tiles to paint
    [SerializeField]
    private TileBase[] floorTiles;
    [SerializeField]
    private TileBase[] waterTiles;  //List of avaliable tilesets
    [SerializeField]
    private TileBase[] waterfallBottoms;  //List of avaliable tilesets

   
    public int floorTheme=0;  //A number to choose the theme to paint, set in PlatformGenerator.cs
    private int waterTheme = 0; //For the waterfalls and lava etc.
    private int secretTheme;

   

    public int getFloorTileListLength()
    {   //Called by platform Generator to generate the max value (didn't work when I stuck it in the Start)
        //Set waterTheme first (used only within this script)!
        waterTheme = UnityEngine.Random.Range(0,waterTiles.Length);
        return floorTiles.Length;
    }

    public void SetSecretTileType()
    {
        //Called by PlatformGenerator after it has set the floor theme
        //Rotate the floorTileMap so we have a different tileset used in the secret levels
        int rotation = UnityEngine.Random.Range(1, floorTiles.Length);
        secretTheme = (floorTheme + rotation) % rotation;
    }
    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)  //Recieve a list of positions
    {
       
       PaintTiles(floorPositions, floorTileMap, floorTiles[floorTheme]);
       
    }

    public void PaintSecretLevelTiles(IEnumerable<Vector2Int> secretFloorPositions)
    {
        
        //Paint the set
        PaintTiles(secretFloorPositions, secretPlatforms, floorTiles[secretTheme]);
    }

    public void PaintWaterTiles(IEnumerable<Vector2Int> waterPositions)
    {
        
        PaintTiles(waterPositions, waterTileMap, waterTiles[waterTheme]);
    }

    public void PaintWaterfallBottom(IEnumerable<Vector2Int> waterPositions)
    {
        PaintTiles(waterPositions, waterTileMap, waterfallBottoms[waterTheme]);
    }

    internal void PaintSingleBasicWall(Vector2Int position, string binaryType)
    {
        print(position + " type: " + binaryType);
        PaintSingleTile(waterTileMap, waterTiles[floorTheme], position);
        
    }

    internal void PaintSingleBasicWall(Vector2Int position, int chosenFloorTheme)
    {
        print(position + " type: " + "Test position");
        PaintSingleTile(waterTileMap, waterTiles[chosenFloorTheme], position);

    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tileMap, TileBase tile)
    {
        foreach(var position in positions)
        {
            PaintSingleTile(tileMap, tile, position);
        }
    }

    private void PaintSingleTile(Tilemap tileMap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tileMap.WorldToCell((Vector3Int)position);  //Find the tile position
        tileMap.SetTile(tilePosition, tile);

    }

    public void Clear()
    {
        floorTileMap.ClearAllTiles();
        waterTileMap.ClearAllTiles();
    }

    
}
