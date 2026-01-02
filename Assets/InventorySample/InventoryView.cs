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
        public int SlotCount => _gridWidth * _gridHeight;
        /// <summary>
        /// 空いているスロットにアイテムを表示する
        /// </summary>
        public void AddItem(ItemDataSO itemData, int amount)
        {
            InventorySlotUI emptySlot = _slots.Find(slot => slot.IsEmpty());

            if (emptySlot == null)
            {
                Debug.LogWarning("インベントリに空きスロットがありません");
                return;
            }

            emptySlot.SetItem(itemData);
        }

        /// <summary>
        /// 指定アイテムを表示しているスロットを空にする
        /// </summary>
        public void RemoveItem(ItemDataSO itemData)
        {
            foreach (InventorySlotUI slot in _slots)
            {
                if (slot.IsEmpty())
                {
                    continue;
                }

                if (slot.IsItem(itemData))
                {
                    slot.ClearItem();
                    return;
                }
            }
        }

        public void UpdateSelect(int index)
        {
            for(int i = 0; i < _slots.Count; i++)
            {
                bool isSelected = (i == index);
                _slots[i].SetSelected(isSelected);
            }
        }


        [SerializeField]
        private InventorySlotUI _slotPrefab;

        [SerializeField]
        private GridLayoutGroup _gridLayoutGroup;

        [SerializeField]
        private int _gridWidth = 3;

        [SerializeField]
        private int _gridHeight = 3;

        private readonly List<InventorySlotUI> _slots = new List<InventorySlotUI>();

        private void Start()
        {
            CreateSlots(_gridWidth, _gridHeight);
        }

        /// <summary>
        /// 指定サイズのインベントリスロットを生成する
        /// </summary>
        private void CreateSlots(int width, int height)
        {
            int slotCount = width * height;

            for (int i = 0; i < slotCount; i++)
            {
                InventorySlotUI slot = Instantiate(_slotPrefab, _gridLayoutGroup.transform);
                slot.ClearItem();
                _slots.Add(slot);
            }
        }
    }
}
