using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;

namespace UI.InventoryLogic
{
    public static class UIShopOverlay
    {
        public static Shop CurrentShop;

        public static bool BuyItem(Item itemToBuy)
        {
            if (UIManager.Singleton.playerInventory.Money >= itemToBuy.Cost * CurrentShop.SellingMarging)
            {
                UIManager.Singleton.playerInventory.Money -= (int)(itemToBuy.Cost * CurrentShop.SellingMarging);
                return true;
            }
            return false;
        }
    }
}
