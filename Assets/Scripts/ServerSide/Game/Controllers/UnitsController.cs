using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerSideLogic
{
    public class UnitsController
    {
        public Database db;
        public List<int> RecentlyRemovedUnitIDs = new List<int>();


        public float BuildingProductionTimeScale = 1;
        public int CurrentSyncPart = 0;

        public int NormalUnitActionsControllingDelay = 2500;
        public int NormalUnitMovementDelay = 250;


        public UnitsController(Database dataBase)
        {
            db = dataBase;
        }

        public int GetRegularUnitsCount()
        {
            return db.RegularUnits.Count;
        }

        public void AddNewUnit(Unit unit, Unit.UnitClass UnitClass = 0)
        {
            unit.ID = db.AddToAllUnits(unit);
            if (UnitClass == Unit.UnitClass.RegularUnit)
            {
                db.RegularUnits.AddLast(unit);
            }
            if (UnitClass == Unit.UnitClass.Tree)
            {
                db.Trees.Add(unit);
            }
        }
        public void AddNewBuildingOnly(Building building)
        {
            if (building != null)
            {
                db.Buildings.Add(building);
                building.IDInBuildingsPool = db.Buildings.IndexOf(building);
            }
            else
            {
                Debug.LogError("Building type not assigned");
            }
        }


        public void DeleteUnit(Unit unit)
        {
            db.AllUnits[unit.ID] = null;
            if (unit.Type.Class == Unit.UnitClass.RegularUnit)
            {
                db.RegularUnits.Remove(unit);
            }
            if (unit.Type.Class == Unit.UnitClass.Tree)
            {
                db.Trees.Remove(unit);
            }
           // Debug.Log("Unit deleted " + unit.ID);
        }
        public void DeleteUnit(Building unit)
        {
            db.Buildings.Remove(unit);
            db.AllUnits[unit.ID] = null;
        }
    }
}
