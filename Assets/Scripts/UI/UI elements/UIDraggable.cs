using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI
{
    public class UIDraggable : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        public DraggableObjectType DraggableType;

        private List<GameObject> PossibleDropSpots = new List<GameObject>();

        
        void Start()
        {
            if (DraggableType == DraggableObjectType.ItemInInventory)
            {
                PossibleDropSpots.Add(UIManager.Singleton.PlayerInventory.UIGrid);
                PossibleDropSpots.Add(UIManager.Singleton.ExternalInventory.UIGrid);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
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
            transform.SetParent(null);
            transform.SetParent(GetNearestDropSpot().transform);
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
            ItemInInventory = 0
        }
    }
}
