using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Unit : MonoBehaviour
{
    public int ID = -1;
    public bool AIControlled = true;
    public UnitStats Stats;




    public Vector3[] Way = null;
    public int CurrentDistance { private get; set; } = 1;

    public delegate void OnKill(Unit killed);
    public OnKill OnKilled;
    public IBehavior behavior;
    private Animator _animator;

    private float _movingTime = 0;
    private Unit _currentTarget;


    private void Start()
    {
        GetBehavior();
        StartUnitActionsControlling();
        try { _animator = GetComponent<Animator>(); } catch { }
    }

    private void Update()
    {

    }
    private void FixedUpdate()
    {
        if (AIControlled) WayMoving(true);
    }

    public bool GetWayTarget(Vector3 Target)
    {
        if (Stats.MoveSpeed == 0)
        {
            Debug.Log("Attempting to move unit with 0 movespeed");
        }
        CurrentDistance = 1;
        Way = null;
        bool Result = GameManager.Pathfinding.GetWayPath(this, Target, 2);
        return Result;
    }
    public bool GetWayTarget(Unit TargetUnit)
    {
        if (Stats.MoveSpeed == 0)
        {
            Debug.Log("Attempting to move unit with 0 movespeed");
        }
        CurrentDistance = 1;
        Way = null;
        bool Result = GameManager.Pathfinding.GetWayPath(this, TargetUnit.transform.position, 2);
        if (Result) _currentTarget = TargetUnit;
        else _currentTarget = null;
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
    public void GetDamage(float Damage, Unit Attacker)
    {
        Stats.CurrentHP -= Damage;
        if (Stats.CurrentHP <= 0) Death();
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
    private void WayMoving(bool alternative)
    {
        _animator.SetBool("Stopped", true);
        if (Way != null && Way.Length > 0)
        {
            _movingTime += Stats.MoveSpeed * Time.fixedDeltaTime;
            transform.position = Vector3.Lerp(Way[CurrentDistance - 1], Way[CurrentDistance], _movingTime);
            Vector3 delta = Way[CurrentDistance] - Way[CurrentDistance - 1];
            _animator.SetFloat("XSpeed", delta.x);
            _animator.SetFloat("YSpeed", delta.y);
            _animator.SetBool("Stopped", false);
            if (delta.x < 0) transform.eulerAngles = new Vector3(0, -180, 0);
            else transform.eulerAngles = new Vector3(0, 0, 0);
            if (_movingTime > 1)
            {
                _movingTime = 0;
                CurrentDistance++;
                if (CurrentDistance >= Way.Length)
                {
                    Way = null;
                    CurrentDistance = 1;
                    if (behavior != null) behavior.HaveExternalOrder = false;
                }
            }
        }
        else if (_currentTarget != null) Chase();
    }
    private void Chase()
    {
        Vector3 Direction = _currentTarget.transform.position - transform.position;

        transform.eulerAngles = new Vector3(0, 0, 0);
        _animator.SetBool("Stopped", false);
        _animator.SetFloat("XSpeed", Direction.x);
        _animator.SetFloat("YSpeed", Direction.y);

        transform.Translate(Direction.normalized * Stats.MoveSpeed * Time.fixedDeltaTime);
        if (Direction.x < 0) transform.eulerAngles = new Vector3(0, -180, 0);
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
