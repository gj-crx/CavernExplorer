using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

    public interface IPathfinding
    {
        bool GetWayPath(Unit MovingUnit, Vector3 Target, BodyType bodyType, byte MaximumCorrectionStep);
        bool GetPathBetweenPoints(Vector3 From, Vector3 Target, BodyType bodyType);
    }
