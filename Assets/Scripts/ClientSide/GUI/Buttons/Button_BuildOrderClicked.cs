using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;


namespace ClientSideLogic.UI.Buttons
{
    public class Button_BuildOrderClicked : MonoBehaviour
    {
        public byte ID = 0;
        void Start()
        {

        }
        public void BuildOrderButtonClicked()
        {
            //Client side check if there is enough resources to build a unit
            byte InsufficientResourceID = DetectInsufficientAmountOfResource();
            
            if (InsufficientResourceID == byte.MaxValue)
            {
                //check if it's a building or a unit
                if (TypesData.AllUnitTypes[ID].Class == ServerSideLogic.Unit.UnitClass.Building)
                {
                    UIController.Singleton.panel_BuildingAndTraining.SetActive(false);
                    UIController.Singleton.buildingHelper.CreateBuildingShadow(ID);
                }
                else
                {
                    var _buildingToTrain = UIPlayerActions.LocalPlayer.clientSide_ControlledState.GetTrainingBuilding();
                    if (_buildingToTrain != null)
                    {
                        UIPlayerActions.LocalPlayer.TrainUnitOrderServerRpc(_buildingToTrain.IDInBuildingsPool, ID);
                    }
                }
                ClientStateController.SubstractResourcesForUnitClientSide(ID, UIPlayerActions.LocalPlayer.clientSide_ControlledState);
            }
            else
            {
                UIController.Singleton.CreateAlert("Not enough resources to train a unit. You are currently have " + UIPlayerActions.LocalPlayer.clientSide_ControlledState.ResourcesAmount[InsufficientResourceID]
                    + " " + TypesData.ResourceTypes[InsufficientResourceID].name + " but you need atleast " + TypesData.AllUnitTypes[ID].Stats.ResourcesCostToBuild[InsufficientResourceID]);
            }
        }
        private byte DetectInsufficientAmountOfResource()
        {
            for (byte i = 0; i < TypesData.ResourceTypes.Count; i++)
            {
                if (UIPlayerActions.LocalPlayer.clientSide_ControlledState.ResourcesAmount[i] < TypesData.AllUnitTypes[ID].Stats.ResourcesCostToBuild[i])
                { //insufficient amount of resources detected
                    return i;
                }
            }
            return byte.MaxValue;
        }
    }
}
