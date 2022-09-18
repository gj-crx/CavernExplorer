using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

namespace ServerSideLogic
{
    public class StatesController
    {
        public int NormalDelayToControlState = 7000;


        private Database _database;

        public StatesController(Database database)
        {
            _database = database;
        }

        public int GetStatesCount()
        {
            int count = 0;
            for (int i = 0; i < _database.States.Length; i++)
            {
                if (_database.States[i] != null) count++;
            }
            return count;
        }
        public byte AddState(State state)
        {
            for (byte i = 0; i < _database.States.Length; i++)
            {
                if (_database.States[i] == null)
                {
                    _database.States[i] = state;
                    return i;
                }
            }
            Debug.Log("StatesController overflown");
            return 0;
        }
    }
}
