using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generation {
    [CreateAssetMenu(fileName = "SpawningPattern", menuName = "ScriptableObjects/Generation/UnitSpawningPattern", order = 1)]
    public class UnitSpawningPattern : ScriptableObject
    {
        public GameObject[] possibleUnitsToSpawn = new GameObject[1];
        public int[] estimatedUnitsPer4Sectors = new int[1];


    }
}
