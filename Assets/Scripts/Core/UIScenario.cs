using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI {
    public class UIScenario : MonoBehaviour
    {
        public MenuBackgroundScenario backgroundScenario;
        private bool LateInit = true;
        private float NormalCameraDistance;
        private void LateUpdate ()
        {
            if (LateInit)
            {
                LateInit = false;
                MainMenuBackgroundScenario();
            }
            if (backgroundScenario.isActive && backgroundScenario.MovingPointsCount > 1)
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
        }
        private IEnumerator BackgroundGenerationWaiterCoroutine()
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
            backgroundScenario.GetPointsFromSectors();
            backgroundScenario.isActive = true;
        }
        private void MainMenuBackgroundScenario()
        {
            GameManager.LocalPlayerHeroUnit.gameObject.SetActive(false);
            GameManager.MapGenerator.GenerateMap(GameSettings.Singleton.GeneratorSettingsPerLevels[0]);
            NormalCameraDistance = Camera.main.orthographicSize;
            StartCoroutine(BackgroundGenerationWaiterCoroutine());
        }
        [System.Serializable]
        public struct MenuBackgroundScenario
        {
            public bool isActive;

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
            public void GetPointsFromSectors()
            {
                CameraMovingPoints = new List<Vector3>();
                RandomCameraDistances = new List<float>();
                foreach (var sector in GameManager.MapGenerator.NewlyGeneratedSectors)
                {
                    Debug.Log(sector.GetCentralPoint);
                    CameraMovingPoints.Add(BasicFunctions.ToVector3(sector.GetCentralPoint, Camera.main.transform.position.z));
                    RandomCameraDistances.Add(UnityEngine.Random.Range(CameraMinDistance, CameraMaxDistance));
                }
            }
        }
    }
}
