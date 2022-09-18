using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;
using UnityEngine.UI;

namespace ClientSideLogic
{

    public class UnitStatsUI
    {
        private UnitType UnitType;

        public void FormUnitStatsUI(Transform panel)
        {
            //damage
            Text t = panel.Find("UnitDamage").Find("Text").GetComponent<Text>();
            t.text = UITextFormatter.CutOffNumericalPart(t.text) + UnitType.Stats.Damage;
            //HP
            t = panel.Find("UnitHP").Find("Text").GetComponent<Text>();
            t.text = UITextFormatter.CutOffNumericalPart(t.text) + UnitType.Stats.MaxHP;
            //Resource costs
            for (int i = 0; i < UnitType.Stats.ResourcesCostToBuild.Length; i++)
            {
                t = panel.Find("ResourcesCost" + i).Find("Text").GetComponent<Text>();
                t.text = UITextFormatter.CutOffNumericalPart(t.text) + UnitType.Stats.ResourcesCostToBuild[i];
            }

        }
        public UnitStatsUI(UnitType unitType)
        {
            UnitType = unitType;
        }
        public UnitStatsUI(UnitClientSide unit)
        {
            UnitType = unit._type;
        }
    }
}
