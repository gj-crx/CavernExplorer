using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spells {
    [System.Serializable]
    public class Spell
    {
        public string SpellName = "Lightning strike";
        public List<SpellCastingSystem.SpellEffect> EffectsOnCast;
    }
}
