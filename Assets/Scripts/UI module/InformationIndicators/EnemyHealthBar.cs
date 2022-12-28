using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



namespace UI.Indicators {
    public class EnemyHealthBar : MonoBehaviour, IHealthBar
    {
        public Color LowHPColor;
        public Color HighHPColor;
        public float UpdateInterval = 0.25f;

        [SerializeField]
        private SpriteRenderer hitPointsBar;
        private GameObject background;
        private Unit unitToShow;

        private void Start()
        {
            background = transform.Find("Background").gameObject;
            unitToShow = transform.parent.GetComponent<Unit>();
            StartCoroutine(UpdateStatsCoroutine());
        }


        public void ShowHealth(float currentHP, float maxHP, float recievedDamage)
        {
            hitPointsBar.gameObject.SetActive(currentHP < maxHP);
            background.SetActive(currentHP < maxHP);
            float percentageHP = currentHP / maxHP;
            float xOffset = (1 - percentageHP) / 2.0f;
            hitPointsBar.transform.localScale = new Vector3(percentageHP, 1, 1);
            hitPointsBar.transform.localPosition = new Vector3(xOffset * -1, hitPointsBar.transform.localPosition.y, hitPointsBar.transform.localPosition.z);
        }

        public void ShowMana(float currentMana, float maxMana, float consumedMana)
        {
            
        }

        private IEnumerator UpdateStatsCoroutine()
        {
            while (unitToShow != null)
            {
                if (unitToShow.gameObject.activeInHierarchy)
                {
                    ShowHealth(unitToShow.Stats.CurrentHP, unitToShow.Stats.MaxHP, 0);
                    ShowMana(unitToShow.Stats.CurrentMana, unitToShow.Stats.MaxMana, 0);
                }
                yield return new WaitForSeconds(UpdateInterval);
            }
        }

        public void TurnOff()
        {
            gameObject.SetActive(false);
        }
    }
}
