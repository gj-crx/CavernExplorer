using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UI.InventoryLogic;

namespace UI
{
    public class UIDraggable : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerUpHandler
    {
        public DraggableObjectType DraggableType;

        private List<GameObject> PossibleDropSpots = new List<GameObject>();

        [HideInInspector]
        public bool BeingDragged = false;

        private bool clicked = false;
        private bool dragEnded = false;

        
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
        private void LateUpdate()
        {
            if (DraggableType == DraggableObjectType.ItemInInventory)
            {
                if (clicked)
                {
                    clicked = false;
                    if (dragEnded) Debug.Log("SOSISISIISISI");
                    else Debug.Log("NESOSINESOSI");
                }
                if (dragEnded)
                {
                    dragEnded = false;
                }
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
            transform.SetParent(null);
            GameObject nearestDrop = GetNearestDropSpot();
            transform.SetParent(nearestDrop.transform);

            if (DraggableType == DraggableObjectType.ItemInInventory && (nearestDrop == UIManager.Singleton.PlayerInventory.SlotsPanel ||
                UIManager.Singleton.panel_Toolbar.transform.Find("ItemGrid").gameObject))
            {
                GetComponent<ToolbarItem>().UseItem();
            }

            dragEnded = true;
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

        public void OnPointerUp(PointerEventData eventData)
        {
            clicked = true;
        }

        public enum DraggableObjectType : byte
        {
            ItemInInventory = 0
        }
    }
}
