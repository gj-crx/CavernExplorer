using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Types
{
    [RequireComponent(typeof(UnitTypePrefabVariant))]
    public class BuildingTypePrefabVariant : MonoBehaviour
    {

        public sbyte ProducedResourceID = 0;
        public ResourceType ProducedResource;
        //modifiers
        public float ProducedResourcesAmount = 10;
        public float UnitTrainingSpeedModifier = 1.0f;


        public BuildingType ToBasicType()
        {
            BuildingType b = new BuildingType(this, GetComponent<UnitTypePrefabVariant>());
            return b;
        }
        private void LateUpdate()
        {
            Destroy(this);
        }
    }
}
