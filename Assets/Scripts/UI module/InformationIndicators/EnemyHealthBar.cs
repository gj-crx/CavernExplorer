using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



namespace UI.Indicators {
    public class EnemyHealthBar : MonoBehaviour, IHealthBar
    {
        public Color LowHPColor;
        public Color HighHPColor;

        [SerializeField]
        private SpriteRenderer hitPointsBar;


        public void ShowHealth(float currentHP, float maxHP, float recievedDamage)
        {
            hitPointsBar.gameObject.SetActive(currentHP < maxHP);
            float percentageHP = currentHP / maxHP;
            float xOffset = (1 - percentageHP) / 2.0f;
            hitPointsBar.transform.localScale = new Vector3(percentageHP, 1, 1);
            hitPointsBar.transform.localPosition = new Vector3(xOffset * -1, hitPointsBar.transform.localPosition.y, hitPointsBar.transform.localPosition.z);
        }

        public void TurnOff()
        {
            gameObject.SetActive(false);
        }
    }
}
