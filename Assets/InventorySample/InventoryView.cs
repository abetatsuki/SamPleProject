using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySample
{
    /// <summary>
    /// インベントリ全体のUI表示を管理するクラス
    /// </summary>
    public sealed class InventoryView : MonoBehaviour
    {
        [SerializeField]
        private InventorySlotUI _slotPrefab;

        [SerializeField]
        private GridLayoutGroup _gridLayoutGroup;

        [SerializeField]
        private int _gridWidth = 3;

        [SerializeField]
        private int _gridHeight = 3;

        public int TotalSlots => _gridWidth * _gridHeight;

        private readonly List<InventorySlotUI> _slots = new List<InventorySlotUI>();

        private void Awake()
        {
            CreateSlots(TotalSlots);
        }

        /// <summary>
        /// 指定インデックスのスロット表示を更新する
        /// </summary>
        public void UpdateSlot(int index, ItemDataSO item)
        {
            if (index < 0 || index >= _slots.Count) return;

            if (item != null)
            {
                _slots[index].SetItem(item);
            }
            else
            {
                _slots[index].ClearItem();
            }
        }

        /// <summary>
        /// 選択状態の表示を更新する
        /// </summary>
        public void UpdateSelect(int index)
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                bool isSelected = (i == index);
                _slots[i].SetSelected(isSelected);
            }
        }

        /// <summary>
        /// スロットを生成する
        /// </summary>
        private void CreateSlots(int count)
        {
            if(_slotPrefab == null || _gridLayoutGroup == null)
            {
                Debug.LogError("プレハブまたはGridLayoutGroupが設定されていません");
                return;
            }

            // 既存の表示をクリア
            foreach (Transform child in _gridLayoutGroup.transform)
            {
                Destroy(child.gameObject);
            }
            _slots.Clear();

            for (int i = 0; i < count; i++)
            {
                InventorySlotUI slot = Instantiate(_slotPrefab, _gridLayoutGroup.transform);
                slot.ClearItem();
                _slots.Add(slot);
            }
        }
    }
}