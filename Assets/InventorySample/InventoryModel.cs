using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InventorySample
{
    /// <summary>
    /// スロットベースのインベントリデータ管理クラス
    /// </summary>
    public class InventoryModel
    {
        public IReadOnlyList<InventorySlotModel> Slots => _slots;
        public int Capacity => _slots.Count;

        private readonly List<InventorySlotModel> _slots;

        public InventoryModel(int capacity)
        {
            _slots = new List<InventorySlotModel>();
            for (int i = 0; i < capacity; i++)
            {
                _slots.Add(new InventorySlotModel());
            }
        }

        /// <summary>
        /// アイテムをインベントリに追加する
        /// </summary>
        public bool AddItem(ItemDataSO item, int amount)
        {
            // 1. すでに同じアイテムを持っているスロットがあれば加算
            var existingSlot = _slots.FirstOrDefault(s => !s.IsEmpty && s.ItemData.Value == item);
            if (existingSlot != null)
            {
                existingSlot.AddAmount(amount);
                return true;
            }

            // 2. 空いているスロットを探して追加
            var emptySlot = _slots.FirstOrDefault(s => s.IsEmpty);
            if (emptySlot != null)
            {
                emptySlot.SetItem(item, amount);
                return true;
            }

            Debug.LogWarning("インベントリが一杯です");
            return false;
        }

        /// <summary>
        /// 指定インデックスのアイテムを削除する
        /// </summary>
        public void RemoveItem(int index)
        {
            if (index < 0 || index >= _slots.Count) return;
            _slots[index].Clear();
        }
    }
}
