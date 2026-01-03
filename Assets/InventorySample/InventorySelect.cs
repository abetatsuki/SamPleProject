using UniRx;

namespace InventorySample
{
    /// <summary>
    /// インベントリ内の選択状態を管理するクラス
    /// </summary>
    public class InventorySelect
    {
        public InventorySelect(InventoryModel model)
        {
            _model = model;
        }

        public IReadOnlyReactiveProperty<int> SelectedIndex => _selectedIndex;
        private readonly ReactiveProperty<int> _selectedIndex = new ReactiveProperty<int>(0);
        private readonly InventoryModel _model;

        /// <summary>
        /// 選択位置を移動する（スロット総数に基づいてループ）
        /// </summary>
        public void MoveSelection(int amount)
        {
            int capacity = _model.Capacity;
            if (capacity == 0) return;

            int nextIndex = _selectedIndex.Value + amount;

            if (nextIndex >= capacity)
            {
                nextIndex = 0;
            }
            else if (nextIndex < 0)
            {
                nextIndex = capacity - 1;
            }

            _selectedIndex.Value = nextIndex;
        }
    }
}