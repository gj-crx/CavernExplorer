using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Items;

namespace UI.InventoryLogic
{
    public class ItemStatsIndicator : MonoBehaviour
    {
        [HideInInspector]
        public static ItemStatsIndicator Singleton;
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

        private void Awake()
        {
            Singleton = this;
        }

        public void FormStatsPanel(Item referenceItem)
        {
            itemIcon.sprite = referenceItem.Icon;
            indicator_ItemName.text = referenceItem.ItemName;
            indicator_ItemClass.text = referenceItem.UsedSlot.ToString();

            indicator_Damage.text = UITextFormatter.ChangeNumericalPart(indicator_Damage.text, referenceItem.ItemStats.Damage);
            indicator_HP.text = UITextFormatter.ChangeNumericalPart(indicator_HP.text, referenceItem.ItemStats.MaxHP);
            indicator_Regeneration.text = UITextFormatter.ChangeNumericalPart(indicator_Regeneration.text, referenceItem.ItemStats.Regeneration);
            indicator_MoveSpeed.text = UITextFormatter.ChangeNumericalPart(indicator_MoveSpeed.text, referenceItem.ItemStats.MoveSpeed);
            indicator_VisionDistance.text = UITextFormatter.ChangeNumericalPart(indicator_VisionDistance.text, referenceItem.ItemStats.VisionRadius);
            indicator_AttackRange.text = UITextFormatter.ChangeNumericalPart(indicator_AttackRange.text, referenceItem.ItemStats.AttackRange);
        }
    }
}
