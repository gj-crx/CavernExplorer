using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//namespace
//{
public class NormalPathfinding : IPathfinding
{


    public Map map;
    public System.Random pathfindingRandom;
    private DistanceMapHolder DistancesMap;


    short CurrentDistance = 0;
    Vector2Int[] Way = null;

    Stack<Vector2Int>[] ToCheck;
    short MaxSearchDistance = 150;

    bool BoolCurrentStackTurn = false;
    int CurrentStackTurn
    {
        get
        {
            return BoolToInt(BoolCurrentStackTurn);
        }
    }

    public NormalPathfinding(Map m)
    {
        map = m;
        DistancesMap = new DistanceMapHolder();
        pathfindingRandom = new System.Random();
    }
    public bool GetWayPath(Unit MovingUnit, Vector3 Target, byte MaximumCorrectionStep = 2)
    {
        Vector2Int from = RoundVector3(MovingUnit.LastNonTransformPosition);
        if (PassablePath(from) == false)
        {
            var Result = CorrectPath(from);
            if (Result.Item2 == true) from = Result.Item1; //path resets to correct one
            else return false;
        }
        Vector2Int target = Vector3ToVector2Int(Target);
        if (PassablePath(target) == false)
        {
            var Result = CorrectPath(target);
            if (Result.Item2 == true) target = Result.Item1; //path resets to correct one
            else return false;
        }

        bool result = CalculateWay(from, target);
        if (result)
        {
            //   MovingUnit.unitMovement.Way = BasicFunctions.ConvertToVector3Array(Way, 0.5f);
            MovingUnit.unitMovement.Way = BasicFunctions.ConvertToVector3Array(Way, 0);
            MovingUnit.unitMovement.CurrentDistance = 1;
        }
        return result;
    }
    public bool GetPathBetweenPoints(Vector3 From, Vector3 Target)
    {
        Vector2Int from = Vector3ToVector2Int(Target);
        if (PassablePath(from) == false)
        {
            var Result = CorrectPath(from);
            if (Result.Item2 == true) from = Result.Item1; //path resets to correct one
            else return false;
        }
        Vector2Int target = Vector3ToVector2Int(Target);
        if (PassablePath(target) == false)
        {
            var Result = CorrectPath(target);
            if (Result.Item2 == true) target = Result.Item1; //path resets to correct one
            else return false;
        }
        return CalculateWay(from, target);
    }

