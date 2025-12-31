using UnityEngine;

namespace InventorySample
{
    public class Item
    {
        /// <summary>
        /// アイテム生成時のコンストラクタ
        /// </summary>    
        public Item(string itemName)
        {
            this.itemName = itemName;
        }
        public string ItemName => itemName;

        private string itemName;

    }
}