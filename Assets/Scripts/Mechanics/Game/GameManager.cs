using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generation;

public static class GameManager
{
    public static bool GameIsRunning = true;

    public static Map map;
    public static MapGenerator1 MapGenerator;
    public static UnitSpawner unitSpawner;
    public static TileFormPlacer tileFormPlacer;
    public static IPathfinding Pathfinding;
    public static System.Random _random;


    public static List<Unit> PlayerRelatedCharacters = new List<Unit>();
    public static List<Vector3> PlayerCharactersPositions = new List<Vector3>();



    public static bool DebugMode = true;

    public static void SetPlayersPositions()
    {
        foreach (var PlayerUnit in PlayerRelatedCharacters)
        {
            PlayerCharactersPositions[PlayerRelatedCharacters.IndexOf(PlayerUnit)] = PlayerUnit.transform.position;
        }
    }
    public static void InitializeGame()
    {
        map = new Map();
        MapGenerator = new MapGenerator1(GameSettings.Singleton.MapGeneratorSettings, map, GameSettings.Singleton.tileMap, GameSettings.Singleton.WallTiles);
        unitSpawner = new UnitSpawner();
        tileFormPlacer = new TileFormPlacer(GameSettings.Singleton.PatternTileMap, GameSettings.Singleton.tileMap, MapGenerator);
        Pathfinding = new NormalPathfinding(map);
        _random = new System.Random();

        AddPlayerCharacter(GameObject.Find("Character").GetComponent<Unit>());
    }
    public static void AddPlayerCharacter(Unit Character)
    {
        PlayerRelatedCharacters.Add(Character);
        PlayerCharactersPositions.Add(Character.transform.position);
    }
}
