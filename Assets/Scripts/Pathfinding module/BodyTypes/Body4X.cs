using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class Body4X : BodyType
    {
        public byte BodyRadius
        {
            get { return 1; }
        }

        public bool CheckBodyForm(Vector2Int positionToCheck, Func<Vector2Int, bool> passablePathChecking)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if (x == 0 || y == 0)
                    {
                        if (passablePathChecking(positionToCheck + new Vector2Int(x, y)) == false) return false;
                    }
                }
            }
            return true;
        }
    }
}
