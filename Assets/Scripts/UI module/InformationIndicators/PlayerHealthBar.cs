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

        [SerializeField]
        private Unit unitToShow;

        private void Awake()
        {
            FillerImage = transform.Find("Bar").Find("Filler").GetComponent<Image>();
            HPShowText = transform.Find("Bar").Find("Filler").Find("Text").GetComponent<Text>();
        }

        void Update()
        {
            ShowHealth();
        }
        public void ShowHealth(float currentHP, float maxHP, float recievedDamage)
        {
            FillerImage.fillAmount = currentHP / maxHP;
            HPShowText.text = currentHP + "/" + maxHP;


            if (recievedDamage > 1) FadingRedScreenEffect.Singleton.ResetColor();
        }
        private void ShowHealth(bool showInternally = true)
        {
            FillerImage.fillAmount = unitToShow.Stats.CurrentHP / unitToShow.Stats.MaxHP;
            HPShowText.text = unitToShow.Stats.CurrentHP + "/" + unitToShow.Stats.MaxHP;
        }

        public void TurnOff()
        {

        }
    }
}
