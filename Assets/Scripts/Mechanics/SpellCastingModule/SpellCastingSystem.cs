using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spells
{
    public static class SpellCastingSystem
    {
        public static bool CastSpell(Spell spellToCast)
        {
            //...

            return true;
        }



        public enum SpellEffect : byte
        {
            None = 0,
            HPRestoration = 1,
            MovespeedBonus = 2,
            Slow = 3,
            Damage = 4,
            FireballCast = 5
        }
    }
}
