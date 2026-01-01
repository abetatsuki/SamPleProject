using UniRx;

namespace InventorySample
{
    /// <summary>
    /// Inventory の Model と View を仲介するプレゼンター
    /// </summary>
    public sealed class InventoryPresenter
    {
        private readonly UniRxInventory _model;
        private readonly InventoryView _view;

        /// <summary>
        /// Model と View を受け取り、購読を開始する
        /// </summary>
        public InventoryPresenter(UniRxInventory model, InventoryView view)
        {
            _model = model;
            _view = view;

            Bind();
        }

        /// <summary>
        /// Model の変更を購読して View に反映する
        /// </summary>
        private void Bind()
        {
            _model.ItemsDic
                .ObserveAdd()
                .Subscribe(OnItemAdded);

            _model.ItemsDic
                .ObserveRemove()
                .Subscribe(OnItemRemoved);
        }

        /// <summary>
        /// アイテムが追加されたときの処理
        /// </summary>
        private void OnItemAdded(DictionaryAddEvent<ItemDataSO, int> addEvent)
        {
            _view.AddItem(addEvent.Key, addEvent.Value);
        }

        /// <summary>
        /// アイテムが削除されたときの処理
        /// </summary>
        private void OnItemRemoved(DictionaryRemoveEvent<ItemDataSO, int> removeEvent)
        {
            _view.RemoveItem(removeEvent.Key);
        }
    }
}
