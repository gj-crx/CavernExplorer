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

        public CaveDwellerBehaviour(Unit RelatedUnit)
        {
            _unit = RelatedUnit;
            _fighting = _unit.gameObject.GetComponent<Fighting>();

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
            siso();
        }
        private async Task BehaviourIterationsAsync(int ActualDelay, int PreDelay)
        {
            await Task.Delay(PreDelay);
            while (Active)
            {
                Debug.Log(Thread.CurrentThread.IsBackground);
                var Target = GetNearestPlayerObject();
                if (Target != null)
                {
                    _unit.unitMovement.GetWayTarget(Target);
                    _fighting.CurrentTarget = Target;
                }
                await Task.Delay(ActualDelay);
            }
        }
        private void siso()
        {
            var Target = GetNearestPlayerObject();
            if (Target != null)
            {
                _unit.unitMovement.GetWayTarget(Target);
                _fighting.CurrentTarget = Target;
            }
        }

        private Unit GetNearestPlayerObject()
        {
            float MinimalDistance = _unit.Stats.VisionRadius;
            Unit MinimalDistanceObject = null;

            foreach (var CurrentObject in GameManager.PlayerRelatedCharacters)
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
