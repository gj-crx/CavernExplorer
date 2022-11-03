using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviours
{
    public class CreepHitbox : MonoBehaviour
    {
        private Unit attackerUnit;
        private Fighting fighting;
        public bool IsBullet = false;
        void Start()
        {
            attackerUnit = transform.parent.GetComponent<Unit>();
            fighting = transform.parent.GetComponent<Fighting>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log(collision.gameObject.name);
            if (fighting.ReadyToHit && collision.gameObject.tag == "Player")
            {
                fighting.ReadyToHit = false;
                collision.gameObject.GetComponent<Unit>().GetDamage(attackerUnit.Stats.Damage, attackerUnit);
            }
        }
    }
}
