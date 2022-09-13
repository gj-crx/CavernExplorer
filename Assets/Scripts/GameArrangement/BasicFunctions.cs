using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BasicFunctions
{
    public static Vector3 Vector2IntToVector3(Vector2Int Vector)
    {
        return new Vector3(Vector.x, Vector.y);
    }
    public static Vector3Int Vector2IntToVector3Int(Vector2Int Vector)
    {
        return new Vector3Int(Vector.x, Vector.y, 0);
    }
    public static Vector2Int Vector3ToVector2Int(Vector3 v)
    {
        return new Vector2Int((int)v.x, (int)v.y);
    }
    public static Vector2Int GetDirectionBetween2Points(Vector2Int From, Vector2Int To)
    {
        Vector2Int Delta = To - From;
        bool XAxisPriority = Convert.ToBoolean(GameManager._random.Next(0, 2));
        //if x == 0 or y == 0 then return immidiatly, direction is clear
        if (Delta.x == 0 || Delta.y == 0)
        {
            return NormalizeVector2Int(Delta);
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
    public static Vector2Int GetDirectionBetween2Points(Vector2Int From, Vector2Int To, bool XAxisPriority)
    { 
        Vector2Int Delta = To - From;
        //if x == 0 or y == 0 then return immidiatly, direction is clear
        if (Delta.x == 0 || Delta.y == 0)
        { 
            return NormalizeVector2Int(Delta); 
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
    public static Vector2Int NormalizeVector2Int(Vector2Int v)
    {
        if (v.x > 0) v.x = 1;
        if (v.x < 0) v.x = -1;

        if (v.y > 0) v.y = 1;
        if (v.y < 0) v.y = -1;

        return v;
    }
    public static Vector3[] ConvertToVector3Array(Vector2Int[] SourceArray, float AddictiveValue = 0)
    {
        Vector3[] NewArray = new Vector3[SourceArray.Length];
        for (int i = 0; i < SourceArray.Length; i++)
        {
            NewArray[i] = Vector2IntToVector3(SourceArray[i]);
            NewArray[i] += new Vector3(AddictiveValue, AddictiveValue);
        }
        return NewArray;
    }
    public static bool PassableTile(Vector2Int TilePosition, Map ReferenceMap)
    {
        return ReferenceMap.LandscapeMap[TilePosition.x, TilePosition.y].Land == LandType.Passable || ReferenceMap.LandscapeMap[TilePosition.x, TilePosition.y].Land == LandType.WaterLow;
    }
    public static Vector3 GetPlayerTransformPositionFromMainthread(Unit MentionedUnit)
    {
        return GameManager.PlayerCharactersPositions[GameManager.PlayerRelatedCharacters.IndexOf(MentionedUnit)];
    }
}
