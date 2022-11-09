using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Buttons
{
    public class ButtonMoveAll : MonoBehaviour
    {
        [SerializeField] private bool ItemsMovedFromPlayerInventory = false;
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(MoveAll);
        }

        private void MoveAll()
        {
            if (UIManager.Singleton.ExternalInventory != null && UIManager.Singleton.PlayerInventory != null)
            {
                if (ItemsMovedFromPlayerInventory)
                {
                    foreach (var Item in UIManager.Singleton.PlayerInventory.CarriedItems)
                    {
                        UIManager.Singleton.ExternalInventory.CarriedItems.Add(Item);
                        UIManager.Singleton.PlayerInventory.CarriedItems.Remove(Item);
                    }
                }
                else
                {
                    foreach (var Item in UIManager.Singleton.ExternalInventory.CarriedItems)
                    {
                        UIManager.Singleton.ExternalInventory.CarriedItems.Remove(Item);
                        UIManager.Singleton.PlayerInventory.CarriedItems.Add(Item);
                    }
                }
            }
        }

    }
}
