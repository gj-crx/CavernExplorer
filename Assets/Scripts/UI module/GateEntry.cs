using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generation;

namespace UI
{
    public class GateEntry : MonoBehaviour
    {
        [SerializeField]
        private float gateEntryDistance = 2.0f;
        private bool gateEntered = false;
        private LevelGate currentApproachedGate = null;

        private void Update()
        {
            if (gateEntered)
            {
                if (Vector3.Distance(currentApproachedGate.Position, GameManager.playerControls.PlayerCharacterUnit.LastNonTransformPosition) > gateEntryDistance)
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
                currentApproachedGate = GetLevelGate(GameManager.playerControls.PlayerCharacterUnit.LastNonTransformPosition);
                if (currentApproachedGate.GoingUp)
                {
                    UIManager.Singleton.dialoguePanelsDictionary["LevelGateUpDialogue"].SetActive(true);
                }
                else
                {
                    UIManager.Singleton.dialoguePanelsDictionary["LevelGateDownDialogue"].SetActive(true);
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
