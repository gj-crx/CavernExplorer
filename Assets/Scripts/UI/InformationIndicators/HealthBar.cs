using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace UI.Indicators
{
    public class HealthBar : MonoBehaviour
    {
        public Unit unitToShow;

        private Text HPShowText;
        private Image FillerImage;

        private void Awake()
        {
            FillerImage = transform.Find("Filler").GetComponent<Image>();
            HPShowText = transform.Find("Filler").Find("Text").GetComponent<Text>();
        }
        void Update()
        {
            FillerImage.fillAmount = unitToShow.Stats.CurrentHP / unitToShow.Stats.MaxHP;
            HPShowText.text = unitToShow.Stats.CurrentHP.ToString() + "/" + unitToShow.Stats.MaxHP;
        }
    }
}
