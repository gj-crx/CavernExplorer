using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClientSideLogic
{
    namespace UI
    {
        public class UIController : MonoBehaviour
        {
            [HideInInspector]
            public static UIController Singleton;

            [Header("Other components")]
            [SerializeField]
            public BuildingHelper buildingHelper;

            [Header("GUI rects")]
            public GameObject MainMenuGUI;
            public GameObject InGameGUI;
            [Header("Buttons")]
            [SerializeField]
            private Button button_ActionOpenBuildingPanel;
            [Header("Lists content")]
            public GameObject content_UnitsToBuildList;
            public GameObject content_LobbiesList;


            [Header("Panels")]
            public GameObject panel_BuildingAndTraining;
            public GameObject panel_LobbiesMenu;
            public GameObject panel_Resources;
            


            [Header("UI Prefabs")]
            [SerializeField]
            public GameObject prefab_UnitBuildingOverview;
            public GameObject prefab_Lobby;


            [Header("Cursors")]
            public GameObject Cursor_ArmyOrders;

            [Header("Other")]
            [SerializeField]
            private GameObject Alert;
            [SerializeField]
            private GameObject[] ConnectionStatusIndicators = new GameObject[2];


            //HIDEN lists of in game created UI panels
            [HideInInspector]
            public List<GameObject> list_LobbyPanels = new List<GameObject>();

            // [Header("Other")]

            private bool BuildingPanelFormed = false;

            private void Awake()
            {
                Singleton = this;

                buildingHelper = new BuildingHelper();

                MainMenuGUI.SetActive(true);
                InGameGUI.SetActive(false);

            }

            private void Update()
            {
                if (InGameGUI.activeInHierarchy)
                {
                    ControlDisplayers();
                    buildingHelper.ControlBuildingHelper();
                }
            }




            private void ControlDisplayers()
            {
                for (byte i = 0; i < UIPlayerActions.LocalPlayer.clientSide_ControlledState.ResourcesAmount.Length; i++)
                {
                    Text ResourceText = null;
                    try
                    {
                        ResourceText = panel_Resources.transform.Find("Indicator_resource" + i).Find("Text").GetComponent<Text>();
                    }
                    catch { } //there is no such indicator but it's not actually an exeption 
                    if (ResourceText != null)
                    {
                        ResourceText.text = UITextFormatter.CutOffNumericalPart(ResourceText.text) + ((int)UIPlayerActions.LocalPlayer.clientSide_ControlledState.ResourcesAmount[i]).ToString();
                    }
                }
            }

            public void StartInGameGUI()
            {
                MainMenuGUI.SetActive(false);
                InGameGUI.SetActive(true);
            }

            public void ShowConnectionStatus(bool ConnectionStatus)
            {
                if (ConnectionStatus)
                {
                    ConnectionStatusIndicators[0].SetActive(false);
                    ConnectionStatusIndicators[1].SetActive(true);
                }
                else
                {
                    ConnectionStatusIndicators[0].SetActive(true);
                    ConnectionStatusIndicators[1].SetActive(false);
                }
            }
            public void CreateAlert(string AlertText)
            {
                Alert.SetActive(true);
                Alert.transform.Find("Text").GetComponent<Text>().text = AlertText;
            }


        }
    }
}
