using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public interface BodyType
    {
        public byte BodyRadius { get; }
        public bool CheckBodyForm(Vector2Int positionToCheck, Func<Vector2Int, bool> passablePathChecking);

    }
}
