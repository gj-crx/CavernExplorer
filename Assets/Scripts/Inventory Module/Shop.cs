using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "Shop", menuName = "ScriptableObjects/Shop", order = 0)]
    public class Shop : ScriptableObject
    {
        public string ShopName = "Lidl";
        public List<Item> ItemsBeingSold;
        public float SellingMarging = 1.0f;


    }
}
