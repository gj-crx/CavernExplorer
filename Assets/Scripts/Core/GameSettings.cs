using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Generation;

public class GameSettings : MonoBehaviour
{
    public GeneratorSettings[] GeneratorSettingsPerLevels;
    public Tilemap unpassableTilemap;
    public Tilemap passableTilemap;
    public Sprite UselessTileSprite;

    public PlayerCharacterStartingAssets startingCharacterAsset;

    public RuleTile[] WallTiles;
    public Tile[] FloorTiles;

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
            GameManager.MapGenerator.SpawnAllTiles_MainThread(unpassableTilemap, passableTilemap);
            GameManager.MapGenerator.ToGenerateOrder = false;
        }
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

    [System.Serializable]
    public struct GeneratorSettings
    {
        public int Seed;

        public bool ContiniousGeneration;

        public byte PointsPerSector;
        public byte SectorRadius;
        public byte UpperWayGatesCount;
        public byte DownWayGatesCount;

        public byte StartingSectorsCreationRadius;
        public float CreepSpawningChance;
    }
}
