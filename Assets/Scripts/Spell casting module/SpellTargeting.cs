using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spells
{
    public class SpellTargeting : MonoBehaviour
    {
        public Spell PreparedSpell;
        public Unit SpellCaster;

        private bool initialPositionSet = false;

        private void Awake()
        {
            SpellCastingSystem.targetingInput = this;
            gameObject.SetActive(false);
        }
        private void Update()
        {
            if (Application.isMobilePlatform)
            { //1: users taps on spell icon 2: user taps on the screen to set initial position and it being dragged 3: user stops aiming spell at it being casted
                if (initialPositionSet == false)
                { //waiting for initial touch
                    if (Input.touchCount > 0)
                    {
                        transform.position = Input.touches[0].position;
                        initialPositionSet = true;
                    }
                }
                else
                {
                    if (Input.touchCount == 0)
                    { //touch released and spell is being casted
                        TryToCast(Camera.main.ScreenToWorldPoint(new Vector3(transform.position.x, transform.position.y, 0)));
                    }
                    else transform.position = Input.touches[0].position; //dragging target
                }
            }
            else
            {
                transform.position = Input.mousePosition;
                if (Input.GetMouseButtonUp(0))
                {
                    TryToCast(Camera.main.ScreenToWorldPoint(new Vector3(transform.position.x, transform.position.y, 0)));
                }
            }
        }

        private void TryToCast(Vector3 position)
        {
            position = new Vector3(position.x, position.y, 0);
            if (PreparedSpell.Method == Spell.CastingMethod.TargetedAtPointInstant)
            {
                SpellCastingSystem.CastSpell(PreparedSpell, new Spell.CastingTarget(position), SpellCaster);
            }
            else if (PreparedSpell.Method == Spell.CastingMethod.TargetedOnUnitInstant)
            {
                List<Unit> possibleTargets = GameManager.dataBase.GetUnitsInRangeOfPoint(position, 2.5f);
                if (possibleTargets.Count > 0) SpellCastingSystem.CastSpell(PreparedSpell, new Spell.CastingTarget(possibleTargets[0]), SpellCaster);
            }
            gameObject.SetActive(false);
        }
        private void OnEnable()
        {
            initialPositionSet = false;
        }
    }
}
