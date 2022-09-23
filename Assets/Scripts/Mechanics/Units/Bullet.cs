using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed = 10f;
    [SerializeField]
    private Rigidbody2D rigidbody;
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        rigidbody.AddForce(transform.up * Speed);
    }
}
