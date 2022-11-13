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
                while (unitsToControl.Count > 0)
                {
                    Unit currentUnit = unitsToControl.Dequeue();
                    if (currentUnit.behavior != null) currentUnit.behavior.BehaviorInteraction();
                    Thread.Sleep(100);
                }
                Thread.Sleep(100);
            }
            UnitControllingThread = null;
        }
        private void GetUnitsQueue()
        {
            unitsToControl = new Queue<Unit>();
            foreach (var unit in GameManager.dataBase.AllUnits)
            {
                if (unit.behavior != null) unitsToControl.Enqueue(unit);
            }
        }
    }
}
