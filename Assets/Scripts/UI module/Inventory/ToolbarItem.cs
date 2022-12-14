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
        public GameObject ItemInfo;

        private UIDraggable draggable;
        [SerializeField]
        private GameObject itemInfoPrefab;

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
        public void GenerateItemInfo()
        {
            ItemInfo = Instantiate(itemInfoPrefab);
            ItemInfo.transform.SetParent(transform);
            ItemInfo.transform.Find("ItemName").GetComponent<Text>().text = RepresentedItem.ItemName;
            ItemInfo.transform.Find("ItemDescription").GetComponent<Text>().text = RepresentedItem.ItemDescription;
            ItemInfo.transform.Find("Price").GetComponent<Text>().text = RepresentedItem.Cost + " silver";
        }
        private void OnTransformParentChanged()
        {
            if (inventory == null)
            {
                if (ItemInfo != null) Destroy(ItemInfo);
            }
        }
    }
}
