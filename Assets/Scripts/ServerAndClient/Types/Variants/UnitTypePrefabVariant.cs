using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Types
{
    public class UnitTypePrefabVariant : MonoBehaviour
    {
        public ServerSideLogic.Unit.UnitClass UnitClass = ServerSideLogic.Unit.UnitClass.RegularUnit;
        [HideInInspector]
        public byte UnitTypeID = 0;
        public byte RaceRelated = 0;
        public string UnitTag = "Generic";
        public string UnitTypeName = "Unknown unit";
        public short RequiredTechnology = 1;

        public UnitType.UnitStaticStats Stats;
        public UnitType ToBasicType()
        {
            UnitType b = new UnitType(this);

            return b;
        }
        private void LateUpdate()
        {
            Destroy(this);
        }
    }
}
