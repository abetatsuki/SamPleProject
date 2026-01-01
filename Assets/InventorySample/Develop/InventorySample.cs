using UniRx;
using UnityEngine;
namespace InventorySample.Develop
{
    public class InventorySample : MonoBehaviour
    {
        private UniRxItemModel _uniRxModel;
        private UniRxInventory _uniRxInventory;
        private Inventory _inventory;
        [SerializeField]
        private ItemDataSO[] _itemData;
        private float _timer = 0f;
        private void Start()
        {
            _inventory = new Inventory();
            _uniRxModel = new UniRxItemModel();
            _uniRxInventory = new UniRxInventory();
            PrintModelCount(_uniRxModel);
            PrintInventory(_uniRxInventory);
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= 2f)
            {
                _timer = 0f;
                //  _uniRxModel.AddAmount(1);
                _uniRxInventory.AddItem(_itemData[Random.Range(0, _itemData.Length)], 1);
                _uniRxInventory.RemoveItem(_itemData[Random.Range(0, _itemData.Length)], 1);
            }

        }

        private void InventoryTest()
        {

            foreach (var itemData in _itemData)
            {
                // 各アイテムを5個ずつ追加する
                _inventory.AddItem(itemData, 5);
            }
            _inventory.PrintItemList();
            _inventory.PrintItemListItemId();
        }

        /// <summary>
        /// モデルの Amount を購読して表示する
        /// </summary>
        private void PrintModelCount(UniRxItemModel model)
        {
            model.Amount
                .Subscribe(amount =>
                {
                    // Amount が更新されたときの処理
                    Debug.Log($"Current Amount: {amount}");
                })
                .AddTo(this);
        }

        /// <summary>
        /// インベントリの所持アイテムを購読して表示する
        /// </summary>
        private void PrintInventory(UniRxInventory inventory)
        {
            inventory.ItemsDic
                .ObserveAdd()
                .Subscribe(addEvent =>
                {
                    Debug.Log($"Added Item: {addEvent.Key.ItemName}, Amount: {addEvent.Value}");
                })
                .AddTo(this);
            inventory.ItemsDic
                .ObserveReplace()
                .Subscribe(replaceEvent =>
                {
                    Debug.Log($"Updated Item: {replaceEvent.Key.ItemName}, New Amount: {replaceEvent.NewValue}");
                })
                .AddTo(this);
            inventory.ItemsDic
                .ObserveRemove()
                .Subscribe(x =>
                {
                    // 削除されたアイテムを表示する
                    Debug.Log($"Removed: {x.Key.name}");
                })
                .AddTo(this);

        }



    }
}