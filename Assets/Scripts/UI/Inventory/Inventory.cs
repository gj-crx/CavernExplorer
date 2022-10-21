using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Items;

namespace UI.InventoryLogic
{
    public class Inventory : MonoBehaviour
    {
        [HideInInspector]
        public GameObject UIGrid;

        public Unit CarrierOfItems = null;

        public List<Item> CarriedItems = new List<Item>();
        public Item[] ItemsInEquipmentSlots = new Item[6];

        private void Awake()
        {
            UIGrid = transform.Find("ItemGrid").gameObject;
        }
        private void Start()
        {
            if (CarrierOfItems != null)
            {
                StartingItemsInitializer();
            }
        }


        public void AddItem(Item NewlyAddedItem, bool AddToToolbar = false)
        {
            GameObject ItemObject = GameObject.Instantiate(PrefabManager.Singleton.ItemPrefab);
            if (AddToToolbar)
            {
                ItemObject.transform.SetParent(UIManager.Singleton.panel_Toolbar.transform.Find("ItemGrid"));
            }
            else
            {
                ItemObject.transform.SetParent(UIGrid.transform);
            }
            ItemObject.transform.Find("Icon").GetComponent<Image>().sprite = NewlyAddedItem.Icon;
            ItemObject.transform.Find("Icon").GetComponent<Image>().color = new Color(1, 1, 1);
            ItemObject.GetComponent<ToolbarItem>().RepresentedItem = NewlyAddedItem;

            

            CarriedItems.Add(NewlyAddedItem);
        }
        public void ApplyItem(Item ReferenceItem)
        {
            if (ItemsInEquipmentSlots[(byte)ReferenceItem.UsedSlot] != null)
            {
                DisapplyItem(ItemsInEquipmentSlots[(byte)ReferenceItem.UsedSlot]);
            }
            if (ReferenceItem.UsedSlot != EquipmentSlot.None) ItemsInEquipmentSlots[(byte)ReferenceItem.UsedSlot] = ReferenceItem;
            CarrierOfItems.Stats.CombineStats(ReferenceItem.ItemStats);
        }
        public void DisapplyItem(Item ReferenceItem)
        {
            Debug.Log("Item disapplied " + ReferenceItem.ItemName);
            ItemsInEquipmentSlots[(byte)ReferenceItem.UsedSlot] = null;
            CarrierOfItems.Stats.SubstactStats(ReferenceItem.ItemStats);
        }
        public void StartingItemsInitializer()
        {
            foreach (var StartingItem in GameSettings.Singleton.startingCharacterAsset.StartingItems)
            {
                AddItem(StartingItem, true);
                ApplyItem(StartingItem);
            }
        }

        public enum EquipmentSlot : byte
        {
            Helmet = 0,
            Chestplates = 1,
            Leggings = 2,
            Boots = 3,
            LeftHand = 4,
            RightHand = 5,
            None = 6
        }
    }
}
