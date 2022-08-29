using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public interface IPathfinding
    {
        bool GetWayPath(Unit MovingUnit, Vector3 Target, byte MaximumCorrectionStep);
        bool GetPathBetweenPoints(Vector3 From, Vector3 Target);
    }
