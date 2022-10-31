using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI {
    public class UIScenario : MonoBehaviour
    {

        public void StartGeneration()
        {
            GameManager.MapGenerator.GenerateMap();
            UIManager.Singleton.GenerationProgressBar.SetActive(true);
            StartCoroutine(GenerationEndingWaiterCoroutine());
        }
        private IEnumerator GenerationEndingWaiterCoroutine()
        {
            while (GameManager.GameIsRunning)
            {
                Debug.Log("check");
                if (GameManager.MapGenerator.GenerationCompleted)
                {
                    break;
                }
                else yield return null;
            }
            UIManager.Singleton.GenerationProgressBar.SetActive(false);
            UIManager.Singleton.PreGameUI.SetActive(false);
            UIManager.Singleton.InGameUI.SetActive(true);
        }
    }
}
