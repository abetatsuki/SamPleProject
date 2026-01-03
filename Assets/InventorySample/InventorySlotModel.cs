using UniRx;

namespace InventorySample
{
    /// <summary>
    /// 1つのインベントリスロットのデータを管理するクラス
    /// </summary>
    public class InventorySlotModel
    {
        // 何が入っているか
        public IReadOnlyReactiveProperty<ItemDataSO> ItemData => _itemData;
        // 何個入っているか
        public IReadOnlyReactiveProperty<int> Amount => _amount;

        public bool IsEmpty => _itemData.Value == null;

        private readonly ReactiveProperty<ItemDataSO> _itemData = new ReactiveProperty<ItemDataSO>();
        private readonly ReactiveProperty<int> _amount = new ReactiveProperty<int>(0);

        public void SetItem(ItemDataSO item, int amount)
        {
            _itemData.Value = item;
            _amount.Value = amount;
        }

        public void Clear()
        {
            _itemData.Value = null;
            _amount.Value = 0;
        }

        public void AddAmount(int value)
        {
            _amount.Value += value;
        }
    }
}
