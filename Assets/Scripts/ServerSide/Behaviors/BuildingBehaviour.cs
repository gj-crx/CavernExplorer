using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ServerSideLogic.Behaviors
{
    public class BuildingBehaviour : IBehavior
    {
        private Building _building;
        private bool _active = true;
        private bool _haveOrder = false;
        public bool Active { get { return _active; } set { _active = value; } }
        public bool HaveOrder { get { return _haveOrder; } set { _haveOrder = value; } }

        public int CurrentTargetID { get { return -1; } set { } }

        public BuildingBehaviour(Building _building)
        {
            this._building = _building;
        }

        public void Clear()
        {
            _building = null;
        }


        public async Task StartIterations(int ActualDelay, int PreDelay)
        {
            await Task.Delay(PreDelay);
            while (_building.game.StillRunning)
            {
                _building.ControlBuildingProduction();
                await Task.Delay(ActualDelay);
            }
        }

        public void BehaviorAction()
        {
            _building.ControlBuildingProduction();
        }
    }
}
