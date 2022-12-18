using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Contains all dynamic information of the game
/// </summary>
public class DataBase 
{
    public List<Unit> AllUnits = new List<Unit>();
    public Stack<UI.InventoryLogic.Corpse> Corpses = new Stack<UI.InventoryLogic.Corpse>();



    public List<Unit> GetUnitsInRangeOfPoint(Vector3 referencePoint, float range)
    {
        List<Unit> unitsInRange = new List<Unit>();
        foreach (Unit checkedUnit in AllUnits)
        {
            if (Vector3.Distance(referencePoint, checkedUnit.transform.position) < range)
            {
                unitsInRange.Add(checkedUnit);
            }
        }
        return unitsInRange;
    }
    public List<Unit> GetUnitsInRangeOfPoint(Vector3 referencePoint, float range, string searchTag, Unit originUnit)
    {
        List<Unit> unitsInRange = new List<Unit>();
        foreach (Unit checkedUnit in AllUnits)
        {
            if (checkedUnit.tag == searchTag && checkedUnit != originUnit && Vector3.Distance(referencePoint, checkedUnit.transform.position) < range)
            {
                unitsInRange.Add(checkedUnit);
            }
        }
        return unitsInRange;
    }
}
