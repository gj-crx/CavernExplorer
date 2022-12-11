using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

//namespace
//{
public class NormalPathfinding : IPathfinding
{


    public Map map;

    private DistanceMapHolder distancesMap;
    private BodyType currentBodyType = null;
    private short currentDistance = 0;
    private Vector2Int[] way = null;

    private Stack<Vector2Int>[] toCheck;
    private short maxSearchDistance = 150;
    private bool boolCurrentStackTurn = false;
    private int currentStackTurn
    {
        get
        {
            return BoolToInt(boolCurrentStackTurn);
        }
    }

    public NormalPathfinding(Map m)
    {
        map = m;
        distancesMap = new DistanceMapHolder();
    }
    public bool GetWayPath(Unit MovingUnit, Vector3 Target, BodyType bodyType, byte MaximumCorrectionStep = 2)
    {
        currentBodyType = bodyType;
        Vector2Int from = RoundVector3(MovingUnit.LastNonTransformPosition);
        if (currentBodyType.CheckBodyForm(from, PassablePath) == false)
        {
            var Result = CorrectPath(from, bodyType.BodyRadius);
            if (Result.Item2 == true) from = Result.Item1; //path resets to correct one
            else return false;
        }
        Vector2Int target = Vector3ToVector2Int(Target);
        if (currentBodyType.CheckBodyForm(target, PassablePath) == false)
        {
            var Result = CorrectPath(target, bodyType.BodyRadius);
            if (Result.Item2 == true) target = Result.Item1; //path resets to correct one
            else return false;
        }

        bool result = CalculateWay(from, target, bodyType);
        if (result)
        {
            //   MovingUnit.unitMovement.Way = BasicFunctions.ConvertToVector3Array(Way, 0.5f);
            MovingUnit.unitMovement.Way = BasicFunctions.ConvertToVector3Array(way, 0);
            MovingUnit.unitMovement.CurrentDistance = 1;
        }
        return result;
    }
    public bool GetPathBetweenPoints(Vector3 From, Vector3 Target, BodyType bodyType)
    {
        currentBodyType = bodyType;
        Vector2Int from = Vector3ToVector2Int(Target);
        if (currentBodyType.CheckBodyForm(from, PassablePath) == false)
        {
            var Result = CorrectPath(from, bodyType.BodyRadius);
            if (Result.Item2 == true) from = Result.Item1; //path resets to correct one
            else return false;
        }
        Vector2Int target = Vector3ToVector2Int(Target);
        if (currentBodyType.CheckBodyForm(target, PassablePath) == false)
        {
            var Result = CorrectPath(target, bodyType.BodyRadius);
            if (Result.Item2 == true) target = Result.Item1; //path resets to correct one
            else return false;
        }
        return CalculateWay(from, target, bodyType);
    }

