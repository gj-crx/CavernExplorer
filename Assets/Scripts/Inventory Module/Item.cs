using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.InventoryLogic;
using Spells;

namespace Items
{
    [CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item", order = 1)]
    public class Item : ScriptableObject
    {
        public string ItemName = "Undefined item";
        public string ItemDescription = "no description";
        public int Cost = 10;
        public Sprite Icon;
        public Unit.UnitStats ItemStats;
        public Inventory.EquipmentSlot UsedSlot;
        public Spell SpellCastOnApply = null;
        /// <summary>
        /// -1 = infinite charges
        /// </summary>
        public sbyte Charges = -1;


        public void ChargeUsed(ToolbarItem itemRepresentation)
        {
            Charges -= 1;
            if (Charges <= 0) itemRepresentation.inventory.RemoveItem(itemRepresentation);
        }
    }
}
