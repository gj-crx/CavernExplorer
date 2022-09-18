using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;
using ServerSideLogic;

namespace ClientSideLogic
{
    public static class ClientUnitController
    {
        public static bool UnitExistInClient(int UnitObjectID)
        {
            try
            {
                return ClientGameController.Singleton.dataBase.AllUnits[UnitObjectID] != null;
            }
            catch
            {
                return false;
            }
        }

        public static GameObject GetUnitPrefab(int UnitTypeID)
        {
            return PrefabManager.Singleton.prefabs_AllUnits[UnitTypeID];
        }

        public static UnitClientSide CreateUnitRepresentation(Unit.UnitSerializableData data)
        {
            UnitClientSide NewUnit = GameObject.Instantiate(GetUnitPrefab(data.UnitTypeID), data.position, Quaternion.identity).GetComponent<UnitClientSide>();
            NewUnit.ID = data.UnitObjectID;
            NewUnit._type = TypesData.AllUnitTypes[data.UnitTypeID];
            NewUnit.ApplyRecievedData(data);
            ClientGameController.Singleton.dataBase.AddToAllUnits(NewUnit);
            //Add building to it's state
            if (TypesData.AllUnitTypes[data.UnitTypeID].Class == Unit.UnitClass.Building && data.StateID != -1 && ClientGameController.Singleton.dataBase.States[data.StateID] != null)
            {
                ClientGameController.Singleton.dataBase.States[data.StateID].Buildings.Add(NewUnit.GetComponent<BuildingClientSide>());
            }

            //adding to the state
            NewUnit.StateID = data.StateID;
            if (data.StateID > -1)
            {
                if (NewUnit._type.Class != Unit.UnitClass.Building && ClientGameController.Singleton.dataBase.States[data.StateID])
                {
                    ClientGameController.Singleton.dataBase.States[NewUnit.StateID].MainArmy.Add(NewUnit);
                }
            }

            //adding to in-client category
            if (NewUnit._type.Class == Unit.UnitClass.Tree)
            {
                NewUnit.transform.SetParent(ClientGameController.Singleton.category_Trees.transform);
            }
            else
            {
                NewUnit.transform.SetParent(ClientGameController.Singleton.category_NormalUnits.transform);
            }

            return NewUnit;
        }
    }
}
