using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "Possible drop", menuName = "ScriptableObjects/PossibleDrop", order = 2)]
    public class PossibleDrop : ScriptableObject
    {
        public Item DroppedItem = null;
        public float DropChance = 0.5f;

        public static List<Item> GenerateItems(List<PossibleDrop> possibleDrop)
        {
            List<Item> itemsToDrop = new List<Item>();
            foreach (PossibleDrop drop in possibleDrop)
            {
                if (Random.Range(0.0f, 1.0f) < drop.DropChance) itemsToDrop.Add(drop.DroppedItem);
            }
            return itemsToDrop;
        }
    }
}
