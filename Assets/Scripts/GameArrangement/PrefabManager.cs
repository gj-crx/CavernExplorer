using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public GameObject[] WallPrefabs = new GameObject[10];
    public List<GameObject> CreepPrefabs = new List<GameObject>();

    [HideInInspector]
    public static PrefabManager Singleton;

    private void Awake()
    {
        Singleton = this;
    }
}
