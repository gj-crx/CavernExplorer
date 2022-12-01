using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviours
{
    public class SingleHitbox : MonoBehaviour
    {
        private Unit attackerUnit;
        private Fighting fighting;
        public List<string> TargetsTagsToDamage = new List<string>();

        void Start()
        {
            attackerUnit = transform.parent.GetComponent<Unit>();
            fighting = transform.parent.GetComponent<Fighting>();
        }


        private void OnTriggerStay2D(Collider2D collision)
        {
            if (fighting.ReadyToHit && attackerUnit.Stats.attackType == Unit.AttackType.Melee && TargetsTagsToDamage.Contains(collision.gameObject.tag))
            {
                fighting.ReadyToHit = false;
                collision.gameObject.GetComponent<Unit>().GetDamage(attackerUnit.Stats.Damage, attackerUnit);
            }
        }
    }
}
