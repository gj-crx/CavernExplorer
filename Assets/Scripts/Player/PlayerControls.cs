using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [HideInInspector]
    public static PlayerControls Singleton;
    [SerializeField]
    private Unit _characterUnit = null;

    [SerializeField]
    private Animator _Animator = null;



    private Vector3 Movement;
    [HideInInspector]
    public bool AttackAnimatinoBeingPlayed = false;

    private void Awake()
    {
        Singleton = this;
    }

    private void Update()
    {
        Movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (Movement.x > 0) transform.eulerAngles = new Vector3(0, -180, 0);
        _Animator.SetFloat("XSpeed", Movement.x);
        _Animator.SetFloat("YSpeed", Movement.y);

        if (Movement.magnitude == 0) _Animator.SetBool("Stopped", true);
        else _Animator.SetBool("Stopped", false);

    }

    void FixedUpdate()
    {
        transform.eulerAngles = new Vector3(0, 0, 0);
        transform.Translate(Movement.normalized * _characterUnit.Stats.MoveSpeed * Time.fixedDeltaTime);
    }

    void AttackInputCheck()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            _Animator.SetBool("Attack", true);
        }

        
    }
}
