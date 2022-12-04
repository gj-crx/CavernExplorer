using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class Body6X : BodyType
    {
        public byte BodySize
        {
            get { return 6; }
        }

        public bool CheckBodyForm(Vector2Int positionToCheck, Func<Vector2Int, bool> passablePathChecking)
        {
            Debug.LogError("not created");
            return false;
        }
    }
}
