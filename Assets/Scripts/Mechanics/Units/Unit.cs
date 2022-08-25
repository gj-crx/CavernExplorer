using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Unit : MonoBehaviour
{
    public int ID = -1;
    public UnitStats Stats;




    public List<Vector2Int> Way = new List<Vector2Int>();
    public int CurrentDistance { private get; set; }

    public delegate void OnKill(Unit killed);

    public OnKill OnKilled;
    public IBehavior behavior;


    private void Start()
    {
        GetBehavior();
        StartUnitActionsControlling();
    }

    private void Update()
    {
        if (Way.Count > 0)
        {
            WayMoving();
        }
    }

    public bool GetWayTarget(Vector3 target)
    {
        if (Stats.MoveSpeed == 0)
        {
            Debug.Log("Attempting to move unit with 0 movespeed");
        }
        CurrentDistance = 1;
        bool Result = GameManager.Pathfinding.GetWayPath(this, target, 2);
        if (GameManager.DebugMode)
        {
       //     game.TestWay = game.pf.GetLastWay();
        }
        return Result;
    }
    public Vector3 GetPositionNextToUnit(Vector3 From)
    {
        Vector3 direction = transform.position - From;
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            if (direction.x > 0) return transform.position + new Vector3(Stats.CollisionRadius + 1, 0, 0);
            else return transform.position + new Vector3(Stats.CollisionRadius - 1, 0, 0);
        }
        else
        {
            if (direction.z > 0) return transform.position + new Vector3(0, 0, Stats.CollisionRadius + 1);
            else return transform.position + new Vector3(0, 0, Stats.CollisionRadius - 1);
        }
    }
    public void Death()
    {
        Debug.Log(gameObject.name + " " + ID + " killed");
        if (behavior != null)
        {
            behavior.Clear();
        }
        Destroy(gameObject);
    }
    private void OnKillMethod(Unit KilledUnit)
    {
        if (behavior != null)
        {
            behavior.BehaviorAction();
        }
    }
    private void GetBehavior()
    {
        if (gameObject.tag == "Creep")
        {
            behavior = new Behaviours.CaveDwellerBehaviour(this);
        }
    }
    private void StartUnitActionsControlling()
    {
        if (behavior != null && behavior.HaveExternalOrder == false && behavior.Active)
        {
            behavior.StartIterationsAsync(10000, Random.Range(0, 1000));
        }

    }
    private async Task StartControllingUnitMovements(int ActualDelay, int RandomizedPreDelay = 0)
    {
        await Task.Delay(RandomizedPreDelay);
        while (gameObject.activeInHierarchy)
        {
            if (Way.Count > 0)
            {
                WayMoving();
            }
            await Task.Delay(ActualDelay);
        }
    }
    private void MoveUnit(Vector3 To)
    {
        Vector3 MovementVector = UnitLogic.VectorToDirection(To - transform.position);
        transform.position += (MovementVector * Time.deltaTime * Stats.MoveSpeed);
        //   Debug.Log(UnitName + position + " changed");
    }
    private void WayMoving()
    {
        if (Stats.MoveSpeed == 0)
        {
            return;
        }
        //   Debug.Log("waymoving applyed");
        if (Vector3.Distance(transform.position, BasicFunctions.Vector2IntToVector3(Way[CurrentDistance])) < Stats.MoveSpeed * 0.1f)
        {
            //  Debug.Log(Way[CurrentDistance] + " next way point of the distance " + CurrentDistance);
            transform.position = BasicFunctions.Vector2IntToVector3(Way[CurrentDistance]);
            CurrentDistance++;
        }
        if (CurrentDistance >= Way.Count)
        {
            Way.Clear();
            CurrentDistance = 1;
            behavior.HaveExternalOrder = false;
        }
        else
        {
            MoveUnit(BasicFunctions.Vector2IntToVector3(Way[CurrentDistance]));
        }
    }
    public enum UnitClass : byte
    {
        RegularUnit = 0,
        Building = 1,
        Tree = 2
    }
    private enum UnitAnimation : byte
    {
        IdleOrMoving = 0,
        Attack = 1,
        ResourceGathering = 2,
        SpellCasting = 3
    }


    [System.Serializable]
    public struct UnitStats
    {
        public float MaxHP;
        public float CurrentHP;
        public float Damage;
        public float Regeneration;
        public float MoveSpeed;
        public float AttackDelay;
        public float AttackRange;
        public float VisionRadius;
        public float TrainTimeNeeded;
        public float[] ResourcesCostToBuild;
        public float ResourcesCarriedMaximum;
        public float[] ResourcesGivenOnKilled;

        public byte CollisionRadius;
    }
}
