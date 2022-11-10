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

        [HideInInspector]
        public bool BeingDragged = false;

        
        void Start()
        {
            if (DraggableType == DraggableObjectType.ItemInInventory)
            {
                PossibleDropSpots.Add(UIManager.Singleton.PlayerInventory.UIGrid);
                PossibleDropSpots.Add(UIManager.Singleton.ExternalInventory.UIGrid);
                PossibleDropSpots.Add(UIManager.Singleton.panel_Toolbar.transform.Find("ItemGrid").gameObject);
                PossibleDropSpots.Add(UIManager.Singleton.PlayerInventory.SlotsPanel);
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
                if (nearestDrop == UIManager.Singleton.ExternalInventory.UIGrid)
                {
                    UIManager.Singleton.ExternalInventory.AddToolbarItem(GetComponent<ToolbarItem>(), nearestDrop);
                }
                else 
                {
                    UIManager.Singleton.PlayerInventory.AddToolbarItem(GetComponent<ToolbarItem>(), nearestDrop);
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
