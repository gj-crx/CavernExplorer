using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;
using ServerSideLogic.Behaviors;

namespace ServerSideLogic
{
    /// <summary>
    /// part of an Unit class
    /// </summary>
    public class Building : Unit, IBuilding
    {
        /// <summary>
        /// refers to Buildings in UnitsController
        /// </summary>
        public int IDInBuildingsPool = 0;
        private int TimerMS_ResourceProduction;
        private int TimerMS_UnitTraining;

        public BuildingType type;
        /// <summary>
        /// contains UnitTypeIDs
        /// </summary>
        private Queue<byte> UnitTrainingQueue = new Queue<byte>();

        private Vector3 UnitSpawningOffset = new Vector3(3.4f, 0, 2.5f);

        public Building(BuildingType type, Game GameToBe, Vector3 position, State state, byte UnitTypeID) : base(GameToBe, position, state, UnitTypeID)
        {
            this.type = type;
            GameToBe.unitsController.AddNewBuildingOnly(this);
            GetBehavior();
        }
        public void EnqueueUnitTraining(byte EnqueuedUnitTypeID)
        {
            if (UnitLogic.IsPossibleToBuildAUnit(EnqueuedUnitTypeID, state))
            {
                UnitLogic.SubstractResourcesForUnitCost(EnqueuedUnitTypeID, state);
                UnitTrainingQueue.Enqueue(EnqueuedUnitTypeID);
                state.TrainingQueuedUnitsTypeIDs.Add(EnqueuedUnitTypeID);
            }
        }

        private void GetBehavior()
        {
            if (state != null)
            {
                Debug.Log(type.UnitTypeName + " got building behavior");
                behavior = new BuildingBehaviour(this);
                if (behavior != null) behavior.StartIterations(game.unitsController.NormalUnitActionsControllingDelay, Random.Range(0, 1000));
            }
        }

        public void ControlBuildingProduction()
        {
            if (type.ProducedResource != null)
            {
                TimerMS_ResourceProduction += game.unitsController.NormalUnitActionsControllingDelay;
                if (TimerMS_ResourceProduction > type.ProducedResource.ProductionTimeNeeded)
                {
                    TimerMS_ResourceProduction = 0;
                    ProduceResources();
                }
            }
            if (UnitTrainingQueue.Count > 0)
            {
                Debug.Log("unit training");
                TimerMS_UnitTraining += game.unitsController.NormalUnitActionsControllingDelay;
                if (TimerMS_UnitTraining > TypesData.AllUnitTypes[UnitTrainingQueue.Peek()].Stats.TrainTimeNeeded * type.UnitTrainingSpeedModifier)
                {
                    TimerMS_ResourceProduction = 0;
                    TrainUnit();
                }
            }
        }

        private void ProduceResources()
        {
            if (TimerMS_ResourceProduction >= type.ProducedResource.ProductionTimeNeeded)
            {
                bool EnoughResources = true;
                for (int i = 0; i < type.ProducedResource.ResourcesCostPer1.Length; i++)
                {
                    if (state.ResourcesAmount[i] < type.Stats.ResourcesCostToBuild[i])
                    {
                        EnoughResources = false;
                        break;
                    }
                }
                if (EnoughResources)
                {
                    for (int i = 0; i < type.ProducedResource.ResourcesCostPer1.Length; i++)
                    {
                        state.ResourcesAmount[i] -= type.ProducedResource.ResourcesCostPer1[i];
                    }
                    state.ResourcesAmount[type.ProducedResource.ID] += type.ProducedResourcesAmount;
                }
            }
            else
            {
                TimerMS_ResourceProduction = 0;
            }
        }
        private void TrainUnit()
        {
            Unit NewUnit = new Unit(game, position + UnitSpawningOffset, state, UnitTrainingQueue.Dequeue());
            state.TrainingQueuedUnitsTypeIDs.Remove(NewUnit.Type.UnitTypeID);
            Debug.Log("Trained unit ID " + NewUnit.ID + " typeID " + NewUnit.Type.UnitTypeID);
        }
    }
}
