using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Behaviours
{
    public class Fighting : MonoBehaviour
    {
        public Unit CurrentTarget;
        public bool IsHitting = false;
        public bool ReadyToHit = false;

        private Unit ownerUnit;
        private Animator animator;


        void Awake()
        {
            ownerUnit = GetComponent<Unit>();
            try { animator = GetComponent<Animator>(); } catch { }
        }
        private void Start()
        {
            HitDistanceCheckAsync(200);
        }
        private void Update()
        {
            if (ownerUnit == null) Destroy(this);
        }

        private void Hit(Unit target)
        {
            target.GetDamage(ownerUnit.Stats.Damage, ownerUnit);
        }
        private async Task HitDistanceCheckAsync(int CheckIntervalMiliseconds)
        {
            while (GameManager.GameIsRunning)
            {
                if (CurrentTarget != null && Vector3.Distance(transform.position, CurrentTarget.transform.position) < ownerUnit.Stats.AttackRange)
                {
                    ownerUnit.unitMovement.Way = null;
                    if (animator != null) animator.SetBool("Attacked", true);
                   // OwnerUnit.MovementHalted = true;
                }
                await Task.Delay(CheckIntervalMiliseconds);
            }
        }


    }
}
