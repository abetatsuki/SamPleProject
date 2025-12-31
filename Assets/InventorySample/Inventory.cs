using System.Collections.Generic;
using UnityEngine;

namespace InventorySample
{
    public class Inventory
    {
        /// <summary>
        /// アイテムを所持リストに追加する
        /// すてに所持してる場所合は数量を加算する
        /// </summary>
        public void AddItem(ItemData item, int amount)
        {
            if (amount == 0) return;
            // すでに同じアイテムを所持しているか確認する
            if (_itemsDic.ContainsKey(item))
            {
                // 既存の数量に加算する
                _itemsDic[item] += amount;
            }
            else
            {
                // 新しいアイテムとして追加する
                _itemsDic.Add(item, amount);
            }
        }
        /// <summary>
        /// 現在の所持リストを取得する
        /// </summary>
        public Dictionary<ItemData, int> GetItemList()
        {
            return _itemsDic;
        }

        public void PrintItemList()
        {
            foreach (var kvp in _itemsDic)
            {
                Debug.Log($"Item: {kvp.Key.ItemName}, Amount: {kvp.Value}");
            }
        }
        /// <summary>
        /// 所持アイテムと数量を管理する辞書
        /// </summary>
        private Dictionary<ItemData, int> _itemsDic = new Dictionary<ItemData, int>();
    }
}