using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;

namespace Spells
{
    public static class SpellCastingSystem
    {
        public static SpellTargeting targetingInput;


        public static bool CastSpell(Spell spellToCast, Spell.CastingTarget target, Unit casterUnit = null)
        {
            if (CheckSpellRequirements(spellToCast, casterUnit))
            {
                casterUnit.Stats.CurrentMana -= spellToCast.ManaCost;
                if (casterUnit == null) casterUnit = GameManager.playerControls.PlayerCharacterUnit;
                foreach (Spell.Effect effect in spellToCast.EffectsOnCast) effect.CastEffect(target, casterUnit);

                return true;
            }
            else
            {
                UIScenario.Singleton.ShowMinorError("Not enough mana to cast");
                return false;
            }
        }

        public static void PrepareSpellToCast(Spell spellToCast, Spell.CastingTarget target, Unit casterUnit = null)
        {
            if (casterUnit == null) casterUnit = GameManager.playerControls.PlayerCharacterUnit;
            targetingInput.gameObject.SetActive(true);
            targetingInput.PreparedSpell = spellToCast;
            targetingInput.SpellCaster = casterUnit;
        }

        private static bool CheckSpellRequirements(Spell spellToCheck, Unit casterUnit)
        {
            return spellToCheck.ManaCost <= casterUnit.Stats.CurrentMana;
        }



    }
}
