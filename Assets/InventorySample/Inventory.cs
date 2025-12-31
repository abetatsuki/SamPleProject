using NUnit.Framework;
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
        public void AddItem(Item item,int amount)
        {
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
        public Dictionary<Item,int> GetItemList()
        {
           return _itemsDic;
        }
        /// <summary>
        /// 所持アイテムと数量を管理する辞書
        /// </summary>
        private Dictionary<Item,int> _itemsDic = new Dictionary<Item,int>();
    }
}