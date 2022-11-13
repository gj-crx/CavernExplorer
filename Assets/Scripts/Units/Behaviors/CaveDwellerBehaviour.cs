using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Behaviours 
{

    public class CaveDwellerBehaviour : IBehavior
    {
        private Unit unit;
        private Fighting fighting;

        public bool Active { get; set; } = true;
        public bool HaveExternalOrder { get; set; } = false;

        private List<Unit> possibleTargets = new List<Unit>();

        public CaveDwellerBehaviour(Unit RelatedUnit)
        {
            unit = RelatedUnit;
            fighting = unit.gameObject.GetComponent<Fighting>();
            possibleTargets.Add(GameManager.LocalPlayerHeroUnit);

        }

        public void Clear()
        {
            Active = false;
            unit = null;
        }

        public void BehaviorInteraction()
        {
            if (Active == false) return;

            fighting.CurrentTarget = GetNearestTarget();
            if (fighting.CurrentTarget != null) unit.unitMovement.GetWayTarget(fighting.CurrentTarget);

        }
        private Unit GetNearestTarget()
        {
            float MinimalDistance = unit.Stats.VisionRadius;
            Unit MinimalDistanceObject = null;

            foreach (var CurrentObject in possibleTargets)
            {
                float CurrentDistance = Vector2.Distance(CurrentObject.LastNonTransformPosition, unit.LastNonTransformPosition);
                if (CurrentDistance < MinimalDistance)
                {
                    CurrentDistance = MinimalDistance;
                    MinimalDistanceObject = CurrentObject;
                }
            }
            return MinimalDistanceObject;
        }

        private Vector3 GetEscapePosition()
        {
            return Vector3.zero;
        }
    }
}
