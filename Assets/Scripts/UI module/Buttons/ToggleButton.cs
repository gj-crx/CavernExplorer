using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Buttons
{
    [RequireComponent(typeof(Button))]
    public class ToggleButton : MonoBehaviour
    {
        [SerializeField]
        private GameObject objectToToggle;
        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(Toggle);   
        }
        public void Toggle()
        {
            objectToToggle.SetActive(!objectToToggle.activeSelf);
        }
    }
}
