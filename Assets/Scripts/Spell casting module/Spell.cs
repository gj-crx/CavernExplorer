using System.Collections.Generic;
using UnityEngine;

namespace Spells
{
    [CreateAssetMenu(fileName = "Spell", menuName = "ScriptableObjects/Spell", order = 0)]
    public class Spell : ScriptableObject
    {
        public string SpellName = "Lightning strike";
        public float ManaCost = 10;

        public CastingMethod Method;
        public List<Effect> EffectsOnCast;
        
        



        [System.Serializable]
        public class Effect
        {
            public EffectType Type;

            public float HitPointsChange = 0;
            public float HitRadius = 0;
            public float EffectDuration = 10;
            public string SpellTargetTag;

            public Unit.UnitStats AffectedStats;
            private Projectile.ProjectileStats projectileStats;

            [SerializeField]
            private GameObject effectProjectilePrefab;
            [SerializeField]
            private GameObject lightningEffectPrefab;

            public void CastEffect(CastingTarget target, Unit casterUnit)
            {
                if (Type == EffectType.Projectile)
                { //launching projectile that applies effect on collision
                    GameObject projectile = Instantiate(effectProjectilePrefab, casterUnit.transform.position, Quaternion.identity);
                    projectile.GetComponent<Projectile>().SetProjectileValues(projectileStats, casterUnit, this);
                }
                else if (Type == EffectType.Instant)
                {
                    //---
                    if (target.MethodName == CastingMethod.TargetedOnUnitInstant)
                    {
                        if (HitRadius == 0) HitTargetedUnit(target.UnitTarget, casterUnit); //instantly casting on target
                        else
                        { //searching for other targets in radius around of targeted unit position
                            foreach (var hittedUnit in GameManager.dataBase.GetUnitsInRangeOfPoint(target.UnitTarget.transform.position, HitRadius, SpellTargetTag, casterUnit)) 
                                HitTargetedUnit(hittedUnit, casterUnit);
                        }
                    }
                    //---
                    else if (target.MethodName == CastingMethod.TargetedAtPointInstant)
                    { //searching for targets around of cast point
                        foreach (var hittedUnit in GameManager.dataBase.GetUnitsInRangeOfPoint(target.PointOfTarget, HitRadius, SpellTargetTag, casterUnit))
                            HitTargetedUnit(hittedUnit, casterUnit);
                    }
                    //---
                    else if (target.MethodName == CastingMethod.Selfcasted)
                    { //caster unit hitting himself with a spell
                        HitTargetedUnit(casterUnit, casterUnit);
                    }
                    //---
                }
                //visualizing lightning effect if it is not null
                if (lightningEffectPrefab != null)
                {
                    var lightningEffect = GameObject.Instantiate(lightningEffectPrefab, casterUnit.transform.position, Quaternion.identity).GetComponent<DigitalRuby.LightningBolt.LightningBoltScript>();
                    lightningEffect.StartObject = casterUnit.gameObject;
                    if (target.MethodName == CastingMethod.TargetedOnUnitInstant) lightningEffect.EndObject = target.UnitTarget.gameObject;
                    else if (target.MethodName == CastingMethod.TargetedAtPointInstant) lightningEffect.EndPosition = target.PointOfTarget;
                    else lightningEffect.EndObject = casterUnit.gameObject;
                }
            }

            private void HitTargetedUnit(Unit target, Unit casterUnit)
            {
                Debug.Log(target.name + " is hitted by an effect");
                target.GetDamage(HitPointsChange, casterUnit);
                target.ApplyEffect(AffectedStats, EffectDuration);
            }

            public enum EffectType : byte
            {
                Instant = 0,
                Projectile = 1
            }
        }

        public struct CastingTarget
        {
            public CastingMethod MethodName;
            public Vector3 PointOfTarget;
            public Unit UnitTarget;
            public CastingTarget(Vector3 pointOfTarget)
            { //Point target
                MethodName = CastingMethod.TargetedAtPointInstant;
                this.PointOfTarget = pointOfTarget;
                UnitTarget = null;
            }
            public CastingTarget(Unit targetUnit)
            { //Point target
                MethodName = CastingMethod.TargetedOnUnitInstant;
                this.PointOfTarget = Vector3.zero;
                UnitTarget = targetUnit;
            }
            public CastingTarget(Unit casterUnit, bool SelfCasted)
            { //Point target
                MethodName = CastingMethod.Selfcasted;
                this.PointOfTarget = Vector3.zero;
                UnitTarget = casterUnit;
            }
        }
        public enum CastingMethod : byte
        {
            Selfcasted = 0,
            TargetedOnUnitInstant = 1,
            TargetedAtPointInstant = 2,
        }


    }
}
