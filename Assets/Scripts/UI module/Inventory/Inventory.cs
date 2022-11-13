using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;
using UnityEngine.UI;

namespace UI.InventoryLogic
{
    public abstract class Inventory
    {
        public List<ToolbarItem> visualizedItems = new List<ToolbarItem>();
        public GameObject UIGrid;


        public void CreateItem(Item newItem)
        {
            ToolbarItem createdItem = GameObject.Instantiate(PrefabManager.Singleton.ItemPrefab).GetComponent<ToolbarItem>();
            createdItem.transform.SetParent(UIGrid.transform);
            createdItem.inventory = this;

            createdItem.transform.Find("Icon").GetComponent<Image>().sprite = newItem.Icon;
            createdItem.transform.Find("Icon").GetComponent<Image>().color = new Color(1, 1, 1);

            createdItem.RepresentedItem = newItem;
            visualizedItems.Add(createdItem);
        }
        public void RemoveItem(ToolbarItem removedItem)
        {
            visualizedItems.Remove(removedItem);
            GameObject.Destroy(removedItem.gameObject);
        }
        public abstract void MoveItem(ToolbarItem movedItem, Inventory moveTo);


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
