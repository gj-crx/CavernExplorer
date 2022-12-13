using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UI.InventoryLogic;

namespace UI
{
    public class UIDraggable : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        public DraggableObjectType DraggableType;

        private List<GameObject> PossibleDropSpots = new List<GameObject>();

        private ToolbarItem toolbarItem;

        [HideInInspector]
        public bool BeingDragged = false;

        
        void Start()
        {
            if (DraggableType == DraggableObjectType.ItemInInventory)
            {
                PossibleDropSpots.Add(UIManager.Singleton.playerInventory.SlotsPanel);
                PossibleDropSpots.Add(UIManager.Singleton.playerInventory.UIGrid);
                PossibleDropSpots.Add(UIManager.Singleton.externalInventory.UIGrid);
                PossibleDropSpots.Add(UIManager.Singleton.panel_Toolbar.transform.Find("ItemGrid").gameObject);
                PossibleDropSpots.Add(UIManager.Singleton.panel_ShopItemsGrid);


                toolbarItem = GetComponent<ToolbarItem>();
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            BeingDragged = true;
            transform.SetParent(UIManager.Singleton.gameObject.transform);
            if (UIManager.Singleton.MobileControls == false)
            {
                transform.position = Input.mousePosition;
            }
            else
            {
                transform.position = Input.GetTouch(0).position;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            BeingDragged = false;
            GameObject nearestDrop = GetNearestDropSpot();
            
            if (DraggableType == DraggableObjectType.ItemInInventory)
            {
                if (nearestDrop == UIManager.Singleton.playerInventory.SlotsPanel) //dragged to slots panel
                { //moved to players inventory and applied
                    if (toolbarItem.inventory == null) transform.SetParent(UIManager.Singleton.playerInventory.UIGrid.transform); //putting item back to shop
                    else
                    {
                        toolbarItem.inventory.MoveItem(toolbarItem, UIManager.Singleton.playerInventory);
                        UIManager.Singleton.playerInventory.ApplyItem(toolbarItem);
                    }
                }

                else if (nearestDrop == UIManager.Singleton.panel_Toolbar.transform.Find("ItemGrid").gameObject)
                { //carried from any inventory to toolbar panel
                    if (toolbarItem.inventory == null) transform.SetParent(UIManager.Singleton.playerInventory.UIGrid.transform); //putting item back to shop
                    else toolbarItem.inventory.MoveItemToToolbar(toolbarItem, UIManager.Singleton.playerInventory);
                }

                else if (nearestDrop == UIManager.Singleton.playerInventory.UIGrid)
                { //carried from external inventory or shop to player's inventory

                    if (toolbarItem.inventory == null)
                    { //buying item from shop
                        if (UIShopOverlay.BuyItem(toolbarItem.RepresentedItem))
                        { //we can afford to buy that item, so it transfered to inventory
                            UIManager.Singleton.playerInventory.GetItemFromOtherSource(toolbarItem);
                            transform.SetParent(UIManager.Singleton.playerInventory.UIGrid.transform);
                        }
                        else transform.SetParent(UIManager.Singleton.playerInventory.UIGrid.transform); //putting item back to shop
                    }
                    else UIManager.Singleton.externalInventory.MoveItem(toolbarItem, UIManager.Singleton.playerInventory); //transfering item from external inventory to player's one
                }

                else if (nearestDrop == UIManager.Singleton.externalInventory.UIGrid)
                { //carried from player's inventory to external one
                    UIManager.Singleton.playerInventory.MoveItem(toolbarItem, UIManager.Singleton.externalInventory);
                }
                else if (nearestDrop == UIManager.Singleton.panel_ShopItemsGrid)
                {
                    if (toolbarItem.inventory == UIManager.Singleton.playerInventory)
                    { //selling item from players inventory to shop
                        UIManager.Singleton.playerInventory.Money += (int)(toolbarItem.RepresentedItem.Cost * UIShopOverlay.CurrentShop.BuyingMargin);
                        Destroy(gameObject);
                    }
                    else transform.SetParent(nearestDrop.transform);
                }

            }
        }

        private GameObject GetNearestDropSpot()
        {
            GameObject MinimalDistanceObject = null;
            float MinimalDistance = 10000;
            foreach (var Variant in PossibleDropSpots)
            {
                if (Variant.activeInHierarchy)
                {
                    float CurrentDistance = Vector3.Distance(transform.position, Variant.transform.position);
                    if (CurrentDistance < MinimalDistance)
                    {
                        MinimalDistance = CurrentDistance;
                        MinimalDistanceObject = Variant;
                    }
                }
            }
            return MinimalDistanceObject;
        }

        public enum DraggableObjectType : byte
        {
            ItemInInventory = 0,
        }
        
    }
}
