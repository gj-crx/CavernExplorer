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

    public static System.Random GenRandom;
    public static System.Random Random;
    public static System.Threading.Thread MainThread;

    private static UnitController unitController;
    public static Unit LocalPlayerHeroUnit;



    public static bool DebugMode = true;

    public static void InitializeGame()
    {
        Application.targetFrameRate = 60;

        map = new Map();
        unitSpawner = new UnitSpawner();
        MapGenerator = new MapGenerator1(GameSettings.Singleton.GeneratorSettingsPerLevels[1], map);
        tileFormPlacer = new TileFormPlacer(GameSettings.Singleton.UnpassableTilemap, MapGenerator);
        Pathfinding = new NormalPathfinding(map);

        for (int i = 0; i < GameSettings.Singleton.GeneratorSettingsPerLevels.Length; i++)
        {
            if (GameSettings.Singleton.GeneratorSettingsPerLevels[i].Seed == 0) GameSettings.Singleton.GeneratorSettingsPerLevels[i].Seed = UnityEngine.Random.Range(0, int.MaxValue);
            GenRandom = new System.Random(GameSettings.Singleton.GeneratorSettingsPerLevels[i].Seed);
        }
        Random = new System.Random();

        MainThread = System.Threading.Thread.CurrentThread;

        unitController = new UnitController();

        LocalPlayerHeroUnit = GameObject.Find("Character").GetComponent<Unit>();
    }
}
