using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Behaviours
{
    public class BigCaveDwellerBehavior : IBehavior
    {
        private Unit unit;
        private Fighting fighting;

        public bool Active { get; set; } = true;
        public bool HaveExternalOrder { get; set; } = false;

        [SerializeField]
        private Vector3 basePosition;


        public BigCaveDwellerBehavior(Unit RelatedUnit)
        {
            unit = RelatedUnit;
            fighting = unit.gameObject.GetComponent<Fighting>();
            fighting.possibleTargets.Add(GameManager.playerControls.PlayerCharacterUnit);
            basePosition = RelatedUnit.LastNonTransformPosition;
        }

        public void Clear()
        {
            Active = false;
            unit = null;
            fighting = null;
        }

        public void BehaviorInteraction()
        {
            if (Active == false) return;

            float distanceToTarget = fighting.GetNearestTarget();
            if (fighting.CurrentTarget != null)
            {
                if (unit.unitMovement.GetWayTarget(fighting.CurrentTarget) == false) 
                { 
                    unit.unitMovement.GetWayTarget(basePosition); //big creature retreats to it's base position
                }
            }

        }
    }
}