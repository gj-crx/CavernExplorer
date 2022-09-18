using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerSideLogic.AI
{
    public class AIBuildOrder
    {
        public byte[] TypeIDsOfUnitsToBuildOrder;

        public AIBuildOrder()
        {
            TypeIDsOfUnitsToBuildOrder = BasicAIScheme.Singleton.BasicUnitBuildOrder;
        }

        public byte GetNextUnitIDToBuild(State _state)
        { //function lineary checks from the beginning of the order array if this unit was already built / trained
            //if not - he is the next order
            byte[] CurrentUnitsCount = new byte[Types.TypesData.AllUnitTypes.Length];
            for (int i = 0; i < TypeIDsOfUnitsToBuildOrder.Length; i++)
            {
                if (CurrentUnitsCount[TypeIDsOfUnitsToBuildOrder[i]] < _state.CountUnitsOfType[TypeIDsOfUnitsToBuildOrder[i]])
                { //this step in the order already completed, so moving forward
                    CurrentUnitsCount[TypeIDsOfUnitsToBuildOrder[i]]++;
                }
                else
                {
                    return TypeIDsOfUnitsToBuildOrder[i];
                }
            }
            return 0;
        }
    }
}
