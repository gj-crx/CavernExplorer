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

                moveTo.RecieveItem(movedItem);
            }
        }

        public override void RecieveItem(ToolbarItem newItem)
        {
            if (LootSource != null) LootSource.CarriedItems.Add(newItem.RepresentedItem);
        }

        public void TransferAllitems()
        {
            Stack<ToolbarItem> copyOfItemsList = new Stack<ToolbarItem>();
            foreach (var item in visualizedItems) copyOfItemsList.Push(item);
            foreach (var item in copyOfItemsList) MoveItem(item, UIManager.Singleton.playerInventory);
            visualizedItems.Clear();
        }
    }
}
