using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;

namespace ClientSideLogic.UI
{
    public class BuildingHelper
    {
        public bool CastingBuildingShadow = false;
        public GameObject BuildingShadow = null;
        public byte SelectedUnitTypeID = 0;

        public void CreateBuildingShadow(byte UnitTypeID)
        {
            BuildingShadow = GameObject.Instantiate(PrefabManager.Singleton.prefabs_AllUnits[UnitTypeID]);
            SelectedUnitTypeID = UnitTypeID;
            CastingBuildingShadow = true;
        }
        public void ControlBuildingHelper()
        {
            if (CastingBuildingShadow && BuildingShadow != null)
            {
                BuildingShadow.transform.position = UIPlayerActions.GetPointOfClick();
                if (BuildingShadow.transform.position == Vector3.zero)
                {
                    return;
                }
                BuildingShadow.transform.position = new Vector3((int)BuildingShadow.transform.position.x, BuildingShadow.transform.lossyScale.y / 2, (int)BuildingShadow.transform.position.z);
                if (UIPlayerActions.UserClicked())
                {
                    OrderToBuild();
                }
            }
        }
        private void OrderToBuild()
        {
            UIPlayerActions.LocalPlayer.CreateBuildingOrderServerRpc(SelectedUnitTypeID, BuildingShadow.transform.position);
            GameObject.Destroy(BuildingShadow);
            CastingBuildingShadow = false;
        }
    }
}
