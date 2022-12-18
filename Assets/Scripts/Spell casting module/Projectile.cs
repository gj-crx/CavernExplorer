using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spells;

public class Projectile : MonoBehaviour
{
    
    private Unit ownerUnit;
    private ProjectileStats stats;


    [SerializeField]
    private Rigidbody2D rigidbody;
    private Spell.Effect effectOnImpact = null;
    private bool collisionActive = true;
    [SerializeField]
    private UnityEngine.Animator animator;
    void Start()
    {
        StartCoroutine(BulletLifeTimerCoroutine());
    }
    public void SetProjectileValues(ProjectileStats stats, Unit shootingUnit, Spell.Effect effectOnImpact = null)
    {
        ownerUnit = shootingUnit;
        this.stats = stats;
        this.effectOnImpact = effectOnImpact;
    }
    public void RotateWithDirection(Vector3 direction)
    {
        float rotationZCord = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotationZCord - 90);
    }
    IEnumerator BulletLifeTimerCoroutine()
    {
        yield return new WaitForSeconds(stats.LifeTime);
        if (collisionActive) Destroy(gameObject);
    }
    IEnumerator ExplosionTimerCoroutine()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
    private void FixedUpdate()
    {
        rigidbody.velocity = transform.up * stats.Speed;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.isTrigger || collisionActive == false || (ownerUnit != null && collision.gameObject == ownerUnit.gameObject)) return;

        Hit(collision);
        if (animator != null)
        {
            animator.Play("Explode");
            collisionActive = false;
            StartCoroutine(ExplosionTimerCoroutine());
            stats.Speed = 0;
        }
        else Destroy(gameObject);
    }
    private void Hit(Collider2D collision)
    {
        if (ownerUnit.gameObject.tag == "Player" && collision.gameObject.tag == "Creep")
        {
            collision.GetComponent<Unit>().GetDamage(stats.Damage, ownerUnit);
            if (effectOnImpact != null) effectOnImpact.CastEffect(new Spell.CastingTarget(collision.GetComponent<Unit>()), ownerUnit);
        }
        else if (ownerUnit.gameObject.tag == "Creep" && collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Unit>().GetDamage(stats.Damage, ownerUnit);
        }
    }
    [System.Serializable]
    public struct ProjectileStats
    {
        [HideInInspector]
        public float Damage;
        public float Speed;
        public float LifeTime;

    }
}
