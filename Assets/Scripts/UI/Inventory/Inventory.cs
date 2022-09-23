using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;

namespace UI.InventoryLogic
{
    public class Inventory : MonoBehaviour
    {
        [HideInInspector]
        public GameObject UIGrid;

        public List<Item> CarriedItems = new List<Item>();

        private void Awake()
        {
            UIGrid = transform.Find("ItemGrid").gameObject;
        }

    }
}
