using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using Items;

namespace UI {
    public class UIScenario : MonoBehaviour
    {
        [HideInInspector]
        public static UIScenario Singleton;

        public MenuBackgroundScenario backgroundScenario;
        private bool LateInit = true;
        private float NormalCameraDistance;
        [SerializeField]
        private GameObject blackMask;
        [SerializeField]
        private Shop[] shops;
        private void Awake()
        {
            Singleton = this;
        }
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
        public void ButtonStartGame()
        { //entry point of the game
            UIManager.Singleton.PreGameUI.SetActive(false);
            UIManager.Singleton.InGameUI.SetActive(false);
            UIManager.Singleton.TownInteractionsUI.SetActive(true);
        }
        public void ButtonStartGeneration(int gameLevelAdvance) //activated by in game buttons
        {
            backgroundScenario.isActive = false;
            GameManager.MapGenerator.GenerateMap(gameLevelAdvance);
            GameManager.playerControls.gameObject.SetActive(false);

            UIManager.Singleton.InGameUI.SetActive(false);
            UIManager.Singleton.PreGameUI.SetActive(true);
            UIManager.Singleton.panel_Options.SetActive(false);
            UIManager.Singleton.GenerationProgressBar.SetActive(true);

            PlayerControls.RespawnPlayer();

            StartCoroutine(GenerationEndingWaiterCoroutine());
        }
        public void ButtonSetShop(int ShopID)
        {
            InventoryLogic.UIShopOverlay.CurrentShop = shops[ShopID];
            InventoryLogic.UIShopOverlay.GenerateItemsGrid();
        }
        public void ExitToMenu() //activated by in game buttons
        {
            GameManager.playerControls.gameObject.SetActive(false);
            GameManager.MapGenerator.ClearMap();
            UIManager.Singleton.InGameUI.SetActive(false);
            UIManager.Singleton.PreGameUI.SetActive(true);
        }
        public void CloseAllDialogues()
        {
            foreach (var dialogue in UIManager.Singleton.DialoguePanels) dialogue.SetActive(false);
        }
        public void ShowMinorError(string errorText)
        {
            UIManager.Singleton.MinorErrorText.gameObject.SetActive(true);
            UIManager.Singleton.MinorErrorText.text = errorText;
        }
        private IEnumerator GenerationEndingWaiterCoroutine()
        {
            while (GameManager.GameIsRunning)
            {
                if (GameManager.MapGenerator.GenerationCompleted)
                {
                    break;
                }
                else yield return null;
            }
            UIManager.Singleton.panel_Options.SetActive(true);
            UIManager.Singleton.GenerationProgressBar.SetActive(false);
            UIManager.Singleton.PreGameUI.SetActive(false);
            UIManager.Singleton.InGameUI.SetActive(true);
            GameManager.playerControls.gameObject.SetActive(true);
            blackMask.SetActive(true);
            Camera.main.orthographicSize = NormalCameraDistance;
        }
        private void MainMenuBackgroundScenario()
        {
            GameManager.playerControls.gameObject.SetActive(false);
            NormalCameraDistance = Camera.main.orthographicSize;
            Camera.main.orthographicSize = (backgroundScenario.CameraMinDistance + backgroundScenario.CameraMaxDistance) / 2 + 5;
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
