using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Items;

namespace UI.InventoryLogic
{
    public class ItemStatsIndicator : MonoBehaviour
    {
        [SerializeField]
        private Image itemIcon;
        [SerializeField]
        private Text indicator_ItemName;
        [SerializeField]
        private Text indicator_ItemClass;

        [SerializeField]
        private Text indicator_GoldCost;
        [SerializeField]
        private Text indicator_SilverCost;

        [SerializeField]
        private Text indicator_Damage;
        [SerializeField]
        private Text indicator_HP;
        [SerializeField]
        private Text indicator_Regeneration;
        [SerializeField]
        private Text indicator_MoveSpeed;
        [SerializeField]
        private Text indicator_VisionDistance;
        [SerializeField]
        private Text indicator_AttackRange;


        public void FormStatsPanel(Item referenceItem)
        {
            itemIcon.sprite = referenceItem.Icon;
            indicator_ItemName.text = referenceItem.ItemName;
            indicator_ItemClass.text = referenceItem.UsedSlot.ToString();

            indicator_GoldCost.text = UITextFormatter.CutOffNumericalPart(indicator_GoldCost.text) + (referenceItem.Cost / 100).ToString();
            indicator_SilverCost.text = UITextFormatter.CutOffNumericalPart(indicator_SilverCost.text) + referenceItem.Cost.ToString();

            indicator_Damage.transform.parent.gameObject.SetActive(referenceItem.ItemStats.Damage != 0);
            indicator_HP.transform.parent.gameObject.SetActive(referenceItem.ItemStats.MaxHP != 0);
            indicator_Regeneration.transform.parent.gameObject.SetActive(referenceItem.ItemStats.Regeneration != 0);
            indicator_MoveSpeed.transform.parent.gameObject.SetActive(referenceItem.ItemStats.MoveSpeed != 0);
            indicator_VisionDistance.transform.parent.gameObject.SetActive(referenceItem.ItemStats.VisionRadius != 0);
            indicator_AttackRange.transform.parent.gameObject.SetActive(referenceItem.ItemStats.AttackRange != 0);

            if (indicator_Damage.gameObject.activeInHierarchy) indicator_Damage.text = UITextFormatter.ChangeNumericalPart(indicator_Damage.text, referenceItem.ItemStats.Damage);
            if (indicator_HP.gameObject.activeInHierarchy) indicator_HP.text = UITextFormatter.ChangeNumericalPart(indicator_HP.text, referenceItem.ItemStats.MaxHP);
            if (indicator_Regeneration.gameObject.activeInHierarchy) indicator_Regeneration.text = UITextFormatter.ChangeNumericalPart(indicator_Regeneration.text, referenceItem.ItemStats.Regeneration);
            if (indicator_MoveSpeed.gameObject.activeInHierarchy) indicator_MoveSpeed.text = UITextFormatter.ChangeNumericalPart(indicator_MoveSpeed.text, referenceItem.ItemStats.MoveSpeed);
            if (indicator_VisionDistance.gameObject.activeInHierarchy) indicator_VisionDistance.text = UITextFormatter.ChangeNumericalPart(indicator_VisionDistance.text, referenceItem.ItemStats.VisionRadius);
            if (indicator_AttackRange.gameObject.activeInHierarchy) indicator_AttackRange.text = UITextFormatter.ChangeNumericalPart(indicator_AttackRange.text, referenceItem.ItemStats.AttackRange);
        }
    }
}
