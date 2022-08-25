using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AStarPathfinding : IPathfinding
{


    private Map map;
    private DistanceMapHolder DistancesMap;
    private List<Vector2Int> BlockedPaths;

    private short CurrentDistance = 0;

    public List<Vector2Int> Way = new List<Vector2Int>();
    public short MaxSearchDistance = 150;


    public AStarPathfinding(Map _map, short MaxSearchDistance)
    {
        map = _map;
        this.MaxSearchDistance = MaxSearchDistance;


        DistancesMap = new DistanceMapHolder();
    }
    public List<Vector2Int> GetLastWay()
    {
        return Way;
    }
    public bool GetPathBetweenPoints(Vector3 From, Vector3 Target)
    {
        return CalculateWay(From, Target);
    }
    public bool GetWayPath(Unit MovingUnit, Vector3 TargetPath, byte MaximumCorrectionStep = 2)
    {
        Vector2Int Target = BasicFunctions.Vector3ToVector2Int(TargetPath);
        Vector2Int From = BasicFunctions.Vector3ToVector2Int(MovingUnit.transform.position);
        if (ValidPathNotIncludeBlocked(Target) == false)
        { //target path correction
            Target = CorrectPath(Target, 2);
            if (Target == Vector2Int.zero)
            {
                Debug.Log("Path not found");
                return false;
            }
        }
        if (ValidPathNotIncludeBlocked(From) == false)
        { //from path correction
            From = CorrectPath(From, 1);
            if (From == Vector2Int.zero)
            {
                Debug.Log("Path not found");
                return false;
            }
        }
        bool result = CalculateWay(MovingUnit.transform.position, BasicFunctions.Vector2IntToVector3(Target));
        if (result)
        {
            MovingUnit.Way = Way;
            MovingUnit.CurrentDistance = 1;
        }
        return result;
    }
    private Vector2Int CorrectPath(Vector2Int Path, byte MaximumCorrectionStep)
    {
        byte CurrentStep = 1;
        Vector2Int CurrentPath;
        while (CurrentStep <= MaximumCorrectionStep)
        {
            CurrentPath = Path + new Vector2Int(CurrentStep, 0);
            if (ValidPathNotIncludeBlocked(CurrentPath)) return CurrentPath;

            CurrentPath = Path + new Vector2Int(-CurrentStep, 0);
            if (ValidPathNotIncludeBlocked(CurrentPath)) return CurrentPath;

            CurrentPath = Path + new Vector2Int(0, CurrentStep);
            if (ValidPathNotIncludeBlocked(CurrentPath)) return CurrentPath;

            CurrentPath = Path + new Vector2Int(0, -CurrentStep);
            if (ValidPathNotIncludeBlocked(CurrentPath)) return CurrentPath;
            CurrentStep++;
        }
        return Vector2Int.zero;
    }

    private bool CalculateWay(Vector3 from, Vector3 target)
    {
        Vector2Int From = BasicFunctions.Vector3ToVector2Int(from);
        Vector2Int Target = BasicFunctions.Vector3ToVector2Int(target);
        lock (DistancesMap)
        {
            DistancesMap = new DistanceMapHolder();
            BlockedPaths = new List<Vector2Int>();

            Way = new List<Vector2Int>();
            CurrentDistance = 0;

            bool found = false;
            var CurrentPathResult = Tuple.Create(From, false);
            while (found == false && CurrentDistance < MaxSearchDistance)
            {
                CurrentPathResult = IterateWay(CurrentPathResult.Item1, Target); //getting next path
                if (CurrentPathResult.Item2)
                { //no way, literally
                    Way = null;
                    return false;
                }
                found = CurrentPathResult.Item1 == Target;
            }
            if (found) Way.Add(Target);
            if (CurrentDistance >= MaxSearchDistance) Debug.Log("Max search distance exceded");
            return found;
        }
    }
    /// <returns>Tuple Vector2Int NextPath, FailedToFindAWay</returns>
    private Tuple<Vector2Int, bool> IterateWay(Vector2Int CurrentPath, Vector2Int Target)
    {
        bool FailedToFindAWay = false;
        Vector2Int Direction = GetCorrectDirection(CurrentPath, Target);
      //  Debug.Log("current path " + CurrentPath + " target path " + Target + " direction " + Direction);
        if (Direction != Vector2Int.zero)
        {
            Way.Add(CurrentPath + Direction);
            CurrentDistance++;
            return new Tuple<Vector2Int, bool>(CurrentPath + Direction, FailedToFindAWay);
        }
        else
        { //path is blocked
          //trying to step back and block this route
            var PreviousPathResult = GetPreviousPath(CurrentPath);
            if (PreviousPathResult.Item2 == false)
            {
                Debug.Log("Path not found, current is " + CurrentPath.x + " " + CurrentPath.y);
                FailedToFindAWay = true;
            }
            else
            {
                BlockedPaths.Add(CurrentPath);
                CurrentDistance--;
                Debug.Log("Path " + CurrentPath + " is added to blocked list");
            }
            return Tuple.Create(CurrentPath + Direction, FailedToFindAWay);
        }
    }
    private bool ValidPath(Vector2Int path, bool IncludeAlreadyMarked = false)
    {
      //  Debug.Log(map.LandscapeMap[path.x, path.y].Land);
        if (DistancesMap[path.x, path.y] == null && ((map.LandscapeMap[path.x, path.y].Land == LandType.Passable || map.LandscapeMap[path.x, path.y].Land == LandType.WaterLow) && BlockedPaths.Contains(path) == false))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private bool ValidPathNotIncludeBlocked(Vector2Int path)
    {
        if (map.LandscapeMap[path.x, path.y].Land == LandType.Passable || map.LandscapeMap[path.x, path.y].Land == LandType.WaterLow)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private Vector2Int Normalize(Vector2Int p)
    {
        Vector2Int v = new Vector2Int(0, 0);
        if (p.x > 0) v.x = 1;
        if (p.x < 0) v.x = -1;
        if (p.y > 0) v.y = 1;
        if (p.y < 0) v.y = -1;
        return v;
    }

    private Tuple<Vector2Int, bool> GetPreviousPath(Vector2Int CurrentPath)
    {
        int index = Way.IndexOf(CurrentPath) - 1;
        if (index > -1)
        {
            return Tuple.Create(Way[index], true);
        }
        else
        {
            Debug.LogError("-1 waypath returning");
            return Tuple.Create(Way[index], false);
        }

    }
    private Vector2Int GetCorrectDirection(Vector2Int From, Vector2Int To)
    {
        bool XAxisPriority = Convert.ToBoolean(UnityEngine.Random.Range(0, 2));
        Vector2Int Direction = BasicFunctions.GetDirectionBetween2Points(From, To, XAxisPriority);
        if (ValidPath(From + Direction)) return Direction;
        else XAxisPriority = !XAxisPriority;

        Direction = BasicFunctions.GetDirectionBetween2Points(From, To, XAxisPriority);
        if (ValidPath(From + Direction)) return Direction;

        //not found direct way? then search for indirect ones
        int PositiveDirectionPriority = 1;
        if (UnityEngine.Random.Range(0, 2) == 1) PositiveDirectionPriority = -1;
        //Function tries to find parallel way and not to go backwards
        if (Direction.x != 0)
        { //if direction goes towards X axis, parallel way should be offset by Y axis
            Direction = new Vector2Int(0, 1 * PositiveDirectionPriority);
            if (ValidPath(From + Direction)) return Direction;
            else Direction = new Vector2Int(0, 1 * PositiveDirectionPriority * -1);
            if (ValidPath(From + Direction)) return Direction;
            //there is only backwards way left
        }
        else if (Direction.y != 0)
        {
            Direction = new Vector2Int(1 * PositiveDirectionPriority, 0);
            if (ValidPath(From + Direction)) return Direction;
            else Direction = new Vector2Int(1 * PositiveDirectionPriority * -1, 0);
            if (ValidPath(From + Direction)) return Direction;
            //there is only backwards way left
        }
        return Vector2Int.zero;
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
