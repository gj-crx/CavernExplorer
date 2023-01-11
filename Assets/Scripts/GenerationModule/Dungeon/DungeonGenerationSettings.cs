using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Generation
{
    [System.Serializable]
    public class DungeonGenerationSettings
    {
        public int XRadius = 20;
        public int YRadius = 20;


        public int CorridorsNumberToGenerateMax = 10;
        public int CorridorMaxLength = 15;
        public int CorridorMaxWidth = 2;
        
    }
}
