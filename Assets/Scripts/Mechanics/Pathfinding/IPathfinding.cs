using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public interface IPathfinding
    {
        List<Vector2Int> GetLastWay();
        bool GetWayPath(Unit MovingUnit, Vector3 Target, byte MaximumCorrectionStep);
        bool GetPathBetweenPoints(Vector3 From, Vector3 Target);
    }
