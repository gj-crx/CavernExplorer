using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;


public class Unit : MonoBehaviour
{
    public int ID = -1;
    public bool AIControlled = true;
    public UnitStats Stats;
    public List<Items.Item> ItemsDroppedOnDeath = new List<Items.Item>();
    public UnitMovement unitMovement;
    [HideInInspector]
    public Vector3 LastNonTransformPosition;





    public delegate void OnKill(Unit killed);
    public OnKill OnKilled;
    public IBehavior behavior;
    public Animator animator;

    private Unit _currentTarget;
  //  [HideInInspector]
    public bool MovementHalted = false;


    private void Start()
    {
        unitMovement = new UnitMovement(this);
        GetBehavior();
       // StartUnitActionsControlling();
        try { animator = GetComponent<Animator>(); } catch { }
        GameManager.dataBase.AllUnits.Add(this);
    }

    private void Update()
    {
        LastNonTransformPosition = transform.position;
    }
    private void FixedUpdate()
    {
        if (AIControlled && MovementHalted == false) unitMovement.WayMoving();
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
        gameObject.AddComponent<UI.InventoryLogic.Corpse>().InitializeCorpse(new Vector3(0, 0, 165), new Color(0.5660378f, 0.1735493f, 0.1735493f), ItemsDroppedOnDeath);
        gameObject.tag = "Corpse";
        Destroy(animator);
        if (behavior != null) behavior.Clear();
        if (GetComponent<Behaviours.Fighting>() != null) Destroy(GetComponent<Behaviours.Fighting>());
        GameManager.dataBase.AllUnits.Remove(this);
        Destroy(this);
    }
    private void OnKillMethod(Unit KilledUnit)
    {
        if (behavior != null)
        {

        }
    }
    private void GetBehavior()
    {
        if (gameObject.tag == "Creep")
        {
            behavior = new Behaviours.CaveDwellerBehaviour(this);
        }
    }
  
    private void Chase()
    {
        Vector3 Direction = _currentTarget.transform.position - transform.position;

        transform.eulerAngles = new Vector3(0, 0, 0);
        animator.SetBool("Stopped", false);
        animator.SetFloat("XSpeed", Direction.x);
        animator.SetFloat("YSpeed", Direction.y);

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
        public AttackType attackType;

        public float MaxHP;
        public float CurrentHP;
        public float Damage;
        public float Regeneration;
        public float MoveSpeed;
        public float AttackDelay;
        public float AttackRange;
        public float VisionRadius;

        public byte CollisionRadius;

        public void CombineStats(UnitStats AdditionalStats)
        {
            if (AdditionalStats.attackType != AttackType.None) attackType = AdditionalStats.attackType;

            MaxHP += AdditionalStats.MaxHP;
            CurrentHP += AdditionalStats.CurrentHP;
            Damage += AdditionalStats.Damage;
            Regeneration += AdditionalStats.Regeneration;
            MoveSpeed += AdditionalStats.MoveSpeed;
            AttackDelay += AdditionalStats.AttackDelay;
            AttackRange += AdditionalStats.AttackRange;
            VisionRadius += AdditionalStats.VisionRadius;
        }
        public void SubstactStats(UnitStats SubstractedStats)
        {
            if (SubstractedStats.attackType != AttackType.None) attackType = AttackType.Melee;

            MaxHP -= SubstractedStats.MaxHP;
            CurrentHP -= SubstractedStats.CurrentHP;
            Damage -= SubstractedStats.Damage;
            Regeneration -= SubstractedStats.Regeneration;
            MoveSpeed -= SubstractedStats.MoveSpeed;
            AttackDelay -= SubstractedStats.AttackDelay;
            AttackRange -= SubstractedStats.AttackRange;
            VisionRadius -= SubstractedStats.VisionRadius;
        }
    }
    public enum AttackType : byte
    {
        Melee = 0,
        Ranged = 1,
        None = 2
    }
}
