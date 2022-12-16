using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Items;

namespace UI.InventoryLogic
{
    public static class UIShopOverlay
    {
        public static Shop CurrentShop;

        public static void GenerateItemsGrid()
        {
            RemoveAllItems();
            foreach (var itemToSell in CurrentShop.ItemsBeingSold)
            {
                GameObject itemPanel = GameObject.Instantiate(PrefabManager.Singleton.ItemPrefab);
                itemPanel.GetComponent<ToolbarItem>().RepresentedItem = itemToSell;
                itemPanel.transform.SetParent(UIManager.Singleton.panel_ShopItemsGrid.transform);
                itemPanel.transform.Find("Icon").GetComponent<Image>().sprite = itemToSell.Icon;
                itemPanel.GetComponent<ToolbarItem>().GenerateItemInfo();
            }
        }
        public static bool BuyItem(Item itemToBuy)
        {
            if (UIManager.Singleton.playerInventory.Money >= itemToBuy.Cost)
            {
                UIManager.Singleton.playerInventory.Money -= (int)(itemToBuy.Cost);
                return true;
            }
            else
            {
                UIManager.Singleton.MinorErrorText2.gameObject.SetActive(true);
                UIManager.Singleton.MinorErrorText2.text = "Not enough money to buy an item";
            }
            return false;
        }
        public static void SellItem(ToolbarItem itemToSell)
        {
            UIManager.Singleton.playerInventory.Money += (int)(itemToSell.RepresentedItem.Cost * UIShopOverlay.CurrentShop.SellingMarging);
            GameObject.Destroy(itemToSell.gameObject);
        }
        private static void RemoveAllItems()
        {
            var allVisualizedItems = UIManager.Singleton.panel_ShopItemsGrid.GetComponentsInChildren<ToolbarItem>();
            for (int i = 0; i < allVisualizedItems.Length; i++) GameObject.Destroy(allVisualizedItems[i].gameObject);
        }
    }
}
