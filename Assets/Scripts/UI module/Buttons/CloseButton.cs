using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Buttons
{
    public class CloseButton : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(CloseParent);
        }
        public void CloseParent()
        {
            transform.parent.gameObject.SetActive(false);
        }
    }
}
