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
                {
                    UIManager.Singleton.playerInventory.ApplyItem(toolbarItem);
                }

                else if (nearestDrop == UIManager.Singleton.panel_Toolbar.transform.Find("ItemGrid").gameObject)
                { //carried from any inventory to toolbar panel
                    toolbarItem.inventory.MoveItemToToolbar(toolbarItem, UIManager.Singleton.playerInventory);
                }

                else if (nearestDrop == UIManager.Singleton.playerInventory.UIGrid)
                { //carried from external inventory to player's one
                    UIManager.Singleton.externalInventory.MoveItem(toolbarItem, UIManager.Singleton.playerInventory);
                }

                else if (nearestDrop == UIManager.Singleton.externalInventory.UIGrid)
                { //carried from player's inventory to external one
                    UIManager.Singleton.playerInventory.MoveItem(toolbarItem, UIManager.Singleton.externalInventory);
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
                    Debug.Log(Variant.name + " " + CurrentDistance);
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
            ItemInInventory = 0
        }
    }
}
