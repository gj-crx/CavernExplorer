using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;

namespace ClientSideLogic
{

    public class BuildingClientSide : MonoBehaviour, IBuilding
    {
        public int IDInBuildingsPool = 0;
        public UnitClientSide unit;

        public Queue<int> UnitTrainingQueue = new Queue<int>();


        private BuildingType _type;
        private bool IsAssignedToState = false;

        private void Start()
        {
            unit = GetComponent<UnitClientSide>();
            _type = TypesData.BuildingTypes[unit._type.UnitTypeID];
        }
        private void Update()
        {
            if (IsAssignedToState == false && ClientGameController.Singleton.dataBase.States[unit.StateID] != null)
            {
                if (_type.UnitTag == "Townhall")
                {
                    ClientGameController.Singleton.dataBase.States[unit.StateID].Townhall = this;
                }
                IsAssignedToState = true;
            }
        }

    }
}
