using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Behaviours
{
    public class ProvokedHunterBehavior : IBehavior
    {
        private Unit unit;
        private Fighting fighting;

        public bool Active { get; set; } = true;
        public bool HaveExternalOrder { get; set; } = false;

        [SerializeField]
        private Vector3 basePosition;


        public ProvokedHunterBehavior(Unit RelatedUnit)
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
                if (distanceToTarget > 6) unit.unitMovement.RunAway(fighting.CurrentTarget.LastNonTransformPosition);
                else unit.unitMovement.GetWayTarget(fighting.CurrentTarget);
            }


        }
    }
}
