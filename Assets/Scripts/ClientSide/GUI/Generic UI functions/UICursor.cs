using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientSideLogic.UI
{
    public class UICursor : MonoBehaviour
    {
        void Update()
        {
            if (UIPlayerActions.PhoneControls)
            {
                transform.position = Input.GetTouch(0).position;
            }
            else
            {
                transform.position = Input.mousePosition;
            }
        }
    }
}
