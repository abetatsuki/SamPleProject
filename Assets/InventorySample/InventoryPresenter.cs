using UniRx;

namespace InventorySample
{
    /// <summary>
    /// Inventory の Model と View を仲介するプレゼンター
    /// </summary>
    public sealed class InventoryPresenter
    {
        public InventoryPresenter(InventoryModel model, InventoryView view, PlayerInputSystem system, InventorySelect select)
        {
            _model = model;
            _view = view;
            _system = system;
            _select = select;
            Bind();
        }

        private readonly InventoryModel _model;
        private readonly InventoryView _view;
        private readonly PlayerInputSystem _system;
        private readonly InventorySelect _select;

        /// <summary>
        /// Model の各スロットの状態を監視して View に反映する
        /// </summary>
        private void Bind()
        {
            // 各スロットの状態変更を監視
            for (int i = 0; i < _model.Capacity; i++)
            {
                int index = i; // クロージャ用
                _model.Slots[i].ItemData
                    .Subscribe(item =>
                    {
                        _view.UpdateSlot(index, item);
                    })
                    .AddTo(_view);
            }

            // 入力による選択移動
            _system.OnSelect
                .Subscribe(delta =>
                {
                    _select.MoveSelection(delta);
                })
                .AddTo(_view);
                
            // 選択インデックスの変化を監視
            _select.SelectedIndex
                .Subscribe(index =>
                {
                    _view.UpdateSelect(index);
                })
                .AddTo(_view);
        }
    }
}