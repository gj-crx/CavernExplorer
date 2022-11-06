using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generation
{
    public class UnitSpawner
    {
        public Stack<Tuple<GameObject, Vector3>> UnitsToSpawn = new Stack<Tuple<GameObject, Vector3>>();
        public Unit SpawnUnit(Vector3 Position, GameObject Prefab)
        {
            Unit NewUnit = GameObject.Instantiate(Prefab, Position, Quaternion.identity).GetComponent<Unit>();

            return NewUnit;
        }
        public IEnumerator IterateUnitSpawningQueue()
        {
            yield return new WaitForSeconds(0.6f);
            while (GameManager.GameIsRunning)
            {
                Debug.Log(UnitsToSpawn.Count + " " + GameManager.MapGenerator.GenerationCompleted);
                if (UnitsToSpawn.Count > 0 && GameManager.MapGenerator.GenerationCompleted == true)
                {
                    var CurrentUnitToSpawn = UnitsToSpawn.Pop();
                    GameObject.Instantiate(CurrentUnitToSpawn.Item1, CurrentUnitToSpawn.Item2, Quaternion.identity);
                    yield return null;
                }
                else yield return new WaitForSeconds(0.6f);
                
            }
        }

        public void SpawnUnitsInSector(Sector ReferenceSector, System.Random random)
        {
            Debug.Log(GameManager.MapGenerator.CurrentGenSettings.CreepSpawningChance);
            while (random.Next(0, 100) < GameManager.MapGenerator.CurrentGenSettings.CreepSpawningChance)
            {
               UnitsToSpawn.Push(new Tuple<GameObject, Vector3>(PrefabManager.Singleton.CreepPrefabs[0], BasicFunctions.ToVector3(ReferenceSector.RandomPoint)));
            }
        }
    }
}