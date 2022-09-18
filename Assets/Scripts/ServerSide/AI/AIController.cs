using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;


namespace ServerSideLogic.AI
{
    public class AIController
    {
        private State _state;
        private AIBuildOrder _AIBuildOrder = new AIBuildOrder();

        public AIController(State _state, int NormalControllingDelay)
        {
            this._state = _state;
            StartControllingAIAsync(Random.Range(0, 600), NormalControllingDelay);
        }

        private async Task StartControllingAIAsync(int PreDelay, int NormalDelay)
        {
            await Task.Delay(PreDelay);
            while (_state.Townhall.game.StillRunning)
            {
                ControlAI();
                await Task.Delay(NormalDelay);
            }
        }
        private void ControlAI()
        {
            _state.GetTrainingBuilding().EnqueueUnitTraining(_AIBuildOrder.GetNextUnitIDToBuild(_state));
        }
    }
}
