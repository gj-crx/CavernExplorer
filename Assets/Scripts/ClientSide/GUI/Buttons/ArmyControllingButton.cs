using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClientSideLogic.UI
{
    public class ArmyControllingButton : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(ToggleArmyOrderClicks);
        }
        private void ToggleArmyOrderClicks()
        {
            UIController.Singleton.Cursor_ArmyOrders.SetActive(!UIController.Singleton.Cursor_ArmyOrders.activeSelf);
            UIPlayerActions.IsOrderingArmyMovement = UIController.Singleton.Cursor_ArmyOrders.activeSelf;
        }
    }
}
