using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generation;

[System.Serializable]
public class UnitMovement
{
    private Unit unit = null;
    public Vector3[] Way = null;
    [SerializeField]
    private Vector3[] localWay = null;

    public int CurrentDistance { private get; set; } = 1;
    [SerializeField]
    private int localCurrentDistance = 1;
    public Unit currentTarget = null;

    [SerializeField]
    private Vector3 CurrentDirection = Vector3.zero;

    public UnitMovement(Unit unit)
    {
        this.unit = unit;
    }

    public void WayMoving()
    {
        if (Way != null && Way.Length > 1)
        {
            localWay = Way;
            Way = null;
            localCurrentDistance = 1;
            CurrentDirection = Vector3.zero;
        }
        if (localWay == null)
        {
            if (currentTarget == null)
            {
                unit.animator.SetFloat("XSpeed", 0);
                unit.animator.SetFloat("YSpeed", 0);
                unit.animator.SetBool("Stopped", true);
            }
            else BlindChase(currentTarget);
            return;
        }

        if (CurrentDirection == Vector3.zero) CurrentDirection = GetDirection();
        //double check
        if (CurrentDirection == Vector3.zero)
        {
            localCurrentDistance++;
            if (localCurrentDistance >= localWay.Length)
            {
                localWay = null;
                return;
            }
            else CurrentDirection = GetDirection();
        }

        //  Debug.Log(CurrentDirection + " new");

        unit.transform.eulerAngles = new Vector3(0, 0, 0);
        unit.transform.Translate(CurrentDirection * unit.Stats.MoveSpeed * Time.fixedDeltaTime);
        if (CurrentDirection.x < 0) unit.transform.eulerAngles = new Vector3(0, 180, 0); 

        unit.animator.SetBool("Stopped", false);
        unit.animator.SetFloat("XSpeed", CurrentDirection.x);
        unit.animator.SetFloat("YSpeed", CurrentDirection.y);

        //correction of direction
        if (CurrentDirection.x > 0 && unit.transform.position.x > localWay[localCurrentDistance].x) 
        {
            CurrentDirection.x = 0;
            unit.transform.position = new Vector3(localWay[localCurrentDistance].x, unit.transform.position.y, 0);
        }
        if (CurrentDirection.x < 0 && unit.transform.position.x < localWay[localCurrentDistance].x) 
        { 
            CurrentDirection.x = 0;
            unit.transform.position = new Vector3(localWay[localCurrentDistance].x, unit.transform.position.y, 0);
        }
        if (CurrentDirection.y > 0 && unit.transform.position.y > localWay[localCurrentDistance].y) 
        {
            CurrentDirection.y = 0; 
            unit.transform.position = new Vector3(unit.transform.position.x, localWay[localCurrentDistance].y, 0);
        }
        if (CurrentDirection.y < 0 && unit.transform.position.y < localWay[localCurrentDistance].y) 
        { 
            CurrentDirection.y = 0;
            unit.transform.position = new Vector3(unit.transform.position.x, localWay[localCurrentDistance].y, 0); 
        }

        if (CurrentDirection == Vector3.zero)
        {
            localCurrentDistance++;
            if (localCurrentDistance >= localWay.Length) localWay = null;
        }
    }

    private Vector3 GetDirection()
    {
        Vector3 Delta = localWay[localCurrentDistance] - unit.transform.position;
        if (Delta.x > 0) Delta.x = 1;
        if (Delta.x < 0) Delta.x = -1;
        if (Delta.y > 0) Delta.y = 1;
        if (Delta.y < 0) Delta.y = -1;
        return Delta;
    }
    private Vector3 GetDirection(Vector3 target)
    {
        Vector3 Delta = target - unit.transform.position;
        if (Delta.x > 0) Delta.x = 1;
        if (Delta.x < 0) Delta.x = -1;
        if (Delta.y > 0) Delta.y = 1;
        if (Delta.y < 0) Delta.y = -1;
        return Delta;
    }

    public bool GetWayTarget(Vector3 Target)
    {
        if (unit.Stats.MoveSpeed == 0)
        {
            Debug.Log("Attempting to move unit with 0 movespeed");
        }
        CurrentDistance = 1;
        bool Result = GameManager.Pathfinding.GetWayPath(unit, Target, unit.BodySize, 2);
        if (Result == false) Way = null;
        return Result;
    }
    public bool GetWayTarget(Unit TargetUnit)
    {
        if (unit.Stats.MoveSpeed == 0)
        {
            Debug.Log("Attempting to move unit with 0 movespeed");
        }
        CurrentDistance = 1;
        bool Result = GameManager.Pathfinding.GetWayPath(unit, TargetUnit.LastNonTransformPosition, unit.BodySize, 2);
        if (Result) currentTarget = TargetUnit;
        else
        {
            Way = null;
            currentTarget = null;
        }
        return Result;
    }

    public void BlindChase(Unit chaseTarget)
    {
        unit.transform.eulerAngles = new Vector3(0, 0, 0);
        Vector3 direction = GetDirection(chaseTarget.transform.position);
        unit.transform.Translate(direction * unit.Stats.MoveSpeed * Time.fixedDeltaTime);
        if (direction.x < 0) unit.transform.eulerAngles = new Vector3(0, 180, 0);
    }

    public void RunAway(Vector3 fearSource)
    {
        Sector unitSector = GameManager.map.GetUnitSector(unit);
        Vector3Int delta = BasicFunctions.ToUnitVector(unit.LastNonTransformPosition - fearSource);
        Vector3 positionToRun = Vector3.zero;
        if (GameManager.Random.Next(0, 2) == 1 && delta.x != 0)
        {
            positionToRun = GetRandomPositionInNeibghourSector(unitSector, delta.x, 0);
            if (positionToRun == Vector3.zero) positionToRun = GetRandomPositionInNeibghourSector(unitSector, 0, delta.y);
            if (positionToRun == Vector3.zero)
            {
                if (GameManager.Random.Next(0, 2) == 1) positionToRun = GetRandomPositionInNeibghourSector(unitSector, delta.x * -1, 0);
                else positionToRun = GetRandomPositionInNeibghourSector(unitSector, 0, delta.y * -1);
            }
        }
        else
        {
            positionToRun = GetRandomPositionInNeibghourSector(unitSector, 0, delta.y);
            if (positionToRun == Vector3.zero) positionToRun = GetRandomPositionInNeibghourSector(unitSector, delta.x, 0);
            if (positionToRun == Vector3.zero)
            {
                if (GameManager.Random.Next(0, 2) == 1) positionToRun = GetRandomPositionInNeibghourSector(unitSector, delta.x * -1, 0);
                else positionToRun = GetRandomPositionInNeibghourSector(unitSector, 0, delta.y * -1);
            }
        }
        if (positionToRun != Vector3.zero) GetWayTarget(positionToRun);
    }

    private Vector3 GetRandomPositionInNeibghourSector(Sector unitSector, int x, int y)
    {
        if (GameManager.map.SectorMap[unitSector.X + x, unitSector.Y + y] != null)
        {
            return BasicFunctions.ToVector3(GameManager.map.SectorMap[unitSector.X + x, unitSector.Y + y].RandomPoint);
        }
        else return Vector3.zero;
    }
}
