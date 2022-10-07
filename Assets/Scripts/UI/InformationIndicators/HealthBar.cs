using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace UI.Indicators
{
    public class HealthBar : MonoBehaviour
    {
        public Unit unitToShow;

        private Text HPShowText = null;

        private void Awake()
        {
            HPShowText = transform.Find("Filler").Find("Text").GetComponent<Text>();
        }
        void Update()
        {
            HPShowText.text = unitToShow.Stats.CurrentHP.ToString() + "/" + unitToShow.Stats.MaxHP;
        }
    }
}
