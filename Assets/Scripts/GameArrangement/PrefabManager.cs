using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    [Header("Environmental prefabs")]
    public GameObject[] WallPrefabs = new GameObject[10];
    public List<GameObject> CreepPrefabs = new List<GameObject>();

    [Header("UI prefabs")]
    public GameObject ItemPrefab = null;

    [Header("Icons")]
    public Sprite[] Icons = new Sprite[10];

    [HideInInspector]
    public static PrefabManager Singleton;

    private void Awake()
    {
        Singleton = this;
    }
}
