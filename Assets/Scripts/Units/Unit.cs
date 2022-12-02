using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UI.Indicators;
using Items;


public class Unit : MonoBehaviour
{
    public int ID = -1;
    public bool AIControlled = true;
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
    private UnitGraphicPresets graphicPresets;
    [SerializeField]
    private List<PossibleDrop> possibleDropOnDeath = new List<PossibleDrop>();
    private Unit currentTarget;



    private void Start()
    {
        unitMovement = new UnitMovement(this);
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
    }
    private void FixedUpdate()
    {
        if (AIControlled && MovementHalted == false) unitMovement.WayMoving();
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
    }
  
    private void Chase()
    {
        Vector3 Direction = currentTarget.transform.position - transform.position;

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
    [System.Serializable]
    public struct UnitGraphicPresets
    {
        public Color DeadColor;
    }
    public enum AttackType : byte
    {
        Melee = 0,
        Ranged = 1,
        None = 2
    }
}
