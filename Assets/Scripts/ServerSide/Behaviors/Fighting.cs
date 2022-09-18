using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ServerSideLogic.Behaviors
{
    public class Fighting
    {
        public Unit CurrentTarget;
        public bool IsHitting = false;

        private Unit OwnerUnit;
        private float timer_Hitting = 0;

        public Fighting(Unit unit)
        {
            this.OwnerUnit = unit;
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
            if (Vector3.Distance(OwnerUnit.position, target.position) < OwnerUnit.Type.Stats.AttackRange)
            {
                if (timer_Hitting > OwnerUnit.Type.Stats.AttackDelay)
                {
                    timer_Hitting = 0;
                    target.CurrentHP -= OwnerUnit.Type.Stats.Damage;
                    if (target.CurrentHP <= 0)
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
