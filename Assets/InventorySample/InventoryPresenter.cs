using UniRx;
using UnityEngine;

namespace InventorySample
{
    /// <summary>
    /// Inventory の Model と View を仲介するプレゼンター
    /// </summary>
    public sealed class InventoryPresenter
    {
        

        /// <summary>
        /// Model と View を受け取り、購読を開始する
        /// </summary>
        public InventoryPresenter(UniRxInventory model, InventoryView view,PlayerInputSystem system
            ,InventorySelect select)
        {
            _model = model;
            _view = view;
            _system = system;
            _Select = select;
            Bind();
        }

        private readonly UniRxInventory _model;
        private readonly InventoryView _view;
        private readonly PlayerInputSystem _system;
        private readonly InventorySelect _Select;

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
            _system.OnSelect
                .Subscribe(delta =>
                {
                    _Select.MoveSelection(delta);
                    Debug.Log($"Selected Index: {_Select.SelectedIndex.Value}");
                });
            _Select.SelectedIndex
                .Subscribe(index =>
                {
                    _view.UpdateSelect(index);
                });
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
