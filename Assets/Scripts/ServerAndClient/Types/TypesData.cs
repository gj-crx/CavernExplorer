using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerSideLogic;

namespace Types
{
    public static class TypesData
    {
        public static List<ResourceType> ResourceTypes = new List<ResourceType>();
        public static List<TechnologyType> TechTypes = new List<TechnologyType>();
        /// <summary>
        /// units, buildings and trees
        /// </summary>
        public static UnitType[] AllUnitTypes = new UnitType[150];
        public static BuildingType[] BuildingTypes = new BuildingType[150];

        public static List<UnitType> TreeTypes = new List<UnitType>();


        public static void LoadAllTypes(GameObject[] UnitPrefabs, GameObject[] BuildingPrefabs, GameObject[] TreesPrefabs)
        {
            LoadResourceTypes();
            LoadUnitTypes(UnitPrefabs, BuildingPrefabs, TreesPrefabs);
        }
        private static void LoadUnitTypes(GameObject[] UnitPrefabs, GameObject[] BuildingPrefabs, GameObject[] TreesPrefabs)
        {
            foreach (var g in UnitPrefabs)
            {
                UnitType unitType = g.GetComponent<UnitTypePrefabVariant>().ToBasicType();
                byte NewUnitTypeID = (byte)AddToUnitTypes(unitType);
                PrefabManager.Singleton.prefabs_AllUnits[NewUnitTypeID] = g;

                Debug.Log(NewUnitTypeID + " = " + unitType.UnitTypeName);
            }
            foreach (var g in BuildingPrefabs)
            {
                UnitType unitType = g.GetComponent<UnitTypePrefabVariant>().ToBasicType();
                byte NewUnitTypeID = (byte)AddToUnitTypes(unitType);
                PrefabManager.Singleton.prefabs_AllUnits[NewUnitTypeID] = g;

                BuildingType BuildingType = g.GetComponent<BuildingTypePrefabVariant>().ToBasicType();
                BuildingType.UnitTypeID = NewUnitTypeID;
                BuildingType.BuildingTypeID = AddToBuildingTypes(BuildingType);

                Debug.Log(NewUnitTypeID + " = " + unitType.UnitTypeName);
            }
            foreach (var g in TreesPrefabs)
            {
                UnitType unitType = g.GetComponent<UnitTypePrefabVariant>().ToBasicType();
                byte NewUnitTypeID = (byte)AddToUnitTypes(unitType);

                TreeTypes.Add(unitType);
                PrefabManager.Singleton.prefabs_AllUnits[NewUnitTypeID] = g;

                Debug.Log(NewUnitTypeID + " = " + unitType.UnitTypeName);
            }
        }
        private static void LoadResourceTypes()
        { //resource types are hardcoded for now

            //0 - lumber
            ResourceType type = new ResourceType();
            type.ID = 0;
            type.name = "Lumber";
            type.ProductionTimeNeeded = 8;
            type.ResourcesCostPer1 = new float[4];
            ResourceTypes.Add(type);

            //1 - materials
            type = new ResourceType();
            type.ID = 1;
            type.name = "Materials";
            type.ProductionTimeNeeded = 14;
            type.ResourcesCostPer1 = new float[4];
            type.ResourcesCostPer1[0] = 1;
            ResourceTypes.Add(type);

            //2
            type = new ResourceType();
            type.ID = 2;
            type.name = "Housing";
            type.ProductionTimeNeeded = 15;
            type.ResourcesCostPer1 = new float[4];
            ResourceTypes.Add(type);
        }
        private static int AddToUnitTypes(UnitType type)
        {
            for (int i = 0; i < AllUnitTypes.Length; i++)
            {
                if (AllUnitTypes[i] == null)
                {
                    type.UnitTypeID = (byte)i;
                    AllUnitTypes[i] = type;
                    return i;
                }
            }
            return -1;
            //add array enlargement and NOT use lists
        }
        private static short AddToBuildingTypes(BuildingType type)
        {
            BuildingTypes[type.UnitTypeID] = type;
            return type.UnitTypeID;
            //add array enlargement and NOT use lists
        }
        public static UnitType GetUnitTypeByTag(string Tag, byte RaceID)
        {
            foreach (var _type in AllUnitTypes)
            {
                if (_type.UnitTag == Tag && _type.RaceRelated == RaceID)
                {
                    return _type;
                }
            }
            return null;
        }
        public static UnitType GetUnitTypeByTag(string Tag)
        {
            foreach (var _type in AllUnitTypes)
            {
                if (_type != null && _type.UnitTag == Tag)
                {
                    return _type;
                }

            }
            return null;
        }
        public static BuildingType GetBuildingTypeByTag(string Tag, byte RaceID)
        {
            foreach (var _type in BuildingTypes)
            {
                if (_type != null && _type.UnitTag == Tag && _type.RaceRelated == RaceID)
                {
                    return _type;
                }
            }
            return null;
        }
        public static BuildingType GetBuildingTypeByTag(string Tag)
        {
            foreach (var _type in BuildingTypes)
            {
                if (_type != null && _type.UnitTag == Tag)
                {
                    return _type;
                }
            }
            return null;
        }
    }
}