    private bool CalculateWay(Vector2Int From, Vector2Int Target, BodyType bodyType)
    {
        distancesMap = new DistanceMapHolder();
        currentDistance = 0;
        toCheck = new Stack<Vector2Int>[2];
        toCheck[0] = new Stack<Vector2Int>();
        toCheck[1] = new Stack<Vector2Int>();
        boolCurrentStackTurn = false;
        toCheck[currentStackTurn].Push(From);

        bool found = false;
        while (found == false && currentDistance < maxSearchDistance)
        {
            found = IterateToCheckList(Target);
        }
       // Debug.Log(found + " distance " + CurrentDistance + " out of " + MaxSearchDistance);
        RestoreWay(Target, From);
        return found;
    }
    private bool IterateToCheckList(Vector2Int Target)
    {
        foreach (var CurrentPoint in toCheck[currentStackTurn])
        {
            if (CurrentPoint == Target)
            { //Target is found
                return true;
            }
            else
            {
                distancesMap[CurrentPoint.x, CurrentPoint.y] = new DistanceMapPoint(currentDistance); //setting this point to distance map
                GetNeighbours(CurrentPoint, BoolToInt(!boolCurrentStackTurn)); //adding neighbour patches to next stack
            }
        }
        toCheck[currentStackTurn].Clear();
        currentDistance++;
        boolCurrentStackTurn = !boolCurrentStackTurn;
        return false;
    }
    private void GetNeighbours(Vector2Int Point, int StackToAdd)
    {
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                Vector2Int current = new Vector2Int(Point.x + x, Point.y + y);
                if ((x == 0 || y == 0) && (x != 0 || y != 0) && ValidPath(current))
                {
                    toCheck[StackToAdd].Push(current);
                }
            }
        }
    }
    private Vector2Int GetPartOfReturningWay(Vector2Int CurrentPoint, Vector2Int deltaToTargetNormalized)
    {
        short CurrentMinDistance = maxSearchDistance;
        Vector2Int MinDistancePath = Vector2Int.zero;
        int x = deltaToTargetNormalized.x;
        int y = deltaToTargetNormalized.y;
        if (Mathf.Abs(deltaToTargetNormalized.x) > Mathf.Abs(deltaToTargetNormalized.y)) y = 0;
        else x = 0;
        for (int yTries = 0; yTries < 3; yTries++)
        {
            for (int xTries = 0; xTries < 3; xTries++)
            {
                //  Debug.Log("path " + new Vector2Int(CurrentPoint.x, CurrentPoint.y) + " distance: " + DistancesMap[CurrentPoint.x, CurrentPoint.y]);
                if ((x == 0 || y == 0) && (x != 0 || y != 0))
                if (distancesMap[CurrentPoint.x + x, CurrentPoint.y + y] != null && distancesMap[CurrentPoint.x + x, CurrentPoint.y + y].Distance < CurrentMinDistance)
                {
                    CurrentMinDistance = distancesMap[CurrentPoint.x + x, CurrentPoint.y + y].Distance;
                    MinDistancePath = new Vector2Int(CurrentPoint.x + x, CurrentPoint.y + y);
                    //  Debug.Log("Minimal distance out of  " + CurrentPoint + " is "  + MinimalDistancePath + " : " + MinimalDistance);
                }
                x = GetNextCordToTry(x, CurrentPoint.x > 0);
            }
            x = 0;
            y = GetNextCordToTry(y, CurrentPoint.y > 0);
        }
        return MinDistancePath;
    }
    private void RestoreWay(Vector2Int SearchStartingPosition, Vector2Int SearchEndingPosition)
    {
        Vector2Int normalizedDelta = BasicFunctions.NormalizeVector2Int(SearchEndingPosition - SearchStartingPosition);
        way = new Vector2Int[currentDistance + 1];
        way[currentDistance] = SearchStartingPosition;
        way[0] = SearchEndingPosition;
        for (int i = currentDistance - 1; i >= 1; i--)
        {
            way[i] = GetPartOfReturningWay(way[i + 1], normalizedDelta);
        }
    }
    private int BoolToInt(bool b)
    {
        if (b) return 1;
        else return 0;
    }
    private Vector2Int Vector3ToVector2Int(Vector3 v)
    {
        return new Vector2Int((int)v.x, (int)v.y);
    }

    private bool ValidPath(Vector2Int PathToCheck)
    {
        return currentBodyType.CheckBodyForm(PathToCheck, PassablePath) && toCheck[0].Contains(PathToCheck) == false && toCheck[1].Contains(PathToCheck) == false && distancesMap[PathToCheck.x, PathToCheck.y] == null;
    }
    private bool PassablePath(Vector2Int PathToCheck)
    {
        return map.LandscapeMap[PathToCheck.x, PathToCheck.y] != null && 
            (map.LandscapeMap[PathToCheck.x, PathToCheck.y].Land == LandType.Passable || map.LandscapeMap[PathToCheck.x, PathToCheck.y].Land == LandType.WaterLow) ;
    }
    private Tuple<Vector2Int, bool> CorrectPath(Vector2Int Path, int bodyRadius)
    {
        int x = 0;
        int y = 0;
        for (int cycle = 1; cycle <= bodyRadius; cycle++)
        {
            for (int yTries = 0; yTries < 3; yTries++) //trying each cord 3 times (-1, 0, 1) in a semi-random pattern starting from 0
            {
                for (int xTries = 0; xTries < 3; xTries++)
                {
                    if ((x == 0 || y == 0) && currentBodyType.CheckBodyForm(Path + new Vector2Int(x, y), PassablePath)) return Tuple.Create(Path + new Vector2Int(x, y), true);

                    x = GetNextCordToTry(x, Path.x > 0) * cycle;
                }
                x = 0;
                y = GetNextCordToTry(y, Path.y > 0) * cycle;
            }
            y = 0;
        }
        return Tuple.Create(Vector2Int.zero, false);
    }
    private int GetNextCordToTry(int currentCord, bool falseRandomMethod)
    {
        if (currentCord == 0)
        {
            if (falseRandomMethod) return 1;
            else return -1;
        }
        if (falseRandomMethod)
        {
            if (currentCord > 0) return -1;
            else return 0;
        }
        else
        {
            if (currentCord > 0) return 0;
            else return 1;
        }
    }
    private Vector2Int RoundVector3(Vector3 pos)
    {
        if (pos.x < 0)
        {
            if (pos.x - (int)pos.x < -0.5) pos.x = (int)pos.x - 1;
        }
        else if (pos.x - (int)pos.x > 0.5) pos.x = (int)pos.x + 1;

        if (pos.y < 0)
        {
            if (pos.y - (int)pos.y < -0.5) pos.y = (int)pos.y - 1;
        }
        else if (pos.y - (int)pos.y > 0.5f) pos.y = (int)pos.y + 1;

        return BasicFunctions.ToVector2Int(pos);
    }
}
internal class DistanceMapPoint
{
    public short Distance { get; set; } = 0;
    public DistanceMapPoint(short Distance)
    {
        this.Distance = Distance;
    }
}
internal class DistanceMapHolder
{
    Dictionary<Tuple<int, int>, DistanceMapPoint> DistanceMapDictionary = new Dictionary<Tuple<int, int>, DistanceMapPoint>();


    public DistanceMapPoint this[int x, int y]
    {
        get
        {
            var t = Tuple.Create(x, y);
            if (DistanceMapDictionary.ContainsKey(t)) return DistanceMapDictionary[t];
            return null;
        }
        set
        {
            var t = Tuple.Create(x, y);
            DistanceMapDictionary[t] = value;
        }
    }
}

//}