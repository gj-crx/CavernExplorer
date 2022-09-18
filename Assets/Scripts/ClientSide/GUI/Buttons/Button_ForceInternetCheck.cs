using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClientSideLogic.UI.Buttons
{
    public class Button_ForceInternetCheck : MonoBehaviour
    {
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(ForceMultiplayerConnectionCheck);
        }
        private void ForceMultiplayerConnectionCheck()
        {
            if (UIPlayerActions.LocalPlayer == null)
            {
                UIController.Singleton.ShowConnectionStatus(false);
            }
            else
            {
                UIController.Singleton.ShowConnectionStatus(true);
            }
        }
    }
}
