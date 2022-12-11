using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace UI.Indicators
{
    public class PlayerHealthBar : MonoBehaviour, IHealthBar
    {
        public float UpdateInterval = 0.7f;
        private Text HPShowText;
        private Text ManaShowText;
        private Image HPFillerImage;
        private Image ManaFillerImage;

        [SerializeField]
        private Unit unitToShow;

        private void Awake()
        {
            HPFillerImage = transform.Find("Bar").Find("Filler").GetComponent<Image>();
            HPShowText = transform.Find("Bar").Find("Filler").Find("Text").GetComponent<Text>();

            ManaFillerImage = transform.Find("ManaBar").Find("Filler").GetComponent<Image>();
        }
        void Start()
        {
            StartCoroutine(UpdateStatsCoroutine());
        }


        public void ShowHealth(float currentHP, float maxHP, float recievedDamage)
        {
            HPFillerImage.fillAmount = currentHP / maxHP;
            HPShowText.text = currentHP + "/" + maxHP;


            if (recievedDamage > 1) FadingRedScreenEffect.Singleton.ResetColor();
        }
        public void ShowMana(float currentMana, float maxMana, float consumedMana)
        {
            throw new System.NotImplementedException();
        }
        private IEnumerator UpdateStatsCoroutine()
        {
            while (unitToShow != null)
            {
                if (unitToShow.gameObject.activeInHierarchy)
                {
                    HPFillerImage.fillAmount = unitToShow.Stats.CurrentHP / unitToShow.Stats.MaxHP;
                    HPShowText.text = unitToShow.Stats.CurrentHP + "/" + unitToShow.Stats.MaxHP;

                    ManaFillerImage.fillAmount = unitToShow.Stats.CurrentMana / unitToShow.Stats.MaxMana;
                    if (ManaShowText != null) ManaShowText.text = unitToShow.Stats.CurrentMana + "/" + unitToShow.Stats.MaxMana;
                }
                yield return new WaitForSeconds(UpdateInterval);
            }
        }

        public void TurnOff()
        {

        }

        
    }
}
