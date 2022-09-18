using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ClientSideLogic.UI.Buttons;
using Types;

namespace ClientSideLogic
{
    public static class BuildingsList
    {
        public static List<GameObject> UnitsToTrainPanels = new List<GameObject>();
        public static void FormUnitsToTrainList(GameObject content, GameObject prefab)
        {
            ClearUnitsToTrainList();
            byte ElementsCount = 0;
            foreach (var type in TypesData.AllUnitTypes)
            {
                if (type != null && UIPlayerActions.LocalPlayer.clientSide_ControlledState.AquiredTechnologies.Contains(type.RequiredTechnology))
                {
                    GameObject n = GameObject.Instantiate(prefab);
                    n.transform.SetParent(content.transform);
                    //set Unit name, unit type and icon
                    n.transform.Find("IconAndName").Find("UnitName").Find("Text").GetComponent<Text>().text = type.UnitTypeName;
                    //set UIElement ID
                    Button_BuildOrderClicked button_BuildOrderClicked = n.transform.Find("IconAndName").GetComponent<Button_BuildOrderClicked>();
                    button_BuildOrderClicked.ID = ElementsCount;

                    //set unit stats
                    UnitStatsUI UnitStats = new UnitStatsUI(type);
                    UnitStats.FormUnitStatsUI(n.transform.Find("UnitStats"));

                    ElementsCount++;
                }
            }
        }
        private static void ClearUnitsToTrainList()
        {
            foreach (var panel in UnitsToTrainPanels)
            {
                GameObject.Destroy(panel);
            }
            UnitsToTrainPanels.Clear();
        }
    }
}
