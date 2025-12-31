using UnityEngine;

namespace InventorySample
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
    public class ItemData : ScriptableObject
    {
        public string ItemName => _itemName;
        public int ItemId => _itemId;

        [SerializeField, Tooltip("アイテム名")]
        private string _itemName;
        [SerializeField, Tooltip("アイテムID")]
        private int _itemId;

    }
}