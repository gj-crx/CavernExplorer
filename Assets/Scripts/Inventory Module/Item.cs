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
