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
        private List<Item> AppliedItems = new List<Item>();

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
            if (AppliedItems.Contains(ReferenceItem) == false && CarriedItems != null)
            {
                AppliedItems.Add(ReferenceItem);
                CarrierOfItems.Stats.CombineStats(ReferenceItem.ItemStats);
            }
        }
        public void StartingItemsInitializer()
        {
            foreach (var StartingItem in GameSettings.Singleton.startingCharacterAsset.StartingItems)
            {
                AddItem(StartingItem, true);
                ApplyItem(StartingItem);
            }
        }
    }
}
