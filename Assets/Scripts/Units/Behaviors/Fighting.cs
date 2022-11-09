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
        public bool ReadyToHit = false;

        private Unit OwnerUnit;
        private Animator animator;

        void Awake()
        {
            OwnerUnit = GetComponent<Unit>();
            try { animator = GetComponent<Animator>(); } catch { }
        }
        private void Start()
        {
            HitDistanceCheckAsync(325);
        }

        private void Hit(Unit target)
        {
            target.GetDamage(OwnerUnit.Stats.Damage, OwnerUnit);
        }
        private async Task HitDistanceCheckAsync(int CheckIntervalMiliseconds)
        {
            while (GameManager.GameIsRunning)
            {
                if (CurrentTarget != null && Vector3.Distance(transform.position, CurrentTarget.transform.position) < OwnerUnit.Stats.AttackRange)
                {
                    OwnerUnit.unitMovement.Way = null;
                    if (animator != null) animator.SetBool("Attacked", true);
                   // OwnerUnit.MovementHalted = true;
                }
                await Task.Delay(CheckIntervalMiliseconds);
            }
        }
    }
}
