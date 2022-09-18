using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;

namespace ClientSideLogic
{
    public class StateClientSide : MonoBehaviour, IState
    {
        public short StateID = 0;
        public byte RaceID = 0;
        public string StateName = "Unsyncronized state";
        public int ControllingPlayerID = 0;

        public float[] ResourcesAmount = new float[3];
        public List<BuildingClientSide> Buildings = new List<BuildingClientSide>();

        public List<short> AquiredTechnologies = new List<short>();

        public List<UnitClientSide> MainArmy = new List<UnitClientSide>();
        public BuildingClientSide Townhall;

        private void Awake()
        {
            AquiredTechnologies.Add(0);
        }
        public BuildingClientSide GetTrainingBuilding()
        {
            if (Townhall != null)
            {
                return Townhall;
            }
            else
            {
                Debug.LogError("Townhall of the state is not assigned");
                return null;
            }
        }
        public IBuilding GetRandomTrainingBuilding()
        {
            int AttemptsCount = 0;
            while (AttemptsCount < 4)
            {
                int random = Random.Range(0, Buildings.Count);
                if (Buildings[random] != null)
                {
                    return Buildings[random];
                }
                AttemptsCount++;
            }
            return null;
        }
    }
}
