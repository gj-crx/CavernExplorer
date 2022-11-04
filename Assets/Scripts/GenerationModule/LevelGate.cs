using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generation {
    /// <summary>
    /// represents a tile or tile combination that transfers player between levels
    /// </summary>
    public class LevelGate
    {
        public bool GoingUp = false;
        public Vector3 Position;

        public LevelGate(Sector sectorToCreateGate, Map ReferenceMap, bool WayUp)
        {
            GoingUp = WayUp;
            Vector3Int RandomPositionInSector = Vector3Int.zero;
            int Attempts = 0;
            while (Attempts < 10000)
            {
                Attempts++;
                RandomPositionInSector = new Vector3Int(GameManager.GenRandom.Next(-sectorToCreateGate.RadiusValue, sectorToCreateGate.RadiusValue),
                    GameManager.GenRandom.Next(-sectorToCreateGate.RadiusValue, sectorToCreateGate.RadiusValue), 0);

                if (ReferenceMap.LandscapeMap[RandomPositionInSector.x, RandomPositionInSector.y].Land == LandType.Impassable && TileHasPassableNeigbhours(RandomPositionInSector, ReferenceMap))
                {
                    Position = RandomPositionInSector;
                    ReferenceMap.LevelGates.Add(this);
                    break;
                }
            }
            Debug.LogError("Not enough attempts");
        }

        private bool TileHasPassableNeigbhours(Vector3Int tilePosition, Map ReferenceMap)
        {
            for (int y = - 1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if ((x == 0 || y == 0) && (x != 0 || y != 0))
                    {
                        if (ReferenceMap.LandscapeMap[tilePosition.x + x, tilePosition.y + y] != null && ReferenceMap.LandscapeMap[tilePosition.x + x, tilePosition.y + y].Land == LandType.Passable)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
