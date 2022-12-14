using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;
using UnityEngine.UI;

namespace UI.InventoryLogic
{
    public abstract class Inventory
    {
        /// <summary>
        /// stored in silver, 100 silver = 1 gold
        /// </summary>
        public virtual int Money { get { return money; }
            set { money = value;

            } }
        internal int money = 0;
        public List<ToolbarItem> visualizedItems = new List<ToolbarItem>();
        public GameObject UIGrid;


        public ToolbarItem CreateItem(Item newItem)
        {
            ToolbarItem createdItem = GameObject.Instantiate(PrefabManager.Singleton.ItemPrefab).GetComponent<ToolbarItem>();
            createdItem.transform.SetParent(UIGrid.transform);
            createdItem.inventory = this;

            createdItem.transform.Find("Icon").GetComponent<Image>().sprite = newItem.Icon;
            createdItem.transform.Find("Icon").GetComponent<Image>().color = new Color(1, 1, 1);

            createdItem.RepresentedItem = newItem;
            visualizedItems.Add(createdItem);

            return createdItem;
        }
        public void RemoveItem(ToolbarItem removedItem)
        {
            visualizedItems.Remove(removedItem);
            GameObject.Destroy(removedItem.gameObject);
        }
        public void MoveItemToToolbar(ToolbarItem movedItem, PlayerInventory playerInventory)
        {
            movedItem.transform.SetParent(playerInventory.toolbarPanel.transform);
            if (playerInventory.visualizedItems.Contains(movedItem) == false)
            {
                playerInventory.visualizedItems.Add(movedItem);
                movedItem.inventory = playerInventory;
                visualizedItems.Remove(movedItem);
            }
        }
        public abstract void MoveItem(ToolbarItem movedItem, Inventory moveTo);

        public abstract void RecieveItem(ToolbarItem newItem);


        public enum EquipmentSlot : byte
        {
            Helmet = 0,
            Chestplates = 1,
            Leggings = 2,
            Boots = 3,
            Accesory1 = 4,
            Accesory2 = 5,
            LeftHand = 6,
            RightHand = 7,
            None = 8
        }
    }
}
