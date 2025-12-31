using UnityEngine;
using UniRx;
namespace InventorySample.Develop
{
    public class InventorySample : MonoBehaviour
    {
        private UniRxItemModel _uniRxModel;
        private Inventory _inventory;
        [SerializeField]
        private ItemData[] _itemData;
        private float _timer = 0f;
        private void Start()
        {
            _inventory = new Inventory();
            _uniRxModel = new UniRxItemModel();
            PrintModelCount(_uniRxModel);
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= 2f)
            {
                _timer = 0f;
                _uniRxModel.AddAmount(1);
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



        

    }
}