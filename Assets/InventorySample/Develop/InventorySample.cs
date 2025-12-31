using UnityEngine;

namespace InventorySample.Develop
{
  public class InventorySample : MonoBehaviour
  {
        private void Start()
        {
            _inventory = new Inventory();
            foreach (var itemData in _itemData)
            {
                // 各アイテムを5個ずつ追加する
                _inventory.AddItem(itemData, 5);
            }
            _inventory.PrintItemList();
        }

        private Inventory _inventory;
        [SerializeField]
        private ItemData[] _itemData;

    }
}