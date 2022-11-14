using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Items;

namespace UI.InventoryLogic {
    [System.Serializable]
    public class ExternalInventory : Inventory
    {
        public Corpse LootSource;

        public override void  MoveItem(ToolbarItem movedItem, Inventory moveTo)
        {
            movedItem.transform.SetParent(moveTo.UIGrid.transform);
            if (moveTo.visualizedItems.Contains(movedItem) == false)
            {
                moveTo.visualizedItems.Add(movedItem);
                if (LootSource != null) LootSource.CarriedItems.RemoveAt(visualizedItems.IndexOf(movedItem));
                visualizedItems.Remove(movedItem);
                movedItem.inventory = moveTo;
            }
        }
        

        public void TransferAllitems()
        {
            foreach (var item in visualizedItems)
            {
                MoveItem(item, UIManager.Singleton.playerInventory);
            }
        }
    }
}
