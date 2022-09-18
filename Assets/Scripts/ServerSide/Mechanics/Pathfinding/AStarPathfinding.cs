using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerSideLogic
{
    public class AStarPathfinding : IPathfinding
    {


        private Map map;
        private short[,] DistancesMap;
        private bool[,] BlockedPaths;

        private short CurrentDistance = 0;

        public List<Vector2Int> Way = new List<Vector2Int>();
        public short MaxSearchDistance = 150;


        public AStarPathfinding(Map _map)
        {
            map = _map;
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
            Vector2Int Target = ConvertToVector2Int(TargetPath);
            Vector2Int From = ConvertToVector2Int(MovingUnit.position);
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
            bool result = CalculateWay(MovingUnit.position, Vector2IntToVector3(Target));
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
            Vector2Int From = ConvertToVector2Int(from);
            Vector2Int Target = ConvertToVector2Int(target);
            DistancesMap = new short[map.MapSizeX + (int)map.StartingPosition.x + 100, map.MapSizeY + (int)map.StartingPosition.z + 100];
            BlockedPaths = new bool[map.MapSizeX + (int)map.StartingPosition.x + 100, map.MapSizeY + (int)map.StartingPosition.z + 100];
            CurrentDistance = 0;


            bool found = false;
            Vector2Int CurrentPath = From;
            while (found == false && CurrentDistance < MaxSearchDistance)
            {
                CurrentPath = IterateWay(CurrentPath, Target);
                if (CurrentPath == Vector2.zero)
                { //there is no way
                    return false;
                }
                found = CurrentPath == Target;
            }
            RestoreWay(From, Target);
            return found;
        }

        private Vector2Int IterateWay(Vector2Int CurrentPath, Vector2Int Target)
        {
            Vector2Int NextPath = GetNearestPath(CurrentPath, Target);
            if (NextPath == Vector2.zero) return Vector2Int.zero; //not found anything
            if (DistancesMap[NextPath.x, NextPath.y] != 0)
            {
                BlockedPaths[CurrentPath.x, CurrentPath.y] = true;
                DistancesMap[CurrentPath.x, CurrentPath.y] = 0;
                CurrentDistance--;
            }
            else
            {
                CurrentDistance++;
                DistancesMap[NextPath.x, NextPath.y] = CurrentDistance;
                //  Debug.Log(NextPath.x + " " + NextPath.y + " = " + CurrentDistance);
            }
            return NextPath;
        }
        private Vector2Int GetNearestPath(Vector2Int CurrentPath, Vector2Int Target)
        {
            Vector2Int Direction = Normalize(Target - CurrentPath);
            Vector2Int NextPath = CurrentPath + Direction;
            if (ValidPath(NextPath))
            {
                return NextPath;
            }
            else
            {
                NextPath = CurrentPath + new Vector2Int(1, 0);
                if (ValidPath(NextPath)) return NextPath;

                NextPath = CurrentPath + new Vector2Int(0, 1);
                if (ValidPath(NextPath)) return NextPath;

                NextPath = CurrentPath + new Vector2Int(-1, 0);
                if (ValidPath(NextPath)) return NextPath;

                NextPath = CurrentPath + new Vector2Int(0, -1);
                if (ValidPath(NextPath)) return NextPath;
            }
            Debug.Log("Path not found, current is " + CurrentPath.x + " " + CurrentPath.y);
            return Vector2Int.zero;
        }
        private bool ValidPath(Vector2Int path)
        {
            if (map.ObstaclesMap[path.x, path.y] == false && BlockedPaths[path.x, path.y] == false)
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
            if (map.ObstaclesMap[path.x, path.y] == false)
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
        private Vector2Int GetPartOfReturningWay(Vector2Int CurrentPoint)
        {
            short CurrentMaxDistance = 0;
            Vector2Int MaximumDistancePath = Vector2Int.zero;
            for (int y = 0; y != -2; y++)
            {
                for (int x = 0; x != -2; x++)
                {
                    //  Debug.Log("path " + new Vector2Int(CurrentPoint.x, CurrentPoint.y) + " distance: " + DistancesMap[CurrentPoint.x, CurrentPoint.y]);
                    if (CurrentPoint.x + x >= 0 && CurrentPoint.y + y >= 0 && DistancesMap[CurrentPoint.x + x, CurrentPoint.y + y] > CurrentMaxDistance && DistancesMap[CurrentPoint.x + x, CurrentPoint.y + y] != 0)
                    {
                        CurrentMaxDistance = DistancesMap[CurrentPoint.x + x, CurrentPoint.y + y];
                        MaximumDistancePath = new Vector2Int(CurrentPoint.x + x, CurrentPoint.y + y);
                        //  Debug.Log("Minimal distance out of  " + CurrentPoint + " is "  + MinimalDistancePath + " : " + MinimalDistance);
                    }
                    if (x == -1) x = -3;
                    if (x == 1) x = -2;
                }
                if (y == -1) y = -3;
                if (y == 1) y = -2;
            }
            return MaximumDistancePath;
        }
        private void RestoreWay(Vector2Int StartingPosition, Vector2Int EndPosition)
        {
            Way = new List<Vector2Int>();
            Way.Add(StartingPosition);
            for (int i = 0; i < CurrentDistance; i++)
            {
                Way.Add(GetPartOfReturningWay(Way[i]));
            }
            Way.Add(EndPosition);
        }
        private Vector2Int ConvertToVector2Int(Vector3 SourceVector)
        {
            return new Vector2Int((int)SourceVector.x, (int)SourceVector.z);
        }
        public Vector3 Vector2IntToVector3(Vector2Int SourceVector, float y = 1)
        {
            return new Vector3(SourceVector.x, y, SourceVector.y);
        }
    }
}

