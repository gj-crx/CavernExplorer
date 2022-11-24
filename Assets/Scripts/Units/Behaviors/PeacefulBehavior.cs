using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviours
{
    public class PeacefulBehavior : IBehavior
    {
        public bool Active { get; set; } = true;
        public bool HaveExternalOrder { get; set; } = false;

        private Unit unit;
        private Fighting fighting;

        public PeacefulBehavior(Unit RelatedUnit)
        {
            unit = RelatedUnit;
            fighting = unit.gameObject.GetComponent<Fighting>();
            fighting.possibleTargets.Add(GameManager.LocalPlayerHeroUnit);

        }

        public void BehaviorInteraction()
        {
            if (Active == false) return;

            fighting.GetNearestTarget();
            if (fighting.CurrentTarget != null) unit.unitMovement.RunAway(fighting.CurrentTarget.LastNonTransformPosition);
        }

        public void Clear()
        {
            Active = false;
            unit = null;
            fighting = null;
        }
    }
}
