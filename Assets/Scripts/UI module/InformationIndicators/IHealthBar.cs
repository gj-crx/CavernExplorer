using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Indicators
{
    public interface IHealthBar
    {
        public void ShowHealth(float currentHP, float maxHP, float dealtDamage);
        public void ShowMana(float currentMana, float maxMana, float consumedMana);
        public void TurnOff();
    }
}
