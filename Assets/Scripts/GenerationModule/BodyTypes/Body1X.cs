using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class Body1X : BodyType
    {
        public byte BodySize
        {
            get { return 1; }
        }

        public bool CheckBodyForm(Vector2Int positionToCheck, Func<Vector2Int, bool> passablePathChecking)
        {
            return passablePathChecking(positionToCheck);
        }
    }
}