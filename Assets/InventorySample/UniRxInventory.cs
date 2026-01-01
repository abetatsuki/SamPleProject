using System.Linq;
using UniRx;
using UnityEngine;
namespace InventorySample
{
    public class UniRxInventory
    {
        public IReadOnlyReactiveDictionary<ItemDataSO, int> ItemsDic => _itemsDic;

        /// <summary>
        /// アイテムを所持リストに追加する
        /// すてに所持してる場所合は数量を加算する
        /// </summary>
        public void AddItem(ItemDataSO item, int amount)
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
        /// アイテムを所持リストから削除する    
        /// </summary>
        public void RemoveItem(ItemDataSO item, int amount)
        {
            if (amount == 0) return;
            // アイテムが存在するか確認する
            if (!_itemsDic.ContainsKey(item)) return;

            // 数量を減算する
            _itemsDic[item] -= amount;
            // 数量が0以下になった場合、アイテムを削除する
            if (_itemsDic[item] <= 0)
            {
                _itemsDic.Remove(item);
            }

        }
        /// <summary>
        /// 現在の所持リストを取得する
        /// </summary>
        public ReactiveDictionary<ItemDataSO, int> GetItemList()
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

        private ReactiveDictionary<ItemDataSO, int> _itemsDic = new ReactiveDictionary<ItemDataSO, int>();


    }
}