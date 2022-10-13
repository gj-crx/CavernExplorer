using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Items;

namespace UI.InventoryLogic
{
    public class ToolbarItem : MonoBehaviour
    {
        public Item RepresentedItem;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(UseItem);
        }


        public void UseItem()
        {
            Debug.Log(RepresentedItem.ItemName + " is applied " + RepresentedItem.ItemStats.attackType);
            UIManager.Singleton.PlayerInventory.ApplyItem(RepresentedItem);
        }
    }
}
