using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Items;

namespace UI.InventoryLogic
{
    public class ToolbarItem : MonoBehaviour
    {
        public Item RepresentedItem = null;
        [HideInInspector]
        public Inventory inventory;
        public bool IsEquipedArmor = false;

        private UIDraggable draggable;

        private void Start()
        {
            try
            {
                draggable = GetComponent<UIDraggable>();
            }
            catch { }
            GetComponent<Button>().onClick.AddListener(ShowItemInfo);
        }

        public void ShowItemInfo()
        {
            if (transform.parent != UIManager.Singleton.playerInventory.SlotsPanel.transform)
            {
                if (draggable.BeingDragged == false)
                {
                    if (transform.parent == UIManager.Singleton.panel_Toolbar.transform.Find("ItemGrid"))
                    {
                        ClickApply();
                    }
                    else
                    {
                        UIManager.Singleton.itemStatsIndicator.gameObject.SetActive(true);
                        UIManager.Singleton.itemStatsIndicator.FormStatsPanel(RepresentedItem);
                    }
                }
            }
            else
            { //disapply use
               ClickApply();
            }
        }
        public void ClickApply()
        {
            if (draggable != null && draggable.BeingDragged) return;
            if (!IsEquipedArmor)
            {
                UIManager.Singleton.playerInventory.ApplyItem(this);
            }
            else
            {
                if (RepresentedItem != null) UIManager.Singleton.playerInventory.DisapplyItem(this);
            }
        }
    }
}
