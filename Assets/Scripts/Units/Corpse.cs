using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;

namespace UI.InventoryLogic
{
    public class Corpse : MonoBehaviour
    {
        public List<Item> CarriedItems = new List<Item>();
        public void InitializeCorpse(Vector3 deadRotation, Color deadColor, List<Item> loot)
        {
            transform.eulerAngles = deadRotation;
            GetComponent<SpriteRenderer>().color = deadColor;
            CarriedItems = loot;
        }
        private void OnMouseDown()
        {
            VisualizeLootInUI();
        }
        private void VisualizeLootInUI()
        {
            //removing all items from previous loot source
            Stack<ToolbarItem> allItems = new Stack<ToolbarItem>();
            foreach (var item in UIManager.Singleton.externalInventory.visualizedItems)
            {
                allItems.Push(item);
            }
            foreach (ToolbarItem item in allItems)
            {
                UIManager.Singleton.externalInventory.RemoveItem(item);
            }
            //adding current loot
            foreach (var item in CarriedItems)
            {
                UIManager.Singleton.externalInventory.CreateItem(item);
            }
            UIManager.Singleton.panel_ExternalInventory.SetActive(true);
        }
    }
}