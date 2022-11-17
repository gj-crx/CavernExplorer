using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace UI.Indicators
{
    public class PlayerHealthBar : MonoBehaviour, IHealthBar
    {
        private Text HPShowText;
        private Image FillerImage;

        private void Awake()
        {
            FillerImage = transform.Find("Bar").Find("Filler").GetComponent<Image>();
            HPShowText = transform.Find("Bar").Find("Filler").Find("Text").GetComponent<Text>();
        }

        public void ShowHealth(float currentHP, float maxHP)
        {
            FillerImage.fillAmount = currentHP / maxHP;
            HPShowText.text = currentHP + "/" + maxHP;
        }

        public void TurnOff()
        {

        }
    }
}
