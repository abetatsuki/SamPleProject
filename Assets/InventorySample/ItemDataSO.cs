using UnityEngine;

namespace InventorySample
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
    public class ItemDataSO : ScriptableObject
    {
        public string ItemName => _itemName;
        public int ItemId => _itemId;
        public Sprite Icon => _icon;

        [SerializeField, Tooltip("アイテム名")]
        private string _itemName;
        [SerializeField, Tooltip("アイテムID")]
        private int _itemId;
        [SerializeField, Tooltip("アイテムのアイコン")]
        private Sprite _icon;

    }
}