using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UI.Indicators;
using Items;
using Pathfinding;


public class Unit : MonoBehaviour
{
    public int ID = -1;
    public bool AIControlled = true;
    public BodyType bodyType;
    [SerializeField]
    private BodyTypeName bodyTypeName = BodyTypeName.Normal;
    public UnitMovement unitMovement;
    public UnitStats Stats;
    public Shooting shooting = null;


    public IBehavior behavior = null;
    [HideInInspector]
    public List<Item> ItemsDroppedOnDeath = new List<Item>();
    [HideInInspector]
    public Vector3 LastNonTransformPosition;
    public delegate void OnKill(Unit killed);
    public OnKill OnKilled;
    [HideInInspector]
    public IHealthBar healthBar;
    public Animator animator;
    [HideInInspector]
    public bool MovementHalted = false;

    [SerializeField]
    public string behaviorName = "CaveDweller";
    [SerializeField]
    private List<PossibleDrop> possibleDropOnDeath = new List<PossibleDrop>();
    private Unit currentTarget;
    private List<AppliedEffect> appliedEffects = new List<AppliedEffect>();

    private void Awake()
    {
        LastNonTransformPosition = transform.position;
    }
    private void Start()
    {
        unitMovement = new UnitMovement(this);
        GetBodyType();
        GetBehavior();
        if (animator == null)
        {
            try { animator = GetComponent<Animator>(); } catch { }
        }
        if (gameObject.tag == "Creep")
        {
            try { healthBar = transform.Find("HealthBar").GetComponent<IHealthBar>(); } catch { }
        }
        else if (gameObject.tag == "Player")
        {
            healthBar = UI.UIManager.Singleton.panel_HealthBar.GetComponent<IHealthBar>();
        }
        ItemsDroppedOnDeath = PossibleDrop.GenerateItems(possibleDropOnDeath);
        GameManager.dataBase.AllUnits.Add(this);
    }

    private void Update()
    {
        LastNonTransformPosition = transform.position;
        CalculateEffects();
        Regeneration();
    }
    private void FixedUpdate()
    {
        if (AIControlled && MovementHalted == false && Stats.MoveSpeed > 0) unitMovement.WayMoving();
    }

    
    public void GetDamage(float damage, Unit attacker)
    {
        Stats.CurrentHP -= damage;
        if (Stats.CurrentHP <= 0) Death();
        else
        {
            if (healthBar != null) healthBar.ShowHealth(Stats.CurrentHP, Stats.MaxHP, damage);
        }
    }
    public void Death()
    {
        if (gameObject.tag == "Player")
        {
            UI.UIManager.Singleton.dialoguePanelsDictionary["DeathPanel"].SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            if (behavior != null)
            {
                behavior.Clear();
            }
            gameObject.tag = "Corpse";
            if (animator != null)
            {
                animator.Play("Death");
            }
            gameObject.AddComponent<UI.InventoryLogic.Corpse>().InitializeCorpse(ItemsDroppedOnDeath);
            if (behavior != null) behavior.Clear();
            if (healthBar != null) healthBar.TurnOff();
            if (GetComponent<Behaviours.Fighting>() != null) Destroy(GetComponent<Behaviours.Fighting>());


            GameManager.dataBase.AllUnits.Remove(this);
            Destroy(this);
        }
    }
    public void ApplyEffect(UnitStats effectStats, float duration)
    {
        appliedEffects.Add(new AppliedEffect(effectStats, duration));
        Stats.CombineStats(effectStats);
    }
    private void CalculateEffects()
    {
        Stack<AppliedEffect> effectsToDelete = new Stack<AppliedEffect>();
        for (int i = 0; i < appliedEffects.Count; i++)
        {
            appliedEffects[i].TimeLeft -= Time.deltaTime;
            if (appliedEffects[i].TimeLeft <= 0)
            {
                Stats.SubstactStats(appliedEffects[i].EffectStats);
                effectsToDelete.Push(appliedEffects[i]);
            }
        }
        foreach (var toDelete in effectsToDelete) appliedEffects.Remove(toDelete);
        effectsToDelete.Clear();
    }
    private void Regeneration()
    {
        if (Stats.CurrentHP < Stats.MaxHP) Stats.CurrentHP += Stats.Regeneration * Time.deltaTime;
        if (Stats.CurrentMana < Stats.MaxMana) Stats.CurrentMana += Stats.ManaRegeneration * Time.deltaTime;
    }

