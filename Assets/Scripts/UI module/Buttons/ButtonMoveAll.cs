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
            
        }

    }
}
