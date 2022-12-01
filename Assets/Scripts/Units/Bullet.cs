using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float Damage = 20;
    private Unit ownerUnit;
    private float Speed = 10f;
    private float LifeTime = 3.5f;
    [SerializeField]
    private Rigidbody2D rigidbody;
    void Start()
    {
        StartCoroutine(BulletTimeDeathCoroutine());
    }
    public void SetBulletValues(Shooting shooting, Unit shootingUnit)
    {
        Damage = shootingUnit.Stats.Damage;
        ownerUnit = shootingUnit;
        Speed = shooting.BulletSpeed;
        LifeTime = shooting.BulletLifeTime;
    }
    IEnumerator BulletTimeDeathCoroutine()
    {
        yield return new WaitForSeconds(LifeTime);
        Destroy(gameObject);
    }
    private void FixedUpdate()
    {
        rigidbody.velocity = transform.up * Speed;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (ownerUnit.gameObject.tag == "Player" && collision.gameObject.tag == "Creep")
        {
            collision.gameObject.GetComponent<Unit>().GetDamage(Damage, ownerUnit);
        }
        else if (ownerUnit.gameObject.tag == "Creep" && collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Unit>().GetDamage(Damage, ownerUnit);
        }
        Destroy(gameObject);
    }

}
