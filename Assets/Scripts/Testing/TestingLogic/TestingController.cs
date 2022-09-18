using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;


namespace ServerSideLogic.Testing
{
    public static class TestingController
    {
        public static bool ShowAllResults = true;
        private static object AwaitedResult = null;
        private static object InitialResult = null;
        private static object FinalResult = null;

        private static Unit TestingUnit = null;


        public static void TestUnitsMovement(Vector3 Awaited, Unit TestingObject)
        {
            TestingUnit = TestingObject;
            AwaitedResult = Awaited;
            AwaitForTestResults();
        }
        private static void AwaitForTestResults()
        {
            Thread WaitingThread = new Thread(UnitPositionsChecking);
            WaitingThread.Start();
        }

        private static void UnitPositionsChecking()
        {
            InitialResult = TestingUnit.position;
            Thread.Sleep(5000);
            FinalResult = TestingUnit.position;

            if (Vector3.Distance((Vector3)FinalResult, (Vector3)AwaitedResult) < 0.6f)
            {
                if (ShowAllResults) Debug.Log("~~ TESTING: Unit positions test = ok");
            }
            else
            {
                Debug.LogError("Unit positions test failed initial result was " + InitialResult + " real result was " + FinalResult + " and it should be " + AwaitedResult);
            }
        }
        public enum Tests : int
        {
            UnitMovingTest,
            UnitPathfindingTest,
            AllGameTest
        }
    }
}
