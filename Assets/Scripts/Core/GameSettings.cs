using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Generation;

public class GameSettings : MonoBehaviour
{
    public GeneratorSettings[] GeneratorSettingsPerLevels;

    public PlayerCharacterStartingAssets StartingCharacterAsset;

    public float UnitMovementTileSkipTreshhold = 1;
    public short PathfindingMaxSearchDistance = 250;

    public bool RegenerateMapOrder = false;
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
        FPS = 1.0f / Time.deltaTime;

        if (GameManager.MapGenerator.ToGenerateOrder) StartGenerationProcess();
        if (RegenerateMapOrder) StartRegenerationProcess();
    }

    private void StartGenerationProcess()
    {
        GameManager.MapGenerator.SpawnAllTiles_MainThread(PrefabManager.Singleton.UnpassableTilemap, PrefabManager.Singleton.FloorsTilemap, PrefabManager.Singleton.LevelGatesTilemap);
        GameManager.MapGenerator.ToGenerateOrder = false;
    }
    private void StartRegenerationProcess()
    {
        RegenerateMapOrder = false;
        GameManager.MapGenerator.GenerateMap(0);
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
        [Header("~ Main settings")]
        public int Seed;

        public bool ContiniousGeneration;

        public byte PointsPerSector;
        public byte SectorRadius;
        public byte UpperWayGatesCount;
        public byte DownWayGatesCount;

        [Header("~ Link parameters")]
        public byte StartingSectorsCreationRadius;

        public float LinkDirectionRandomizationChance;
        public byte LinkNormalWidth;
        public int LinkWidthRandomAdjustment;

        [Header("~ Visual properties")]
        public Color unwalkableLayerColor;
        public Color floorLayerColor;

        [Header("~ Creep spawning and structures")]
        public List<DungeonGenerationSettings> dungeonsToGenerate;
        public UnitSpawningPattern unitSpawningPatterns; 
    }
}
