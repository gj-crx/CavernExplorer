using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DialogueOptionButton : MonoBehaviour
    {
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(OptionSelected);
        }
        public void OptionSelected()
        {

            CloseDialoguePanel();   
        }
        private void CloseDialoguePanel()
        { //content - viewport - scrollrect - panel
            transform.parent.parent.parent.parent.gameObject.SetActive(false);
        }
    }
}