    private bool CalculateWay(Vector2Int From, Vector2Int Target)
    {
        DistancesMap = new DistanceMapHolder();
        CurrentDistance = 0;
        ToCheck = new Stack<Vector2Int>[2];
        ToCheck[0] = new Stack<Vector2Int>();
        ToCheck[1] = new Stack<Vector2Int>();
        BoolCurrentStackTurn = false;
        ToCheck[CurrentStackTurn].Push(From);

        bool found = false;
        while (found == false && CurrentDistance < MaxSearchDistance)
        {
            found = IterateToCheckList(Target);
        }
       // Debug.Log(found + " distance " + CurrentDistance + " out of " + MaxSearchDistance);
        RestoreWay(Target, From);
        return found;
    }
    private bool IterateToCheckList(Vector2Int Target)
    {
        foreach (var CurrentPoint in ToCheck[CurrentStackTurn])
        {
            if (CurrentPoint == Target)
            { //Target is found
                return true;
            }
            else
            {
                DistancesMap[CurrentPoint.x, CurrentPoint.y] = new DistanceMapPoint(CurrentDistance); //setting this point to distance map
                GetNeighbours(CurrentPoint, BoolToInt(!BoolCurrentStackTurn)); //adding neighbour patches to next stack
            }
        }
        ToCheck[CurrentStackTurn].Clear();
        CurrentDistance++;
        BoolCurrentStackTurn = !BoolCurrentStackTurn;
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
                    ToCheck[StackToAdd].Push(current);
                }
            }
        }
    }
    private Vector2Int GetPartOfReturningWay(Vector2Int CurrentPoint)
    {
        short CurrentMinDistance = MaxSearchDistance;
        Vector2Int MinDistancePath = Vector2Int.zero;
        int x = 0;
        int y = 0;
        for (int yTries = 0; yTries < 3; yTries++)
        {
            for (int xTries = 0; xTries < 3; xTries++)
            {
                //  Debug.Log("path " + new Vector2Int(CurrentPoint.x, CurrentPoint.y) + " distance: " + DistancesMap[CurrentPoint.x, CurrentPoint.y]);
                if ((x == 0 || y == 0) && (x != 0 || y != 0))
                if (DistancesMap[CurrentPoint.x + x, CurrentPoint.y + y] != null && DistancesMap[CurrentPoint.x + x, CurrentPoint.y + y].Distance < CurrentMinDistance)
                {
                    CurrentMinDistance = DistancesMap[CurrentPoint.x + x, CurrentPoint.y + y].Distance;
                    MinDistancePath = new Vector2Int(CurrentPoint.x + x, CurrentPoint.y + y);
                    //  Debug.Log("Minimal distance out of  " + CurrentPoint + " is "  + MinimalDistancePath + " : " + MinimalDistance);
                }
                x = GetNextCordToTry(x);
            }
            x = 0;
            y = GetNextCordToTry(y);
        }
        return MinDistancePath;
    }
    private void RestoreWay(Vector2Int SearchStartingPosition, Vector2Int SearchEndingPosition)
    {
        Way = new Vector2Int[CurrentDistance + 1];
        Way[CurrentDistance] = SearchStartingPosition;
        Way[0] = SearchEndingPosition;
        for (int i = CurrentDistance - 1; i >= 1; i--)
        {
            Way[i] = GetPartOfReturningWay(Way[i + 1]);
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
    private Vector3 Vector2IntToVector3(Vector2Int v)
    {
        return new Vector3(v.x, v.y, 0);
    }
    private bool ValidPath(Vector2Int PathToCheck)
    {
        return PassablePath(PathToCheck) && ToCheck[0].Contains(PathToCheck) == false && ToCheck[1].Contains(PathToCheck) == false && DistancesMap[PathToCheck.x, PathToCheck.y] == null;
    }
    private bool PassablePath(Vector2Int PathToCheck)
    {
        return map.LandscapeMap[PathToCheck.x, PathToCheck.y] != null && 
            (map.LandscapeMap[PathToCheck.x, PathToCheck.y].Land == LandType.Passable || map.LandscapeMap[PathToCheck.x, PathToCheck.y].Land == LandType.WaterLow) ;
    }
    private Tuple<Vector2Int, bool> CorrectPath(Vector2Int Path)
    {
        int x = 0;
        int y = 0;
        for (int yTries = 0; yTries < 3; yTries++) //trying each cord 3 times (-1, 0, 1) in a semi-random pattern starting from 0
        {
            for (int xTries = 0; xTries < 3; xTries++)
            {
                if ((x == 0 || y == 0) && PassablePath(Path + new Vector2Int(x, y))) return Tuple.Create(Path + new Vector2Int(x, y), true);

                x = GetNextCordToTry(x);
            }
            x = 0;
            y = GetNextCordToTry(y);
        }
        Debug.LogError("Pathfinding correction error");
        return Tuple.Create(Vector2Int.zero, false);
    }
    private int GetNextCordToTry(int currentCord)
    {
        if (currentCord == 0)
        {
            if (pathfindingRandom.Next(0, 2) == 1) return 1;
            else return -1;
        }
        else if (currentCord == 1) return -1;
        else if (currentCord == -1) return 1;
        Debug.LogError("Pathfinding error");
        return 0;
    }
    private Vector2Int RoundVector3(Vector3 pos)
    {
        if (pos.x < 0)
        {
            if (pos.x - (int)pos.x < -0.95f) pos.x = (int)pos.x - 1;
        }
        else if (pos.x - (int)pos.x > 0.95f) pos.x = (int)pos.x + 1;

        if (pos.y < 0)
        {
            if (pos.y - (int)pos.y < -0.95f) pos.y = (int)pos.y - 1;
        }
        else if (pos.y - (int)pos.y > 0.95f) pos.y = (int)pos.y + 1;

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