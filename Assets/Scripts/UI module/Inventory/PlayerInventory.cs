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

                moveTo.RecieveItem(movedItem);
            }
        }
        

        public void ApplyItem(ToolbarItem itemToApply)
        {
            if (itemToApply.RepresentedItem.UsedSlot != EquipmentSlot.None && itemToApply.RepresentedItem.UsedSlot != EquipmentSlot.RightHand)
            { //item uses a slot
                //checking slot before equiping item
                if (EquipmentSlots[(byte)itemToApply.RepresentedItem.UsedSlot].RepresentedItem != null && EquipmentSlots[(byte)itemToApply.RepresentedItem.UsedSlot].RepresentedItem.ItemName != "Undefined item")
                {
                    DisapplyItem(EquipmentSlots[(byte)itemToApply.RepresentedItem.UsedSlot]);
                }
                TransferItemToEquipmentSlot(itemToApply, EquipmentSlots[(byte)itemToApply.RepresentedItem.UsedSlot]);
                EquipmentSlots[(byte)itemToApply.RepresentedItem.UsedSlot].RepresentedItem = itemToApply.RepresentedItem;
                GameObject.Destroy(itemToApply.gameObject);
                GameManager.playerControls.PlayerCharacterUnit.Stats.CombineStats(itemToApply.RepresentedItem.ItemStats);
            }
            else if (itemToApply.RepresentedItem.UsedSlot == EquipmentSlot.RightHand)
            { //weapons
                if (EquipmentSlots[(byte)itemToApply.RepresentedItem.UsedSlot].RepresentedItem != null && EquipmentSlots[(byte)itemToApply.RepresentedItem.UsedSlot].RepresentedItem.ItemName != "Undefined item")
                {
                    DisapplyItem(EquipmentSlots[(byte)itemToApply.RepresentedItem.UsedSlot]);
                }
                itemToApply.transform.SetParent(UIManager.Singleton.panel_Toolbar.transform.Find("ItemGrid"));
                EquipmentSlots[(byte)itemToApply.RepresentedItem.UsedSlot].RepresentedItem = itemToApply.RepresentedItem;
                GameManager.playerControls.PlayerCharacterUnit.Stats.CombineStats(itemToApply.RepresentedItem.ItemStats);
            }
            else if (itemToApply.RepresentedItem.UsedSlot == EquipmentSlot.None)
            {
          //      itemToApply.transform.SetParent(itemToApply.inventory.UIGrid.transform);
            }
            //spells
            if (itemToApply.RepresentedItem.SpellCastOnApply != null)
            {
                if (itemToApply.RepresentedItem.SpellCastOnApply.Method == Spell.CastingMethod.Selfcasted)
                {
                    SpellCastingSystem.CastSpell(itemToApply.RepresentedItem.SpellCastOnApply, new Spell.CastingTarget(GameManager.playerControls.PlayerCharacterUnit));
                    itemToApply.RepresentedItem.ChargeUsed(itemToApply);
                }
                else
                { //inputing target of spell cast
                    SpellCastingSystem.PrepareSpellToCast(itemToApply.RepresentedItem.SpellCastOnApply, new Spell.CastingTarget(GameManager.playerControls.PlayerCharacterUnit));
                }
            }

        }
        public void DisapplyItem(ToolbarItem itemToDisapply)
        {
            if (itemToDisapply.RepresentedItem != null)
            {
                GameManager.playerControls.PlayerCharacterUnit.Stats.SubstactStats(itemToDisapply.RepresentedItem.ItemStats);
                if (itemToDisapply.RepresentedItem.UsedSlot != EquipmentSlot.None && itemToDisapply.RepresentedItem.UsedSlot != EquipmentSlot.RightHand)
                {
                    itemToDisapply.transform.Find("Icon").GetComponent<Image>().sprite = EquipmentSlotsBasicIcons[(byte)itemToDisapply.RepresentedItem.UsedSlot];
                    itemToDisapply.transform.Find("Icon").GetComponent<Image>().color = new Color(1, 1, 1, 0.58f);

                    CreateItem(itemToDisapply.RepresentedItem);
                    itemToDisapply.RepresentedItem = null;
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

        public override void RecieveItem(ToolbarItem newItem)
        {
            
        }
        public void GetItemFromOtherSource(ToolbarItem newItem)
        {
            newItem.transform.SetParent(UIGrid.transform);
            if (visualizedItems.Contains(newItem) == false)
            {
                visualizedItems.Add(newItem);
                newItem.inventory = this;
                visualizedItems.Remove(newItem);
            }
        }
        public void ClearInventory()
        {
            for (int i = 0; i < EquipmentSlots.Length; i++)
            {
                if (EquipmentSlots[i] != null)
                {
                    DisapplyItem(EquipmentSlots[i]);
                }
            }
        }
    }
}
