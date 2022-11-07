using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Behaviours 
{

    public class CaveDwellerBehaviour : IBehavior
    {
        private Unit _unit;
        private Fighting _fighting;

        public bool Active { get; set; } = true;
        public bool HaveExternalOrder { get; set; } = false;

        private List<Unit> possibleTargets = new List<Unit>();

        public CaveDwellerBehaviour(Unit RelatedUnit)
        {
            _unit = RelatedUnit;
            _fighting = _unit.gameObject.GetComponent<Fighting>();
            possibleTargets.Add(GameManager.LocalPlayerHeroUnit);

        }

        public void BehaviorAction()
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            Active = false;
            _unit = null;
        }

        public void StartBehaviourIterations(int ActualDelay, int PreDelay)
        {
            GetNearestTarget();
        }
        private Unit GetNearestTarget()
        {
            float MinimalDistance = _unit.Stats.VisionRadius;
            Unit MinimalDistanceObject = null;

            foreach (var CurrentObject in possibleTargets)
            {
                float CurrentDistance = Vector2.Distance(CurrentObject.LastNonTransformPosition, _unit.LastNonTransformPosition);
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
