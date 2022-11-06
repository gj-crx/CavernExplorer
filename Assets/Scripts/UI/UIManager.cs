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
        public GameObject panel_HealthBar;
        public GameObject panel_Toolbar;


        [Header("Second elements")]
        public Inventory PlayerInventory;
        public Inventory ExternalInventory = null;
        [SerializeField]
        private List<GameObject> DialoguePanels = new List<GameObject>();
        public GameObject GenerationProgressBar;

        private Dictionary<string, GameObject> dialoguePanelsDictionary = new Dictionary<string, GameObject>();


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
        public void DialogueActivity(string dialogueName, bool dialogueStatus)
        {
            dialoguePanelsDictionary[dialogueName].SetActive(dialogueStatus);
        }
        public void CloseAllDialogues()
        {
            foreach (var dialogue in DialoguePanels) dialogue.SetActive(false);
        }

    }
}
