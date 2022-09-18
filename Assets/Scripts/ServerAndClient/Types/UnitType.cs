using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerSideLogic;
using System;

namespace Types
{
    public class UnitType
    {
        public UnitStaticStats Stats;
        public Unit.UnitClass Class = Unit.UnitClass.RegularUnit;
        public byte UnitTypeID = 0;
        public byte RaceRelated = 0;
        public string UnitTypeName = "Unknown unit";
        public string UnitTag = "Generic";
        public short RequiredTechnology = 1;

        public UnitType(UnitTypePrefabVariant v)
        {
            Class = v.UnitClass;
            UnitTypeID = v.UnitTypeID;
            RaceRelated = v.RaceRelated;
            UnitTag = v.UnitTag;
            UnitTypeName = v.UnitTypeName;
            RequiredTechnology = v.RequiredTechnology;

            Stats = v.Stats;
        }
        /// <summary>
        /// return behavior by UnitTag
        /// </summary>
        [Serializable]
        public struct UnitStaticStats
        {
            public float MaxHP;
            public float Damage;
            public float Regeneration;
            public float MoveSpeed;
            public float AttackDelay;
            public float AttackRange;
            public float TrainTimeNeeded;
            public float[] ResourcesCostToBuild;
            public float ResourcesCarriedMaximum;
            public float[] ResourcesGivenOnKilled;

            public byte ObstacleRadius;

            public UnitStaticStats(bool BasicStats)
            {
                MaxHP = 100;
                Damage = 10;
                Regeneration = 1f;
                MoveSpeed = 3;
                AttackDelay = 0.5f;
                AttackRange = 2.5f;
                TrainTimeNeeded = 10;
                ResourcesCostToBuild = new float[4];
                ResourcesCarriedMaximum = 20;
                ResourcesGivenOnKilled = new float[4];
                ObstacleRadius = 1;
            }

        }
        }
    }
    
