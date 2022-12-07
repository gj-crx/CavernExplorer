using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spells
{
    public static class SpellCastingSystem
    {
        public static bool CastSpell(Spell spellToCast, Spell.CastingTarget target, Unit casterUnit = null)
        {
            if (casterUnit == null) casterUnit = GameManager.playerControls.PlayerCharacterUnit;
            foreach (Spell.Effect effect in spellToCast.EffectsOnCast) effect.CastEffect(target, casterUnit);

            return true;
        }



    }
}
