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

        [Header("UIs")]
        public GameObject PreGameUI;
        public GameObject InGameUI;

        [Header("Main panels")]
        public GameObject panel_Options;
        public GameObject panel_HealthBar;
        public GameObject panel_Toolbar;


        [Header("Second elements")]
        public Inventory PlayerInventory;
        public Inventory ExternalInventory = null;
        public List<GameObject> DialoguePanels = new List<GameObject>();
        public GameObject GenerationProgressBar;

        public Dictionary<string, GameObject> dialoguePanelsDictionary = new Dictionary<string, GameObject>();


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

            foreach (var dialoguePanel in DialoguePanels)
            {
                dialoguePanelsDictionary.Add(dialoguePanel.name, dialoguePanel);
            }

            PreGameUI.SetActive(true);
            InGameUI.gameObject.SetActive(false);
            
        }
        

    }
}
