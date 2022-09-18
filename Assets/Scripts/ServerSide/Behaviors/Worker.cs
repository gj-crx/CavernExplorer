using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;
using System.Threading.Tasks;
using System.Threading;

namespace ServerSideLogic.Behaviors
{
    public class Worker : IBehavior
    {


        private Fighting fighting;
        private Unit unit;

        private bool InventoryFull = false;
        private float MaximumTreeSearchingDistance = 75;

        public bool Active { get; set; } = true;
        public bool HaveOrder { get; set; } = false;

        public int CurrentTargetID 
        {   get 
            {
                if (fighting.CurrentTarget != null) return fighting.CurrentTarget.ID;
                else return -1;
            }
            set
            {
                fighting.CurrentTarget = null;
            }
        }

        public Worker(Unit unit, Fighting fighting)
        {
            this.unit = unit;
            this.fighting = fighting;

        }

        public async Task StartIterations(int ActualDelay, int RandomizedPreDelay = 0)
        {
            await Task.Delay(RandomizedPreDelay);
            while (unit.game.StillRunning)
            {
                Actions(ActualDelay);
                await Task.Delay(ActualDelay);
            }
        }
        private void Actions(int DelayedTime)
        {
            if (HaveOrder == false)
            {
                if (InventoryFull == false)
                {
                    GatherTrees();
                    fighting.FightingControlling(DelayedTime);
                }
                else
                {
                    ReturnResources();
                }
            }
        }

        public void OnUnitKillDelegated(Unit victim = null)
        {
            for (int i = 0; i < unit.ResourcesCarried.Length; i++)
            {
                //  Debug.Log(unit.ResourcesCarried.Length + " " + i + " " + TypesData.AllUnitTypes[victim.UnitTypeID].ResourcesGivenOnKilled.Length);
                unit.ResourcesCarried[i] += victim.Type.Stats.ResourcesGivenOnKilled[i];
                if (unit.ResourcesCarried[i] >= unit.Type.Stats.ResourcesCarriedMaximum)
                {
                    InventoryFull = true;
                    unit.ResourcesCarried[i] = unit.Type.Stats.ResourcesCarriedMaximum;
                }
            }
        }
        private Unit FindNearestTree()
        {
            return UnitLogic.FindNearestObject(unit.position, unit.game.DB.Trees, MaximumTreeSearchingDistance);
        }
        private void TransferResources(Unit target, float TransferingDistance = 4.5f)
        {
            if (Vector3.Distance(unit.position, target.position) < TransferingDistance)
            {
                for (int i = 0; i < unit.ResourcesCarried.Length; i++)
                {
                    unit.state.ResourcesAmount[i] += unit.ResourcesCarried[i];
                    unit.ResourcesCarried[i] = 0;
                }
                InventoryFull = false;
                Actions(250);
            }
        }
        private void GatherTrees()
        {
            Unit tree = FindNearestTree();
            if (tree != null)
            {
                if (unit.GetWayTarget(tree.PositionNextToUnit(unit.position)))
                {
                    fighting.CurrentTarget = tree;
                }

            }
            else
            { //roaming
                Debug.Log("no trees");
            }
        }
        private void ReturnResources()
        {
            unit.game.pf.GetWayPath(unit, unit.state.Townhall.PositionNextToUnit(unit.position), 3);
            TransferResources(unit.state.Townhall);
        }
        public void Clear()
        {
            unit = null;
        }

        public void BehaviorAction()
        {
            Actions(250);
        }
    }
}
