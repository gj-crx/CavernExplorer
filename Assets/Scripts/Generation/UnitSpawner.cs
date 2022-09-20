using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generation
{
    public class UnitSpawner
    {

        public Unit SpawnUnit(Vector3 Position, GameObject Prefab)
        {
            Unit NewUnit = GameObject.Instantiate(Prefab, Position, Quaternion.identity).GetComponent<Unit>();

            return NewUnit;
        }

        public void SpawnUnitsInSector(MapGenerator1.Sector ReferenceSector)
        {
            while (Random.Range(0.0f, 1.0f) < GameSettings.Singleton.unitsSpawningSettings.CreepSpawnChance)
            {
                SpawnUnit(BasicFunctions.ToVector3(ReferenceSector.RandomPoint), PrefabManager.Singleton.CreepPrefabs[0]);
            }
        }
    }
}
