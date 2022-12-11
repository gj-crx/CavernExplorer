using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class Body2X : BodyType
    {
        public byte BodyRadius
        {
            get { return 1; }
        }

        public bool CheckBodyForm(Vector2Int positionToCheck, Func<Vector2Int, bool> passablePathChecking)
        {
            if (passablePathChecking(positionToCheck + new Vector2Int(0, 0)) == false) return false;
            if (passablePathChecking(positionToCheck + new Vector2Int(0, 1)) == false) return false;
            return true;
        }
    }
}