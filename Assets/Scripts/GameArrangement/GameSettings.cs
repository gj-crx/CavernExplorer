using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Generation;

public class GameSettings : MonoBehaviour
{
    public MapGenerator1.GeneratorSettings MapGeneratorSettings;
    public UnitsSpawningSettings unitsSpawningSettings;
    public Tilemap tileMap;
    public Tilemap PatternTileMap;
    public RuleTile[] WallTiles;

    public float UnitMovementTileSkipTreshhold = 1;


    public short PathfindingMaxSearchDistance = 250;

    public string TileNameToRemove;
    public float FPS = 0;

    [HideInInspector]
    public static GameSettings Singleton;


    private void Awake()
    {
        Singleton = this;
    }
    private void Start()
    {
        Application.targetFrameRate = 60;
        GameManager.InitializeGame();
    }
    private void Update()
    {
        GameManager.SetPlayersPositions();
        FPS = 1.0f / Time.deltaTime;

        if (GameManager.MapGenerator.ToGenerateOrder)
        {
            GameManager.MapGenerator.ToGenerateOrder = false;
            GameManager.MapGenerator.SpawnAllTiles_MainThread(tileMap, WallTiles[0]);
        }
    }
    public void DebugActionExecute()
    {
        GameManager.MapGenerator.GenerateMap();
    }

    [System.Serializable]
    public struct UnitsSpawningSettings
    {
        /// <summary>
        /// range 0-100%
        /// </summary>
        public int CreepSpawnChance;
    }
    private void OnApplicationQuit()
    {
        GameManager.GameIsRunning = false;
    }
}
