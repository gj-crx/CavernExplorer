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
            HPRestoration = 0,
            MovespeedBonus = 1,
            Slow = 2,
            Damage = 3,
            FireballCast = 4
        }
    }
}
