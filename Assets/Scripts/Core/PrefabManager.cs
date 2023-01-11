using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PrefabManager : MonoBehaviour
{
    [Header("~Tilemaps")]
    public Tilemap UnpassableTilemap;
    public Tilemap FloorsTilemap;
    public Tilemap LevelGatesTilemap;

    [Header("~Environmental prefabs")]
    public GameObject[] WallPrefabs = new GameObject[10];
    public List<GameObject> CreepPrefabs = new List<GameObject>();
    public List<TileBase> DungeonWallPrefabs = new List<TileBase>();

    [Header("~Tile prefabs")]
    public RuleTile[] WallTiles;
    public Tile[] FloorTiles;
    public Tile[] UpLevelGateTiles;
    public Tile[] DownLevelGateTiles;

    [Header("~UI prefabs")]
    public GameObject ItemPrefab = null;

    [Header("~Icons")]
    public Sprite[] Icons = new Sprite[10];

    [HideInInspector]
    public static PrefabManager Singleton;

    private void Awake()
    {
        Singleton = this;
    }
}
