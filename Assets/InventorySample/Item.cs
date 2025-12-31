using UnityEngine;

namespace InventorySample
{
    public class Item
    {
        public string ItemName => itemName;
        [SerializeField]
        private string itemName;

    }
}