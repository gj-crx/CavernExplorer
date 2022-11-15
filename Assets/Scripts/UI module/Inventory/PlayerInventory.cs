using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Items;
using Spells;

namespace UI.InventoryLogic
{
    [System.Serializable]
    public class PlayerInventory : Inventory
    {
        public GameObject SlotsPanel;
        public GameObject toolbarPanel;
        [SerializeField]
        private ToolbarItem[] EquipmentSlots = new ToolbarItem[9];
        [SerializeField]
        private Sprite[] EquipmentSlotsBasicIcons = new Sprite[9];


        public override void MoveItem(ToolbarItem movedItem, Inventory moveTo)
        {
            movedItem.transform.SetParent(moveTo.UIGrid.transform);
            if (moveTo.visualizedItems.Contains(movedItem) == false)
            {
                moveTo.visualizedItems.Add(movedItem);
                movedItem.inventory = moveTo;
                visualizedItems.Remove(movedItem);
            }
        }
        

        public void ApplyItem(ToolbarItem itemToApply)
        {
            if (itemToApply.RepresentedItem.UsedSlot != EquipmentSlot.None && itemToApply.RepresentedItem.UsedSlot != EquipmentSlot.RightHand)
            { //item uses a slot
                //checking slot before equiping item
                if (EquipmentSlots[(byte)itemToApply.RepresentedItem.UsedSlot] != null && EquipmentSlots[(byte)itemToApply.RepresentedItem.UsedSlot].RepresentedItem.ItemName != "Undefined item")
                {
                    DisapplyItem(EquipmentSlots[(byte)itemToApply.RepresentedItem.UsedSlot]);
                }
                TransferItemToEquipmentSlot(itemToApply, EquipmentSlots[(byte)itemToApply.RepresentedItem.UsedSlot]);
                EquipmentSlots[(byte)itemToApply.RepresentedItem.UsedSlot].RepresentedItem = itemToApply.RepresentedItem;
                GameObject.Destroy(itemToApply.gameObject);
                GameManager.LocalPlayerHeroUnit.Stats.CombineStats(itemToApply.RepresentedItem.ItemStats);
            }
            else if (itemToApply.RepresentedItem.UsedSlot == EquipmentSlot.RightHand)
            {
                if (EquipmentSlots[(byte)itemToApply.RepresentedItem.UsedSlot].RepresentedItem != null && EquipmentSlots[(byte)itemToApply.RepresentedItem.UsedSlot].RepresentedItem.ItemName != "Undefined item")
                {
                    DisapplyItem(EquipmentSlots[(byte)itemToApply.RepresentedItem.UsedSlot]);
                }
                itemToApply.transform.SetParent(UIManager.Singleton.panel_Toolbar.transform.Find("ItemGrid"));
                EquipmentSlots[(byte)itemToApply.RepresentedItem.UsedSlot].RepresentedItem = itemToApply.RepresentedItem;
                GameManager.LocalPlayerHeroUnit.Stats.CombineStats(itemToApply.RepresentedItem.ItemStats);
            }
            else if (itemToApply.RepresentedItem.UsedSlot == EquipmentSlot.None)
            {
                itemToApply.transform.SetParent(itemToApply.inventory.UIGrid.transform);
            }
            if (itemToApply.RepresentedItem.SpellCastOnApply is null == false) SpellCastingSystem.CastSpell(itemToApply.RepresentedItem.SpellCastOnApply);

        }
        public void DisapplyItem(ToolbarItem itemToDisapply)
        {
            if (itemToDisapply.RepresentedItem != null)
            {
                GameManager.LocalPlayerHeroUnit.Stats.SubstactStats(itemToDisapply.RepresentedItem.ItemStats);
                if (itemToDisapply.RepresentedItem.UsedSlot != EquipmentSlot.None && itemToDisapply.RepresentedItem.UsedSlot != EquipmentSlot.RightHand)
                {
                    itemToDisapply.transform.Find("Icon").GetComponent<Image>().sprite = EquipmentSlotsBasicIcons[(byte)itemToDisapply.RepresentedItem.UsedSlot];
                    itemToDisapply.transform.Find("Icon").GetComponent<Image>().color = new Color(1, 1, 1, 0.58f);

                    CreateItem(itemToDisapply.RepresentedItem);
                    itemToDisapply.RepresentedItem = null;
                }
                else if (itemToDisapply.RepresentedItem.UsedSlot == EquipmentSlot.RightHand)
                {
                    GameManager.LocalPlayerHeroUnit.Stats.SubstactStats(itemToDisapply.RepresentedItem.ItemStats);
                }
            }
        }
        public void TransferItemToEquipmentSlot(ToolbarItem itemToTransfer, ToolbarItem equipmentSlot)
        {
            equipmentSlot.RepresentedItem = itemToTransfer.RepresentedItem;
            equipmentSlot.transform.Find("Icon").GetComponent<Image>().sprite = equipmentSlot.RepresentedItem.Icon;
            equipmentSlot.transform.Find("Icon").GetComponent<Image>().color = new Color(1, 1, 1, 1);

            visualizedItems.Remove(itemToTransfer);
            GameObject.Destroy(itemToTransfer);
            visualizedItems.Add(equipmentSlot);
        }
    }
}
