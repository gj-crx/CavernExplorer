using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI {
    public class UIScenario : MonoBehaviour
    {
        public MenuBackgroundScenario backgroundScenario;
        private bool LateInit = true;
        private float NormalCameraDistance;
        [SerializeField]
        private GameObject blackMask;
        private void LateUpdate ()
        {
            if (LateInit)
            {
                LateInit = false;
                MainMenuBackgroundScenario();
            }
            else if (backgroundScenario.isActive && backgroundScenario.MovingPointsCount > 1)
            {
                backgroundScenario.MoveCameraThroughThePoints();
            }
        }
        public void StartGeneration()
        {
            backgroundScenario.isActive = false;
            GameManager.MapGenerator.GenerateMap(GameSettings.Singleton.GeneratorSettingsPerLevels[GameManager.MapGenerator.CurrentLevelToGenerate]);
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
            GameManager.LocalPlayerHeroUnit.gameObject.SetActive(true);
            Camera.main.orthographicSize = NormalCameraDistance;
            blackMask.SetActive(true);
        }
        private void MainMenuBackgroundScenario()
        {
            GameManager.LocalPlayerHeroUnit.gameObject.SetActive(false);
            NormalCameraDistance = Camera.main.orthographicSize;
            backgroundScenario.GetRandomPoints();
            blackMask.SetActive(false);
            backgroundScenario.isActive = true;
        }
        [System.Serializable]
        public struct MenuBackgroundScenario
        {
            public bool isActive;

            public float CameraMovingPointsRadius;
            public int CameraMovingPointsCount;
            public float CameraMaxDistance;
            public float CameraMinDistance;
            public int MovingPointsCount { get { return CameraMovingPoints.Count; } }

            private List<Vector3> CameraMovingPoints;
            private List<float> RandomCameraDistances;

            public float CameraMovingSpeed;
            private int CurrentPoint;
            private float CurrentMovingTime;

            public void MoveCameraThroughThePoints()
            {
                CurrentMovingTime += Time.deltaTime * CameraMovingSpeed;
                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, CameraMovingPoints[CurrentPoint], CurrentMovingTime);
                Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, RandomCameraDistances[CurrentPoint], CurrentMovingTime);

                if (Vector3.Distance(Camera.main.transform.position, CameraMovingPoints[CurrentPoint]) < 0.5f)
                {
                    CurrentPoint++;
                    CurrentMovingTime = 0;
                    if (CurrentPoint >= CameraMovingPoints.Count) CurrentPoint = 0;
                }
            }
            public void GetRandomPoints()
            {
                CameraMovingPoints = new List<Vector3>();
                RandomCameraDistances = new List<float>();
                float staticZCord = Camera.main.transform.position.z;
                for (int i = 0; i < CameraMovingPointsCount; i++)
                {
                    CameraMovingPoints.Add(new Vector3(Random.Range(-CameraMovingPointsRadius, CameraMovingPointsRadius), Random.Range(-CameraMovingPointsRadius, CameraMovingPointsRadius), staticZCord));
                    RandomCameraDistances.Add(Random.Range(CameraMinDistance, CameraMaxDistance));
                }
            }
        }
    }
}
