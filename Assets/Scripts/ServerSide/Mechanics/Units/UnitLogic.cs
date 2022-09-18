using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Types;

namespace ServerSideLogic
{
    public static class UnitLogic
    {
        public static Vector3 VectorToDirection(Vector3 v)
        {
            float Length = Mathf.Abs(v.x) + Mathf.Abs(v.y) + Mathf.Abs(v.z);
            if (Length == 0)
            {
                return new Vector3(0, 0, 0);
            }
            return new Vector3(v.x / Length, v.y / Length, v.z / Length);
        }
        public static Unit FindNearestObject(Vector3 pos, List<Unit> CheckRange, float MaximumSearchDistance = 100)
        {
            float MinDistance = MaximumSearchDistance;
            Unit NearestObject = null;
            foreach (Unit t in CheckRange)
            {
                if (t != null)
                {
                    float CurrentDistance = Vector3.Distance(t.position, pos);
                    if (CurrentDistance < MinDistance)
                    {
                        MinDistance = CurrentDistance;
                        NearestObject = t;
                    }
                }
            }
            return NearestObject;
        }
        public static bool CheckPossibleBuildingSpot(Vector3 pos, GameObject prefabToCheck, int radius = 12)
        {
            NavMeshPath path = new NavMeshPath();
            pos = new Vector3(pos.x, prefabToCheck.transform.position.y, pos.z);
            bool result = true;
            for (int z = -radius; z <= radius; z += 2)
            {
                for (int x = -radius; x <= radius; x += 2)
                {
                    result = NavMesh.CalculatePath(new Vector3(prefabToCheck.transform.position.x, 0, prefabToCheck.transform.position.z), pos + new Vector3(x, 0, z), NavMesh.AllAreas, path);
                    if (result == false) return false;
                }
            }
            return result;
        }
        public static bool IsPossibleToBuildAUnit(byte UnitTypeID, State BuilderState)
        {
            bool Possible = true;
            for (byte i = 0; i < TypesData.ResourceTypes.Count; i++)
            {
                if (BuilderState.ResourcesAmount[i] < TypesData.AllUnitTypes[UnitTypeID].Stats.ResourcesCostToBuild[i])
                { //insufficient amount of resources detected
                    Possible = false;
                    break;
                }
            }
            return Possible;
        }
        public static void SubstractResourcesForUnitCost(byte UnitTypeID, State BuilderState)
        {
            for (byte i = 0; i < TypesData.ResourceTypes.Count; i++)
            {
                BuilderState.ResourcesAmount[i] -= TypesData.AllUnitTypes[UnitTypeID].Stats.ResourcesCostToBuild[i];
            }
        }
    }
}
