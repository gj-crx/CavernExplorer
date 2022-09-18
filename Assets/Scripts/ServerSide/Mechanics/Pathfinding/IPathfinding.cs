using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerSideLogic
{
    public interface IPathfinding
    {
        List<Vector2Int> GetLastWay();
        bool GetWayPath(Unit MovingUnit, Vector3 Target, byte MaximumCorrectionStep);
        bool GetPathBetweenPoints(Vector3 From, Vector3 Target);
        Vector3 Vector2IntToVector3(Vector2Int vector2Int, float y);
    }
}
