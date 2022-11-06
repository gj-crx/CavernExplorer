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
        public Vector3Int Position;

        public readonly Stack<Vector3Int> neededPassableOffsets = new Stack<Vector3Int>();
        public readonly Stack<Vector3Int> neededImpassableOffsets = new Stack<Vector3Int>();

        public LevelGate(Sector sectorToCreateGate, Map ReferenceMap, bool WayUp)
        {
            DefineNeededOffsets();
            GoingUp = WayUp;
            Vector3Int RandomPositionInSector = Vector3Int.zero;
            int Attempts = 0;
            while (Attempts < 10000)
            {
                Attempts++;
                RandomPositionInSector = BasicFunctions.ToVector3Int(sectorToCreateGate.GetCentralPoint) + new Vector3Int(GameManager.GenRandom.Next(-sectorToCreateGate.RadiusValue, sectorToCreateGate.RadiusValue),
                    GameManager.GenRandom.Next(-sectorToCreateGate.RadiusValue, sectorToCreateGate.RadiusValue), 0);

                if (ValidPositionForLevelGate(RandomPositionInSector, ReferenceMap))
                {
                    Position = RandomPositionInSector;
                    ReferenceMap.LevelGates.Add(this);
                    break;
                }
            }
            if (Attempts >= 10000) Debug.LogError("Not enough attempts");
        }
        private void DefineNeededOffsets()
        {
            neededPassableOffsets.Push(new Vector3Int(0, -1, 0));
            neededPassableOffsets.Push(new Vector3Int(1, -1, 0));

            neededImpassableOffsets.Push(new Vector3Int(1, 1, 0));
            neededImpassableOffsets.Push(new Vector3Int(0, 1, 0));
            neededImpassableOffsets.Push(new Vector3Int(1, 0, 0));
            neededImpassableOffsets.Push(new Vector3Int(0, 0, 0));
        }

        private bool ValidPositionForLevelGate(Vector3Int tilePosition, Map ReferenceMap)
        {
            foreach (Vector3Int offset in neededImpassableOffsets)
            {
                if (ReferenceMap.LandscapeMap[tilePosition.x + offset.x, tilePosition.y + offset.y] == null
                    || ReferenceMap.LandscapeMap[tilePosition.x + offset.x, tilePosition.y + offset.y].Land != LandType.Impassable)
                {
                    return false;
                }
            }
            foreach (Vector3Int offset in neededPassableOffsets)
            {
                if (ReferenceMap.LandscapeMap[tilePosition.x + offset.x, tilePosition.y + offset.y] == null 
                    || ReferenceMap.LandscapeMap[tilePosition.x + offset.x, tilePosition.y + offset.y].Land != LandType.Passable)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
