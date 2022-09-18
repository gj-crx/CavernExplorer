using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerSideLogic
{
    public interface IScenario
    {
        void StartScenario(Matchmaking.GameLobby lobby);
    }
}
