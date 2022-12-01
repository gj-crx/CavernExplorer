using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Behaviours
{
    public class Fighting : MonoBehaviour
    {
        public Unit CurrentTarget;
        public bool AggressionEnabled = true;
        public bool ReadyToHit = false;
        public bool ReadyToShoot = false;

        public List<Unit> possibleTargets = new List<Unit>();

        private Unit ownerUnit;


        void Awake()
        {
            ownerUnit = GetComponent<Unit>();
        }
        private void Start()
        {
            HitDistanceCheckAsync(200);
        }
        private void Update()
        {
            if (ownerUnit == null) Destroy(this);
            if (ReadyToShoot && ownerUnit.Stats.attackType == Unit.AttackType.Ranged && CurrentTarget != null)
            {
                ReadyToShoot = false;
                ownerUnit.shooting.Shoot(ownerUnit, BasicFunctions.GetDirectionBetween2Points(transform.position, CurrentTarget.transform.position));
            }
        }

        public float GetNearestTarget()
        {
            float minimalDistance = ownerUnit.Stats.VisionRadius;
            Unit minimalDistanceObject = null;

            foreach (var CurrentObject in possibleTargets)
            {
                float CurrentDistance = Vector2.Distance(CurrentObject.LastNonTransformPosition, ownerUnit.LastNonTransformPosition);
                if (CurrentDistance < minimalDistance)
                {
                    CurrentDistance = minimalDistance;
                    minimalDistanceObject = CurrentObject;
                }
            }
            CurrentTarget = minimalDistanceObject;
            return minimalDistance;
        }
        private async Task HitDistanceCheckAsync(int CheckIntervalMiliseconds)
        {
            while (GameManager.GameIsRunning)
            {
                if (AggressionEnabled && CurrentTarget != null && Vector3.Distance(transform.position, CurrentTarget.transform.position) < ownerUnit.Stats.AttackRange)
                {
                    ownerUnit.unitMovement.Way = null;
                    if (ownerUnit.animator != null) ownerUnit.animator.SetBool("Attacked", true);
                    // OwnerUnit.MovementHalted = true;
                }
                await Task.Delay(CheckIntervalMiliseconds);
            }
        }
        
        private void OnDestroy()
        {
            if (transform.Find("HitBox") != null) Destroy(transform.Find("HitBox").gameObject);
        }


    }
}
