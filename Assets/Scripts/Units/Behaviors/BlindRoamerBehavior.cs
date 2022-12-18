using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Behaviours
{
    public class BlindRoamerBehavior : IBehavior
    {
        private Unit unit;
        private Fighting fighting;

        public bool Active { get; set; } = true;
        public bool HaveExternalOrder { get; set; } = false;


        public BlindRoamerBehavior(Unit RelatedUnit)
        {
            unit = RelatedUnit;
            fighting = unit.gameObject.GetComponent<Fighting>();
            fighting.possibleTargets.Add(GameManager.playerControls.PlayerCharacterUnit);

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

            unit.unitMovement.Roam();
        }
    }
}
