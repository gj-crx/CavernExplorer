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
        public GameObject TownInteractionsUI;

        [Header("Main panels")]
        public GameObject panel_Options;
        public GameObject panel_IndicatorsAndControls;
        public GameObject panel_HealthBar;
        public GameObject panel_Toolbar;
        public GameObject panel_PlayerInventory;
        public GameObject panel_ExternalInventory;
        public GameObject panel_ShopItemsGrid;


        [Header("Secondary elements")]
        public PlayerInventory playerInventory;
        public ExternalInventory externalInventory = null;
        public ItemStatsIndicator itemStatsIndicator;
        public List<GameObject> DialoguePanels = new List<GameObject>();
        public GameObject GenerationProgressBar;
        public UnityEngine.UI.Text MinorErrorText;

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
        public void InitializeInventory()
        {
            foreach (var startingItem in GameSettings.Singleton.StartingCharacterAsset.StartingItems)
            {
                var newToolbarItem = playerInventory.CreateItem(startingItem);
                if (startingItem.UsedSlot != Inventory.EquipmentSlot.None)
                {
                    playerInventory.ApplyItem(newToolbarItem);
                    if (startingItem.UsedSlot == Inventory.EquipmentSlot.RightHand) playerInventory.MoveItemToToolbar(newToolbarItem, playerInventory);
                }
            }
        }


    }
}
