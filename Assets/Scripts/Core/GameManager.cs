using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Controllers;
using Generation;

public static class GameManager
{
    public static bool GameIsRunning = true;

    public static DataBase dataBase = new DataBase();
    public static Map map;
    public static MapGenerator1 MapGenerator;
    public static UnitSpawner unitSpawner;
    public static TileFormPlacer tileFormPlacer;
    public static IPathfinding Pathfinding;

    public static System.Random random;
    public static System.Threading.Thread MainThread;

    private static UnitController unitController;


    public static List<Unit> PlayerRelatedCharacters = new List<Unit>();
    public static List<Vector3> PlayerCharactersPositions = new List<Vector3>();
    public static Unit LocalPlayerHeroUnit;



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
        Application.targetFrameRate = 60;

        map = new Map();
        unitSpawner = new UnitSpawner();
        MapGenerator = new MapGenerator1(GameSettings.Singleton.MapGeneratorSettings, map, GameSettings.Singleton.tileMap, GameSettings.Singleton.WallTiles);
        tileFormPlacer = new TileFormPlacer(GameSettings.Singleton.tileMap, MapGenerator);
        Pathfinding = new NormalPathfinding(map);

        random = new System.Random();
        MainThread = System.Threading.Thread.CurrentThread;

        unitController = new UnitController();

        AddPlayerCharacter(GameObject.Find("Character").GetComponent<Unit>());
    }
    public static void AddPlayerCharacter(Unit Character)
    {
        PlayerRelatedCharacters.Add(Character);
        PlayerCharactersPositions.Add(Character.transform.position);
        LocalPlayerHeroUnit = Character;
    }
}
