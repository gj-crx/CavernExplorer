using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generation;

namespace UI
{
    public class GateEntry : MonoBehaviour
    {

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {

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
