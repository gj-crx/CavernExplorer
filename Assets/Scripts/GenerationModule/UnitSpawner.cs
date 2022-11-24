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
                if (UnitsToSpawn.Count > 0 && GameManager.MapGenerator.GenerationCompleted == true)
                {
                    var CurrentUnitToSpawn = UnitsToSpawn.Pop();
                    GameObject.Instantiate(CurrentUnitToSpawn.Item1, CurrentUnitToSpawn.Item2, Quaternion.identity);
                    yield return null;
                }
                else yield return new WaitForSeconds(0.6f);
                
            }
        }

        public void SpawnUnitsInSector(Sector ReferenceSector, System.Random random, UnitSpawningPattern spawningPattern, int spawningDispersionRate = 2)
        {
            for (int unitToSpawnID = 0; unitToSpawnID < spawningPattern.possibleUnitsToSpawn.Length; unitToSpawnID++)
            {
                for (int i = 0; i < spawningPattern.estimatedUnitsPerSector[unitToSpawnID] * spawningDispersionRate; i++)
                {
                    if (random.Next(0, spawningDispersionRate) == 0) 
                        UnitsToSpawn.Push(new Tuple<GameObject, Vector3>(spawningPattern.possibleUnitsToSpawn[unitToSpawnID], BasicFunctions.ToVector3(ReferenceSector.RandomPoint)));
                }
            }
        }
    }
}
