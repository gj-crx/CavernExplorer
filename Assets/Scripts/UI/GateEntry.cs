using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generation;

namespace UI
{
    public class GateEntry : MonoBehaviour
    {
        private bool gateEntered = false;
        private LevelGate currentApproachedGate = null;

        private void Update()
        {
            if (gateEntered)
            {
                if (Vector3.Distance(currentApproachedGate.Position, GameManager.LocalPlayerHeroUnit.LastNonTransformPosition) > 3.5f)
                {
                    UIScenario.Singleton.CloseAllDialogues();
                    gateEntered = false;
                }
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                gateEntered = true;
                currentApproachedGate = GetLevelGate(GameManager.LocalPlayerHeroUnit.LastNonTransformPosition);
                if (currentApproachedGate.GoingUp)
                {
                    UIScenario.Singleton.DialogueActivity("LevelGateUpDialogue", true);
                }
                else
                {
                    UIScenario.Singleton.DialogueActivity("LevelGateDownDialogue", true);
                }
            }
        }
        private LevelGate GetLevelGate(Vector3 playerPosition)
        {
            float minimumDistance = 999999;
            LevelGate minimumDistanceGate = null;
            foreach (var gate in GameManager.map.LevelGates)
            {
                float currentDistance = Vector3.Distance(gate.Position, playerPosition);
                if (currentDistance < minimumDistance)
                {
                    minimumDistance = currentDistance;
                    minimumDistanceGate = gate;
                }
            }
            return minimumDistanceGate;
        }
    }
}
