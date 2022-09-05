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


    public short PathfindingMaxSearchDistance = 250;

    public string TileNameToRemove;

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

    }
    public void DebugActionExecute()
    {
        GameManager.MapGenerator.GenerateMap();
    }

    [System.Serializable]
    public struct UnitsSpawningSettings
    {
        public float CreepSpawnChance;
    }
    private void OnApplicationQuit()
    {
        GameManager.GameIsRunning = false;
    }
}
