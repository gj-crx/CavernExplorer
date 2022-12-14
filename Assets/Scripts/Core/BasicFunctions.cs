using System;
using System.Collections.Generic;
using UnityEngine;

public static class BasicFunctions
{
    public static Vector3 ToVector3(Vector2Int Vector, float ZCord = 0)
    {
        return new Vector3(Vector.x, Vector.y, ZCord);
    }
    public static Vector3Int ToVector3Int(Vector2Int Vector)
    {
        return new Vector3Int(Vector.x, Vector.y, 0);
    }
    public static Vector3Int ToVector3Int(Vector3 Vector)
    {
        return new Vector3Int((int)Vector.x, (int)Vector.y, (int)Vector.z);
    }
    public static Vector2Int ToVector2Int(Vector3 v)
    {
        return new Vector2Int((int)v.x, (int)v.y);
    }
    public static Vector2Int ToVector2Int(Vector3Int v)
    {
        return new Vector2Int(v.x, v.y);
    }
    public static Vector2Int GetNormalizedDirectionBetween2Points(Vector2Int From, Vector2Int To)
    {
        Vector2Int delta = To - From;
        bool XAxisPriority = Convert.ToBoolean(GameManager.GenRandom.Next(0, 2));
        //if x == 0 or y == 0 then return immidiatly, direction is clear
        if (delta.x == 0 || delta.y == 0)
        {
            return ToUnitVector(delta);
        }
        //if x == 1 and y == 1 then axis priority decides which x or y to be 0 and returns
        if (XAxisPriority)
        {
            if (delta.x > 0) return new Vector2Int(1, 0);
            else return new Vector2Int(-1, 0);
        }
        else
        {
            if (delta.y > 0) return new Vector2Int(0, 1);
            else return new Vector2Int(0, -1);
        }
    }
    public static Vector2Int NormalizeVector2Int(Vector2Int referenceVector)
    {
        if (referenceVector.x > 0) referenceVector.x = 1;
        if (referenceVector.y > 0) referenceVector.y = 1;
        if (referenceVector.x < 0) referenceVector.x = -1;
        if (referenceVector.y < 0) referenceVector.y = -1;
        return referenceVector;
    }
    public static Vector2Int GetRandomizedDirection(Vector2Int From, Vector2Int To)
    {
        Vector2Int randomizedDirection = To - From;
        if (Mathf.Abs(randomizedDirection.x) > 0)
        {
            randomizedDirection.x = 0;
            if (GameManager.GenRandom.Next(0, 2) == 1) randomizedDirection.y = 1;
            else randomizedDirection.y = -1;
        }
        else
        {
            randomizedDirection.y = 0;
            if (GameManager.GenRandom.Next(0, 2) == 1) randomizedDirection.x = 1;
            else randomizedDirection.x = -1;
        }
        return randomizedDirection;
    }
    public static Vector3 GetDirectionBetween2Points(Vector3 From, Vector3 To)
    {
        Vector3 delta = To - From;
        float cordSum = Math.Abs(delta.x) + Math.Abs(delta.y);
        delta.x /= cordSum;
        delta.y /= cordSum;
        return delta;
        
    }
    public static Vector2Int GetDirectionBetween2Points(Vector2Int From, Vector2Int To, bool XAxisPriority)
    { 
        Vector2Int Delta = To - From;
        //if x == 0 or y == 0 then return immidiatly, direction is clear
        if (Delta.x == 0 || Delta.y == 0)
        { 
            return ToUnitVector(Delta); 
        }
        //if x == 1 and y == 1 then axis priority decides which x or y to be 0 and returns
        if (XAxisPriority)
        {
            if (Delta.x > 0) return new Vector2Int(1, 0);
            else return new Vector2Int(-1, 0);
        }
        else
        {
            if (Delta.y > 0) return new Vector2Int(0, 1);
            else return new Vector2Int(0, -1);
        }
    }
    public static Vector2Int NumberToOffsetPosition(int SerialNumber)
    {
        switch (SerialNumber)
        {
            case 0: return new Vector2Int(-1, -1);
            case 1: return new Vector2Int(0, -1);
            case 2: return new Vector2Int(1, -1);

            case 3: return new Vector2Int(-1, 0);
            // case 4: return new Vector2Int(0, 0);
            case 4: return new Vector2Int(1, 0);

            case 5: return new Vector2Int(-1, 1);
            case 6: return new Vector2Int(0, 1);
            case 7: return new Vector2Int(1, 1);
        }
        return Vector2Int.zero;
    }
    public static bool ArrayOfBoolEquals(bool[] b1, bool[] b2)
    {
        if (b1.Length != b2.Length) return false;
        for (int i = 0; i < b1.Length; i++)
        {
            if (b1[i] != b2[i]) return false;
        }
        return true;
    }
    public static Vector2Int ToUnitVector(Vector2Int v)
    {
        if (v.x > 0) v.x = 1;
        if (v.x < 0) v.x = -1;

        if (v.y > 0) v.y = 1;
        if (v.y < 0) v.y = -1;

        return v;
    }
    public static Vector3Int ToUnitVector(Vector3 v)
    {
        if (v.x > 0) v.x = 1;
        if (v.x < 0) v.x = -1;

        if (v.y > 0) v.y = 1;
        if (v.y < 0) v.y = -1;

        return ToVector3Int(v);
    }
    public static Vector3[] ConvertToVector3Array(Vector2Int[] SourceArray, float AddictiveValue = 0)
    {
        Vector3[] NewArray = new Vector3[SourceArray.Length];
        for (int i = 0; i < SourceArray.Length; i++)
        {
            NewArray[i] = ToVector3(SourceArray[i]) + new Vector3(AddictiveValue, AddictiveValue);
        }
        return NewArray;
    }
    public static bool PassableTile(Vector2Int TilePosition, Map ReferenceMap)
    {
        return ReferenceMap.LandscapeMap[TilePosition.x, TilePosition.y].Land == LandType.Passable || ReferenceMap.LandscapeMap[TilePosition.x, TilePosition.y].Land == LandType.WaterLow;
    }

    public static Vector2 AngleToVector2(float degree)
    {
        return new Vector2(Mathf.Cos(degree * Mathf.Deg2Rad), Mathf.Sin(degree * Mathf.Deg2Rad));
    }
    public static float DirectionToAngle(Vector3 Direction)
    {
        if (Mathf.Abs(Direction.x) > Mathf.Abs(Direction.y))
        {
            if (Direction.x > 0) return -90;
            else if (Direction.x < 0) return 90;
        }
        else
        {
            if (Direction.y > 0) return 0;
            else if (Direction.y < 0) return 180;
        }
        return 0;
    }
    public static Vector3 RemoveZCord(Vector3 referreceVector3, float zCord = 0)
    {
        return new Vector3(referreceVector3.x, referreceVector3.y, zCord);
    }
    public static Vector2Int ReverseDirection(Vector2Int direction)
    {
        if (direction.x != 0) return new Vector2Int(0, direction.x);
        if (direction.y != 0) return new Vector2Int(direction.y, 0);
        return Vector2Int.zero;
    }
    public static Vector3Int ReverseDirection(Vector2Int direction, bool vector3Output)
    {
        if (direction.x != 0) return new Vector3Int(0, direction.x);
        if (direction.y != 0) return new Vector3Int(direction.y, 0);
        return Vector3Int.zero;
    }
}
