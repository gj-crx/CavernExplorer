using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Behaviours
{
    [RequireComponent(typeof(Unit))]
    public class Fighting : MonoBehaviour
    {
        public Unit CurrentTarget;
        public bool IsHitting = false;

        private Unit OwnerUnit;
        private float timer_Hitting = 0;

        void Awake()
        {
            OwnerUnit = GetComponent<Unit>();
        }
        public void FightingControlling(int TimePassed)
        {
            if (CurrentTarget != null)
            {
                Hit(CurrentTarget, TimePassed);
            }
        }

        private void Hit(Unit target, int TimePassed)
        {
            if (Vector3.Distance(transform.position, target.transform.position) < OwnerUnit.Stats.AttackRange)
            {
                if (timer_Hitting > OwnerUnit.Stats.AttackDelay)
                {
                    timer_Hitting = 0;
                    target.Stats.CurrentHP -= OwnerUnit.Stats.Damage;
                    if (target.Stats.CurrentHP <= 0)
                    {
                        target.Death();
                        CurrentTarget = null;
                        OwnerUnit.OnKilled(target);
                    }
                }
                else
                {
                    timer_Hitting += TimePassed / 1000; 
                }
            }
            else
            {
                timer_Hitting = 0;
            }
        }
    }
}
