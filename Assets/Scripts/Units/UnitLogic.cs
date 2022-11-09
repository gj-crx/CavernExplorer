using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
        foreach (Unit _unit in CheckRange)
        {
            if (_unit != null)
            {
                float CurrentDistance = Vector3.Distance(_unit.transform.position, pos);
                if (CurrentDistance < MinDistance)
                {
                    MinDistance = CurrentDistance;
                    NearestObject = _unit;
                }
            }
        }
        return NearestObject;
    }
    public static bool CheckPossibleBuildingSpot(Vector3 pos, GameObject PrefabToCheck, int radius = 12)
    {
        NavMeshPath path = new NavMeshPath();
        pos = new Vector3(pos.x, PrefabToCheck.transform.position.y, pos.z);
        bool result = true;
        for (int z = -radius; z <= radius; z += 2)
        {
            for (int x = -radius; x <= radius; x += 2)
            {
                result = NavMesh.CalculatePath(new Vector3(PrefabToCheck.transform.position.x, 0, PrefabToCheck.transform.position.z), pos + new Vector3(x, 0, z), NavMesh.AllAreas, path);
                if (result == false) return false;
            }
        }
        return result;
    }


}
