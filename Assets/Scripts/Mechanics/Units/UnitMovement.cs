using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //    Debug.Log("new way");
            localWay = Way;
            Way = null;
            localCurrentDistance = CurrentDistance;
            CurrentDirection = Vector3.zero;
        }
        if (localWay == null)
        {
            unit.animator.SetFloat("XSpeed", 0);
            unit.animator.SetFloat("YSpeed", 0);
            unit.animator.SetBool("Stopped", true);
            return;
        }

        if (CurrentDirection == Vector3.zero) CurrentDirection = GetDirection();

      //  Debug.Log(CurrentDirection + " new");

        unit.transform.eulerAngles = new Vector3(0, 0, 0);
        unit.transform.Translate(CurrentDirection * unit.Stats.MoveSpeed * Time.fixedDeltaTime);
        if (CurrentDirection.x < 0) unit.transform.eulerAngles = new Vector3(0, 180, 0);

        unit.animator.SetBool("Stopped", false);
        unit.animator.SetFloat("XSpeed", CurrentDirection.x);
        unit.animator.SetFloat("YSpeed", CurrentDirection.y);

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
      //  Debug.Log(CurrentDirection);

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

    public bool GetWayTarget(Vector3 Target)
    {
        if (unit.Stats.MoveSpeed == 0)
        {
            Debug.Log("Attempting to move unit with 0 movespeed");
        }
        CurrentDistance = 1;
        bool Result = GameManager.Pathfinding.GetWayPath(unit, Target, 2);
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
        bool Result = GameManager.Pathfinding.GetWayPath(unit, TargetUnit.LastNonTransformPosition, 2);
        if (Result) currentTarget = TargetUnit;
        else
        {
            Way = null;
            currentTarget = null;
        }
        return Result;
    }
}