    private void OnKillMethod(Unit KilledUnit)
    {
        if (behavior != null)
        {

        }
    }
    private void GetBehavior()
    {
        if (behaviorName == "CaveDweller") behavior = new Behaviours.CaveDwellerBehaviour(this);
        else if (behaviorName == "Peaceful") behavior = new Behaviours.PeacefulBehavior(this);
        else if (behaviorName == "ProvokedHunter") behavior = new Behaviours.ProvokedHunterBehavior(this);
        else if (behaviorName == "BlindRoamer") behavior = new Behaviours.BlindRoamerBehavior(this);
        else if (behaviorName == "BigCaveDweller") behavior = new Behaviours.BigCaveDwellerBehavior(this);
    }
    private void GetBodyType()
    {
        if (bodyTypeName == BodyTypeName.Normal) bodyType = new Body1X();
        else if (bodyTypeName == BodyTypeName.X2Top) bodyType = new Body2X();
        else if (bodyTypeName == BodyTypeName.X4) bodyType = new Body4X();
        else if (bodyTypeName == BodyTypeName.X6) bodyType = new Body6X();
        else if (bodyTypeName == BodyTypeName.X9) bodyType = new Body9X();
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
        public float MaxMana;
        public float CurrentHP 
        { get { return currentHP; } 
          set { currentHP = value; }
        }
        public float CurrentMana
        {
            get { return currentMana; }
            set { currentMana = value; }
        }
        public float Damage;
        public float Regeneration;
        public float ManaRegeneration;
        public float MoveSpeed;
        public float AttackSpeed;
        public float AttackRange;
        public float VisionRadius;

        public byte CollisionRadius;

        [SerializeField]
        private float currentHP;
        [SerializeField]
        private float currentMana;

        public void CombineStats(UnitStats AdditionalStats)
        {
            if (AdditionalStats.attackType != AttackType.None) attackType = AdditionalStats.attackType;

            MaxHP += AdditionalStats.MaxHP;
            MaxMana += AdditionalStats.MaxMana;
            CurrentHP += AdditionalStats.CurrentHP;
            CurrentMana += AdditionalStats.CurrentMana;
            Damage += AdditionalStats.Damage;
            Regeneration += AdditionalStats.Regeneration;
            ManaRegeneration += AdditionalStats.ManaRegeneration;
            MoveSpeed += AdditionalStats.MoveSpeed;
            AttackSpeed += AdditionalStats.AttackSpeed;
            AttackRange += AdditionalStats.AttackRange;
            VisionRadius += AdditionalStats.VisionRadius;
        }
        public void SubstactStats(UnitStats SubstractedStats)
        {
            if (SubstractedStats.attackType != AttackType.None) attackType = AttackType.Melee;

            MaxHP -= SubstractedStats.MaxHP;
            MaxMana -= SubstractedStats.MaxMana;
            CurrentHP -= SubstractedStats.CurrentHP;
            CurrentMana -= SubstractedStats.CurrentMana;
            Damage -= SubstractedStats.Damage;
            Regeneration -= SubstractedStats.Regeneration;
            ManaRegeneration -= SubstractedStats.ManaRegeneration;
            MoveSpeed -= SubstractedStats.MoveSpeed;
            AttackSpeed -= SubstractedStats.AttackSpeed;
            AttackRange -= SubstractedStats.AttackRange;
            VisionRadius -= SubstractedStats.VisionRadius;
        }
    }

    private class AppliedEffect
    {
        public UnitStats EffectStats;
        public float TimeLeft = 2;
        public AppliedEffect(UnitStats stats, float duration)
        {
            EffectStats = stats;
            TimeLeft = duration;
        }
    }
    public enum AttackType : byte
    {
        Melee = 0,
        Ranged = 1,
        None = 2
    }
    public enum BodyTypeName : byte
    {
        Normal = 0,
        X2Top = 1,
        X4 = 2,
        X6 = 3,
        X9 = 4
    }
}
