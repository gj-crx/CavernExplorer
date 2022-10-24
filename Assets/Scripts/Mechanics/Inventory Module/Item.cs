using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.InventoryLogic;
using Spells;

namespace Items
{
    [System.Serializable]
    public class Item
    {
        public string ItemName = "Undefined item";
        public Sprite Icon;
        public Unit.UnitStats ItemStats;
        public Inventory.EquipmentSlot UsedSlot;
        public Spell SpellCastOnApply = null;
        /// <summary>
        /// -1 = infinite charges
        /// </summary>
        public sbyte Charges = -1;

    }
}
