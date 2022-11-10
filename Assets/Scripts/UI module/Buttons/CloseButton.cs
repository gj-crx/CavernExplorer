using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Buttons
{
    [RequireComponent(typeof(Button))]
    public class CloseButton : MonoBehaviour
    {
        public bool CloseParent = true;
        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(Close);
        }
        public void Close()
        {
            if (CloseParent)
            {
                transform.parent.gameObject.SetActive(false);
            }
            else gameObject.SetActive(false);
        }
    }
}
