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
    public Sprite UselessTileSprite;

    public PlayerCharacterStartingAssets startingCharacterAsset;

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
        GameManager.InitializeGame();
    }
    private void Update()
    {
        GameManager.SetPlayersPositions();
        FPS = 1.0f / Time.deltaTime;

        if (GameManager.MapGenerator.ToGenerateOrder)
        {
            GameManager.MapGenerator.SpawnAllTiles_MainThread(tileMap, WallTiles[0]);
            GameManager.MapGenerator.ToGenerateOrder = false;
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
    [System.Serializable]
    public struct PlayerCharacterStartingAssets
    {
        public string AssetName;
        public List<Items.Item> StartingItems;

    }
    private void OnApplicationQuit()
    {
        GameManager.GameIsRunning = false;
    }
}