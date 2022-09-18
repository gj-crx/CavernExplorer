using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;


namespace ServerSideLogic.AI
{
    public class BasicAIScheme : MonoBehaviour
    {
        public static BasicAIScheme Singleton;
        public byte[] BasicUnitBuildOrder
        {
            get { return TypeIDsOfUnitsToBuildOrder; }
        }
        [SerializeField]
        private string[] BuildOrderByTags;

        private byte[] TypeIDsOfUnitsToBuildOrder;

        private void Awake()
        {
            ConvertTagsToUnitTypeIDs();
            Singleton = this;
        }
        private void ConvertTagsToUnitTypeIDs()
        {
            TypeIDsOfUnitsToBuildOrder = new byte[BuildOrderByTags.Length];
            for (int i = 0; i < BuildOrderByTags.Length; i++)
            {
                var TypeOfCurrentUnit = TypesData.GetUnitTypeByTag(BuildOrderByTags[i]);
                if (TypeOfCurrentUnit != null)
                {
                    TypeIDsOfUnitsToBuildOrder[i] = TypeOfCurrentUnit.UnitTypeID;
                }
            }
        }
    }
}
