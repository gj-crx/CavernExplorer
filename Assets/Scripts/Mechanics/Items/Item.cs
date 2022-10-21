using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.InventoryLogic;

namespace Items
{
    [System.Serializable]
    public class Item
    {
        public string ItemName = "Undefined item";
        public Sprite Icon;
        public Unit.UnitStats ItemStats;
        public Inventory.EquipmentSlot UsedSlot;

    }
}
