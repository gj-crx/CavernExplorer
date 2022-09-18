using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClientSideLogic.UI.Buttons
{
    public class button_OpenCloseSpecificPanel : MonoBehaviour
    {
        public GameObject PanelToInteract;
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(Clicked);
        }

        void Clicked()
        {
            PanelToInteract.SetActive(!PanelToInteract.activeInHierarchy);
            CheckBuildingListPanel();
        }
        void CheckBuildingListPanel()
        {
            if (PanelToInteract == UIController.Singleton.panel_BuildingAndTraining)
            {
                BuildingsList.FormUnitsToTrainList(UIController.Singleton.content_UnitsToBuildList, UIController.Singleton.prefab_UnitBuildingOverview);
            }
        }
    }
}
