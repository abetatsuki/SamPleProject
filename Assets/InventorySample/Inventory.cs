using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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

        /// <summary>
        /// 現在の所持リストをコンソールに表示する
        /// </summary>
        public void PrintItemList()
        {
            foreach (var kvp in _itemsDic)
            {
                Debug.Log($"Item: {kvp.Key.ItemName}, Amount: {kvp.Value}");
            }
        }

        /// <summary>
        /// 現在の所持リストをアイテムID順にコンソールに表示する    
        ///</summary>   
        public void PrintItemListItemId()
        {
            foreach (var kvp in _itemsDic.OrderBy(kvp => kvp.Key.ItemId))
            {
                Debug.Log($"Item ID: {kvp.Key.ItemId}, Item: {kvp.Key.ItemName}, Amount: {kvp.Value}");
            }
        }
        /// <summary>
        /// 所持アイテムと数量を管理する辞書
        /// </summary>
        private Dictionary<ItemData, int> _itemsDic = new Dictionary<ItemData, int>();
    }
}