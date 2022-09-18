using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerSideLogic
{
    public class NormalPathfinding : IPathfinding
    {


        public Map map;
        short[,] DistancesMap;


        short CurrentDistance = 0;
        List<Vector2Int> Way = new List<Vector2Int>();

        Stack<Vector2Int>[] ToCheck;
        short MaxSearchDistance = 150;

        bool BCurrentStackTurn = false;
        int CurrentStackTurn
        {
            get
            {
                return BoolToInt(BCurrentStackTurn);
            }
        }

        public NormalPathfinding(Map m)
        {
            map = m;
        }
        public List<Vector2Int> GetLastWay()
        {
            return Way;
        }
        public bool GetWayPath(Unit MovingUnit, Vector3 Target, byte MaximumCorrectionStep = 2)
        {
            bool result = CalculateWay(MovingUnit.position, Target);
            if (result)
            {
                MovingUnit.Way = Way;
                MovingUnit.CurrentDistance = 1;
            }
            return result;
        }
        public bool GetPathBetweenPoints(Vector3 From, Vector3 Target)
        {
            return CalculateWay(From, Target);
        }

        private bool CalculateWay(Vector3 from, Vector3 target)
        {
            Vector2Int From = ConvertVector3(from);
            Vector2Int Target = ConvertVector3(target);
            DistancesMap = new short[map.MapSizeX + (int)map.StartingPosition.x + 100, map.MapSizeY + (int)map.StartingPosition.z + 100];
            CurrentDistance = 0;
            ToCheck = new Stack<Vector2Int>[2];
            ToCheck[0] = new Stack<Vector2Int>();
            ToCheck[1] = new Stack<Vector2Int>();
            BCurrentStackTurn = false;
            ToCheck[CurrentStackTurn].Push(Target);

            bool found = false;
            while (found == false && CurrentDistance < MaxSearchDistance)
            {
                found = IterateToCheckList(From);
            }
            RestoreWay(Target, From);
            return found;
        }
        private bool IterateToCheckList(Vector2Int Target)
        {
            foreach (var v in ToCheck[CurrentStackTurn])
            {
                if (v == Target)
                { //Target is found
                    return true;
                }
                else
                {
                    DistancesMap[v.x, v.y] = CurrentDistance; //setting this point to distance map
                    GetNeighbours(v, BoolToInt(!BCurrentStackTurn)); //adding neighbour patches to next stack
                }
            }
            ToCheck[CurrentStackTurn].Clear();
            CurrentDistance++;
            BCurrentStackTurn = !BCurrentStackTurn;
            return false;
        }
        private void GetNeighbours(Vector2Int point, int StackToAdd)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    Vector2Int current = new Vector2Int(point.x + x, point.y + y);
                    if (ToCheck[0].Contains(current) == false && ToCheck[1].Contains(current) == false && DistancesMap[current.x, current.y] == 0 && map.ObstaclesMap[current.x, current.y] == false)
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
            for (int y = 0; y != -2; y++)
            {
                for (int x = 0; x != -2; x++)
                {
                    //  Debug.Log("path " + new Vector2Int(CurrentPoint.x, CurrentPoint.y) + " distance: " + DistancesMap[CurrentPoint.x, CurrentPoint.y]);
                    if (CurrentPoint.x + x >= 0 && CurrentPoint.y + y >= 0 && DistancesMap[CurrentPoint.x + x, CurrentPoint.y + y] < CurrentMinDistance && DistancesMap[CurrentPoint.x + x, CurrentPoint.y + y] != 0)
                    {
                        CurrentMinDistance = DistancesMap[CurrentPoint.x + x, CurrentPoint.y + y];
                        MinDistancePath = new Vector2Int(CurrentPoint.x + x, CurrentPoint.y + y);
                        //  Debug.Log("Minimal distance out of  " + CurrentPoint + " is "  + MinimalDistancePath + " : " + MinimalDistance);
                    }
                    if (x == -1) x = -3;
                    if (x == 1) x = -2;
                }
                if (y == -1) y = -3;
                if (y == 1) y = -2;
            }
            return MinDistancePath;
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
        private int BoolToInt(bool b)
        {
            if (b) return 1;
            else return 0;
        }
        private Vector2Int ConvertVector3(Vector3 v)
        {
            return new Vector2Int((int)v.x, (int)v.z);
        }
        public Vector3 Vector2IntToVector3(Vector2Int v, float y = 1)
        {
            return new Vector3(v.x, y, v.y);
        }
    }
}
