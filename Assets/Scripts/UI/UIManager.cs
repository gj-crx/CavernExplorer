using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.InventoryLogic;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [HideInInspector]
        public bool MobileControls = false;
        [HideInInspector]
        public static UIManager Singleton;



        [Header("Main enteties")]
        public Inventory PlayerInventory;
        public Inventory ExternalInventory = null;


        private void Awake()
        {
            Singleton = this;
            if (SystemInfo.deviceType == DeviceType.Handheld)
            {
                MobileControls = true;
            }
            else
            {
                MobileControls = false;
            }
        }




    }
}