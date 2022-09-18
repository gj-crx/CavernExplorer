using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;
using System.Threading.Tasks;

namespace ClientSideLogic 
{
    public static class ClientStateController
    {




        public static void CreateClientStateRepresentation(ServerSideLogic.State.StateSerializableData data)
        {
            StateClientSide NewState = GameObject.Instantiate(PrefabManager.Singleton.prefab_State).GetComponent<StateClientSide>();
            data.Apply(NewState);
            ClientGameController.Singleton.dataBase.States[data.StateID] = NewState;
        }


        /// <summary>
        /// Alloves immediate responce for user input, but this numbers will be verified by the server later
        /// </summary>
        public static void SubstractResourcesForUnitClientSide(byte UnitTypeID, StateClientSide StateToSubstract)
        {
            for (byte i = 0; i < TypesData.ResourceTypes.Count; i++)
            {
                StateToSubstract.ResourcesAmount[i] -= TypesData.AllUnitTypes[UnitTypeID].Stats.ResourcesCostToBuild[i];
            }
        }

        public static async Task StartReCheckingStates()
        {
            while (ClientGameController.Singleton.gameObject.activeInHierarchy)
            {
                ReCheckStatesArmies();
                await Task.Delay(7000);
            }
        }
        private static void ReCheckStatesArmies()
        {
            foreach (var Unit in ClientGameController.Singleton.dataBase.RegularUnits)
            {
                if (Unit.StateID != -1 && ClientGameController.Singleton.dataBase.States[Unit.StateID] != null && ClientGameController.Singleton.dataBase.States[Unit.StateID].MainArmy.Contains(Unit) == false)
                {
                    ClientGameController.Singleton.dataBase.States[Unit.StateID].MainArmy.Add(Unit);
                }
            }

        }





    }
}
