using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Items;
using Spells;

namespace UI.InventoryLogic
{
    public class Inventory : MonoBehaviour
    {
        [HideInInspector]
        public GameObject UIGrid;
        [HideInInspector]
        public GameObject SlotsPanel;
        [SerializeField]
        private ToolbarItem[] EquipmentSlots = new ToolbarItem[9];
        private Sprite[] EquipmentSlotsBasicIcons = new Sprite[9];

        public Unit CarrierOfItems = null;

        public List<ToolbarItem> CarriedItems = new List<ToolbarItem>();

        private void Awake()
        {
            UIGrid = transform.Find("ItemGrid").gameObject;
            Debug.Log(gameObject.name);
            try
            {
                SlotsPanel = transform.Find("WeaponsAndArmorPanel").Find("SlotsPanel").gameObject;
            } catch { }

            for (int i = 0; i < EquipmentSlots.Length; i++)
            {
                if (EquipmentSlots[i] != null)
                {
                    EquipmentSlotsBasicIcons[i] = EquipmentSlots[i].transform.Find("Icon").GetComponent<Image>().sprite;
                    EquipmentSlots[i].RepresentedItem = null;
                }
            }
        }
        private void Start()
        {
            if (CarrierOfItems != null)
            {
                StartingItemsInitializer();
            }
        }


        public void CreateItem(Item NewlyAddedItem, bool AddToToolbar = false, bool InstantApply = false)
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
            CarriedItems.Add(ItemObject.GetComponent<ToolbarItem>());

            if (InstantApply) ApplyItem(ItemObject.GetComponent<ToolbarItem>());
        }
        public void AddToolbarItem(ToolbarItem itemToAdd, GameObject nearestDrop)
        {
            if (nearestDrop == UIManager.Singleton.panel_Toolbar.transform.Find("ItemGrid").gameObject)
            {
                itemToAdd.transform.SetParent(UIManager.Singleton.panel_Toolbar.transform.Find("ItemGrid"));
            }
            else if (nearestDrop == UIManager.Singleton.PlayerInventory.SlotsPanel)
            {
                itemToAdd.transform.SetParent(UIManager.Singleton.PlayerInventory.SlotsPanel.transform);
                ApplyItem(itemToAdd); //applying equiped armor
            }
            else itemToAdd.transform.SetParent(UIGrid.transform);
            if (CarriedItems.Contains(itemToAdd) == false) CarriedItems.Add(itemToAdd);
        }
        public void RemoveItem(ToolbarItem referenceItem)
        {
            DisapplyItem(referenceItem);
            CarriedItems.Remove(referenceItem);
            referenceItem.transform.SetParent(UIManager.Singleton.transform);
        }
        public void ApplyItem(ToolbarItem ReferenceItem)
        {
            if (ReferenceItem.RepresentedItem.UsedSlot != EquipmentSlot.None)
            { //item uses a slot
                //checking slot before equiping item
                if (EquipmentSlots[(byte)ReferenceItem.RepresentedItem.UsedSlot] != null && EquipmentSlots[(byte)ReferenceItem.RepresentedItem.UsedSlot].RepresentedItem != null) DisapplyItem(EquipmentSlots[(byte)ReferenceItem.RepresentedItem.UsedSlot]);
                EquipmentSlots[(byte)ReferenceItem.RepresentedItem.UsedSlot].RepresentedItem = ReferenceItem.RepresentedItem;
                CarrierOfItems.Stats.CombineStats(ReferenceItem.RepresentedItem.ItemStats);
                VisualizeArmorSlots(ReferenceItem, true);
            }
            else
            {
                if (ReferenceItem.RepresentedItem.SpellCastOnApply is null) SpellCastingSystem.CastSpell(ReferenceItem.RepresentedItem.SpellCastOnApply);
            }
        }
        public void DisapplyItem(ToolbarItem ReferenceItem)
        {
            Debug.Log("Item disapplied " + ReferenceItem.RepresentedItem.ItemName);
            if (EquipmentSlots[(byte)ReferenceItem.RepresentedItem.UsedSlot] == ReferenceItem) //check if item is actually applied in slot
            {
                CarrierOfItems.Stats.SubstactStats(ReferenceItem.RepresentedItem.ItemStats);
                VisualizeArmorSlots(ReferenceItem, false);
                EquipmentSlots[(byte)ReferenceItem.RepresentedItem.UsedSlot].RepresentedItem = null;
            }
        }
        private void VisualizeArmorSlots(ToolbarItem ReferenceItem, bool Apply)
        {
            if (ReferenceItem.RepresentedItem.UsedSlot != EquipmentSlot.RightHand && ReferenceItem.RepresentedItem.UsedSlot != EquipmentSlot.LeftHand && ReferenceItem.RepresentedItem.UsedSlot != EquipmentSlot.None)
            {
                if (Apply)
                {
                    EquipmentSlots[(byte)ReferenceItem.RepresentedItem.UsedSlot].transform.Find("Icon").GetComponent<Image>().sprite = ReferenceItem.RepresentedItem.Icon;
                    EquipmentSlots[(byte)ReferenceItem.RepresentedItem.UsedSlot].transform.Find("Icon").GetComponent<Image>().color = new Color(1, 1, 1, 1);

                    CarriedItems.Remove(ReferenceItem);
                    Destroy(ReferenceItem.gameObject);
                }
                else
                {
                    ReferenceItem.transform.Find("Icon").GetComponent<Image>().sprite = EquipmentSlotsBasicIcons[(byte)ReferenceItem.RepresentedItem.UsedSlot];
                    ReferenceItem.transform.Find("Icon").GetComponent<Image>().color = new Color(1, 1, 1, 0.58f);

                    ToolbarItem ItemInInventory = GameObject.Instantiate(PrefabManager.Singleton.ItemPrefab).GetComponent<ToolbarItem>();
                    ItemInInventory.RepresentedItem = ReferenceItem.RepresentedItem;
                    ItemInInventory.gameObject.transform.SetParent(UIGrid.transform);
                    ItemInInventory.gameObject.transform.Find("Icon").GetComponent<Image>().sprite = ReferenceItem.RepresentedItem.Icon;
                    ItemInInventory.transform.Find("Icon").GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    CarriedItems.Add(ItemInInventory);
                }
            }
        }
        public void TransferAllitems()
        {
            if (this == UIManager.Singleton.ExternalInventory && UIManager.Singleton.PlayerInventory.isActiveAndEnabled)
            {
                foreach (var item in CarriedItems)
                {
                    UIManager.Singleton.PlayerInventory.AddToolbarItem(item, UIManager.Singleton.PlayerInventory.UIGrid);
                }
            }
        }
        public void StartingItemsInitializer()
        {
            foreach (var StartingItem in GameSettings.Singleton.StartingCharacterAsset.StartingItems)
            {
                if (StartingItem.UsedSlot == EquipmentSlot.RightHand || StartingItem.UsedSlot == EquipmentSlot.LeftHand)
                {
                    CreateItem(StartingItem, true, true);
                }
                else
                {
                    CreateItem(StartingItem, false, true);
                }
            }
        }


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
