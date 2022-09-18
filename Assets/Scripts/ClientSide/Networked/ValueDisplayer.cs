using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ClientSideLogic
{

    public class ValueDisplayer : MonoBehaviour
    {
        UnitClientSide unit;
        TextMesh tm;

        private void Start()
        {
            unit = transform.parent.GetComponent<UnitClientSide>();
            tm = GetComponent<TextMesh>();
        }
        void Update()
        {
            tm.text = unit.currentHP.ToString();
        }
    }
}
