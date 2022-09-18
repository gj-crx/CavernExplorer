using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerSideLogic;
using ClientSideLogic;

/// <summary>
/// Server side only
/// </summary>
namespace ServerSideLogic
{
    public class Database
    {
        public Unit[] AllUnits = new Unit[100];
        public State[] States = new State[byte.MaxValue];


        public LinkedList<Unit> RegularUnits = new LinkedList<Unit>();
        public List<Building> Buildings = new List<Building>();
        public List<Unit> Trees = new List<Unit>();


        public int AddToAllUnits(Unit unit)
        {
            for (int i = 0; i < AllUnits.Length; i++)
            {
                if (AllUnits[i] == null)
                {
                    AllUnits[i] = unit;
                    return i;
                }
            }
            //if empty space in array was not found
            Unit[] NewAllUnits = new Unit[AllUnits.Length * 2];
            for (int i = 0; i < AllUnits.Length; i++)
            {
                NewAllUnits[i] = AllUnits[i];
            }
            int index = AllUnits.Length;
            NewAllUnits[index] = unit;
            AllUnits = NewAllUnits;
            return index;
        }
    }
}
namespace ClientSideLogic
{
    public class DatabaseClientVersion
    {
        public UnitClientSide[] AllUnits = new UnitClientSide[100];
        public StateClientSide[] States = new StateClientSide[byte.MaxValue];

        public List<UnitClientSide> RegularUnits = new List<UnitClientSide>();


        public void AddToAllUnits(UnitClientSide unit)
        {
            if (unit.ID < AllUnits.Length)
            {
                AllUnits[unit.ID] = unit;
            }
            else
            {
                UnitClientSide[] NewAllUnits = new UnitClientSide[AllUnits.Length * 2];
                for (int i = 0; i < AllUnits.Length; i++)
                {
                    NewAllUnits[i] = AllUnits[i];
                }
                NewAllUnits[unit.ID] = unit;
                AllUnits = NewAllUnits;
            }
            //add to special category
            if (unit._type.Class == Unit.UnitClass.RegularUnit)
            {
                RegularUnits.Add(unit);
            }
        }
        public void RemoveUnitFromDB(int UnitID)
        {
            AllUnits[UnitID] = null;
        }
    }
}
