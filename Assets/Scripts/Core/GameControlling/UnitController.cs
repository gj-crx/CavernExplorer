using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

namespace Controllers
{
    public class UnitController
    {
        private Thread UnitControllingThread;
        private Queue<Unit> unitsToControl;
        private int controllingDelay = 100;

        public UnitController()
        {
            UnitControllingThread = new Thread(UnitsControllingProcess);
            UnitControllingThread.Start();
        }

        private void UnitsControllingProcess()
        {
            while (GameManager.GameIsRunning)
            {
                GetUnitsQueue();
                controllingDelay = 1750 / Mathf.Max(1, unitsToControl.Count);
                while (unitsToControl.Count > 0)
                {
                    Unit currentUnit = unitsToControl.Dequeue();
                    if (currentUnit.behavior != null) currentUnit.behavior.BehaviorInteraction();
                    Thread.Sleep(controllingDelay);
                }
                Thread.Sleep(controllingDelay);
            }
            UnitControllingThread = null;
        }
        private void GetUnitsQueue()
        {
            unitsToControl = new Queue<Unit>();
            foreach (var unit in GameManager.dataBase.AllUnits)
            {
                if (unit != null && unit.behavior != null) unitsToControl.Enqueue(unit);
            }
        }
    }
}
