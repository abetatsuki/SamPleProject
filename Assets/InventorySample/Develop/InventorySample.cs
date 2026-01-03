using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InventorySample.Develop
{
    [RequireComponent(typeof(PlayerInput))]
    public class InventorySample : MonoBehaviour
    {
        private InventoryModel _inventoryModel;
        private InventoryView _view;
        private InventoryPresenter _presenter;
        private PlayerInputSystem _inputSystem;
        private InventorySelect _select;

        [SerializeField] private ItemDataSO[] _itemData;

        private float _timer = 0f;

        private void Start()
        {
            // View の参照取得（シリアライズされていなければ GetComponent）
            if (_view == null) _view = FindFirstObjectByType<InventoryView>();

            // Input System の初期化
            var playerInput = GetComponent<PlayerInput>();
            _inputSystem = new PlayerInputSystem(playerInput);

            // スロットベースの Model を生成 (View のスロット数に合わせる)
            int capacity = _view != null ? _view.TotalSlots : 9;
            _inventoryModel = new InventoryModel(capacity);

            // Select と Presenter の構築
            _select = new InventorySelect(_inventoryModel);
            _presenter = new InventoryPresenter(_inventoryModel, _view, _inputSystem, _select);

            Debug.Log("Inventory Initialized with " + capacity + " slots.");
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= 2f)
            {
                _timer = 0f;
                if (_itemData != null && _itemData.Length > 0)
                {
                    var randomItem = _itemData[Random.Range(0, _itemData.Length)];
                    _inventoryModel.AddItem(randomItem, 1);
                }
            }
        }
    }
}