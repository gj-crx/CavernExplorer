using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICloseButton : MonoBehaviour
{
    Button b;
    private void Awake()
    {
        b = GetComponent<Button>();
        b.onClick.AddListener(CloseParent);
    }
    private void CloseParent()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
