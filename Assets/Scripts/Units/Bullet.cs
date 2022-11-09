using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed = 10f;
    public float LifeTime = 3.5f;
    [SerializeField]
    private Rigidbody2D rigidbody;
    void Start()
    {
        StartCoroutine(BulletTimeDeathCoroutine());
    }

    void Update()
    {
        
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
}
