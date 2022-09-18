using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace ServerSideLogic
{
    namespace InternalMechanics
    {
        public class TController
        {
            public int NormalDelayMS = 2000;
            private int _calculatedDelay = 2000;
            public int CalculatedDelay { get { return _calculatedDelay; } private set { _calculatedDelay = value; } }
            private int RecalculationDelay = 5000;

            public delegate void ControllingFunction();
            public delegate int ObjectsToControlCount();
            private ControllingFunction _controllingFunction;
            private ObjectsToControlCount GetObjectsToControlCount;

            private float timer_DelayRecalculation = 0;

            public TController(int NormalDelayInMS, ControllingFunction controllingFunction, int RecalculationTimeInMS, ObjectsToControlCount GetObjectsToControlCount)
            {
                NormalDelayMS = NormalDelayInMS;
                _controllingFunction = controllingFunction;
                RecalculationDelay = RecalculationTimeInMS;
                this.GetObjectsToControlCount = GetObjectsToControlCount;
            }

            public void Control()
            {
                _controllingFunction();
                RecalculateDelay();
                Thread.Sleep(CalculatedDelay);
            }
            private void RecalculateDelay()
            {
                if (timer_DelayRecalculation > RecalculationDelay)
                {
                    timer_DelayRecalculation = 0;
                    CalculatedDelay = NormalDelayMS / GetObjectsToControlCount();
                }
                else timer_DelayRecalculation += CalculatedDelay;
            }
        }


        //
    }
}

