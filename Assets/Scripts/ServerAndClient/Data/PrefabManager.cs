using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] prefabs_Units = new GameObject[15];
    public GameObject[] prefabs_Buildings = new GameObject[15];
    public GameObject[] prefabs_Trees = new GameObject[15];

  //  [HideInInspector]
    public GameObject[] prefabs_AllUnits = new GameObject[1000];


    [Header("Logical prefabs")]
    public GameObject prefab_State;
    public GameObject prefab_GameLobby;


    public static PrefabManager Singleton { get; private set; }

    private void Awake()
    {
        Singleton = this;
    }
}
