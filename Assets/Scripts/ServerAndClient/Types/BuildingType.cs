using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Types
{
    public class BuildingType : UnitType
    {
        public short BuildingTypeID = 0;
        public ResourceType ProducedResource;
        //modifiers
        public float ProducedResourcesAmount = 10;
        public float UnitTrainingSpeedModifier = 1.0f;
        

        public BuildingType(BuildingTypePrefabVariant BuildingVariant, UnitTypePrefabVariant UnitVariant) : base (UnitVariant)
        {
            if (BuildingVariant.ProducedResourceID > 0)
            {
                ProducedResource = TypesData.ResourceTypes[BuildingVariant.ProducedResourceID];
            }
            ProducedResourcesAmount = BuildingVariant.ProducedResourcesAmount;
            UnitTrainingSpeedModifier = BuildingVariant.UnitTrainingSpeedModifier;

    }

    }
}
