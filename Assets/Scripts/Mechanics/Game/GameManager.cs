using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generation;

public static class GameManager
{
    public static Map map;
    public static MapGenerator1 MapGenerator;
    public static UnitSpawner unitSpawner;
    public static TileFormPlacer tileFormPlacer;
    public static IPathfinding Pathfinding;


    public static List<Unit> PlayerRelatedCharacters = new List<Unit>();



    public static bool DebugMode = true;


    public static void InitializeGame()
    {
        map = new Map();
        MapGenerator = new MapGenerator1(GameSettings.Singleton.MapGeneratorSettings, map, GameSettings.Singleton.tileMap, GameSettings.Singleton.WallTiles);
        unitSpawner = new UnitSpawner();
        tileFormPlacer = new TileFormPlacer(GameSettings.Singleton.PatternTileMap, GameSettings.Singleton.tileMap);
        Pathfinding = new NormalPathfinding(map);

        PlayerRelatedCharacters.Add(GameObject.Find("Character").GetComponent<Unit>());
    }
}
