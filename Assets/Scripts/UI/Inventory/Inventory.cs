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
        [SerializeField]
        private ToolbarItem[] EquipmentSlots = new ToolbarItem[9];
        private Sprite[] EquipmentSlotsBasicIcons = new Sprite[9];

        public Unit CarrierOfItems = null;

        public List<ToolbarItem> CarriedItems = new List<ToolbarItem>();

        private void Awake()
        {
            UIGrid = transform.Find("ItemGrid").gameObject;

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


        public void AddItem(Item NewlyAddedItem, bool AddToToolbar = false, bool InstantApply = false)
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
        public void RemoveItem(ToolbarItem ReferenceItem)
        {
            DisapplyItem(ReferenceItem);
            Destroy(ReferenceItem.gameObject);
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
        public void StartingItemsInitializer()
        {
            foreach (var StartingItem in GameSettings.Singleton.startingCharacterAsset.StartingItems)
            {
                if (StartingItem.UsedSlot == EquipmentSlot.RightHand || StartingItem.UsedSlot == EquipmentSlot.LeftHand)
                {
                    AddItem(StartingItem, true, true);
                }
                else
                {
                    AddItem(StartingItem, false, true);
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
