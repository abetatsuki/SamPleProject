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

        }


        [SerializeField]
        private InventorySlotUI slotPrefab;

        [SerializeField]
        private GridLayoutGroup gridLayoutGroup;

        [SerializeField]
        private int gridWidth = 3;

        [SerializeField]
        private int gridHeight = 3;

        private readonly List<InventorySlotUI> _slots = new List<InventorySlotUI>();

        private void Start()
        {
            CreateSlots(gridWidth, gridHeight);
        }

        /// <summary>
        /// 指定サイズのインベントリスロットを生成する
        /// </summary>
        private void CreateSlots(int width, int height)
        {
            int slotCount = width * height;

            for (int i = 0; i < slotCount; i++)
            {
                InventorySlotUI slot = Instantiate(slotPrefab, gridLayoutGroup.transform);
                slot.ClearItem();
                _slots.Add(slot);
            }
        }
    }
}
