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
        private Animator _animator;

        void Awake()
        {
            OwnerUnit = GetComponent<Unit>();
            try { _animator = GetComponent<Animator>(); } catch { }
        }
        private void Start()
        {
            HitDistanceCheckCoroutine(325);
        }

        private void Hit(Unit target)
        {
            target.GetDamage(OwnerUnit.Stats.Damage, OwnerUnit);
        }
        private async Task HitDistanceCheckCoroutine(int CheckIntervalMiliseconds)
        {
            while (GameManager.GameIsRunning)
            {
                if (CurrentTarget != null && Vector3.Distance(transform.position, CurrentTarget.transform.position) < OwnerUnit.Stats.AttackRange)
                {
                    OwnerUnit.Way = null;
                    Hit(CurrentTarget);
                    if (_animator != null) _animator.SetBool("Attacked", true);
                    OwnerUnit.MovementHalted = true;
                    await Task.Delay((int)OwnerUnit.Stats.AttackDelay * 1000);
                }
                Debug.Log("wait2");
                await Task.Delay(CheckIntervalMiliseconds);
            }
        }
    }
}
