using UniRx;
using UnityEngine;

namespace InventorySample
{
    /// <summary>
    /// インベントリ内のアイテム選択状態（カーソル位置）を管理するクラス
    /// </summary>
    public class InventorySelect
    {

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="inventory">管理対象のインベントリ</param>
        public InventorySelect(UniRxInventory inventory)
        {
            _inventory = inventory;
        }

        /// <summary>
        /// 現在選択されているインデックス（読み取り専用）
        /// 変更を監視可能
        /// </summary>
        public IReadOnlyReactiveProperty<int> SelectedIndex => _selectedIndex;

        /// <summary>
        /// 選択インデックスを指定した量だけ移動させる
        /// インベントリのアイテム数に基づいてループ処理を行う
        /// </summary>
        /// <param name="amount">移動量（+1 で次へ、-1 で前へ）</param>
        public void MoveSelection(int amount)
        {
            // アイテム総数を取得
            var count = _inventory.ItemsDic.Count;

            // アイテムがない場合は常に0にして終了
            if (count == 0)
            {
                _selectedIndex.Value = 0;
                return;
            }

            // 次のインデックス候補を計算
            var nextIndex = _selectedIndex.Value + amount;

            // 末尾を超えたら先頭へ、先頭より前なら末尾へ移動する
            if (nextIndex >= count)
            {
                nextIndex = 0;
            }
            else if (nextIndex < 0)
            {
                nextIndex = count - 1;
            }

            // 計算したインデックスを適用し、変更を通知する
            _selectedIndex.Value = nextIndex;
        }

        /// <summary>
        /// 操作対象のインベントリモデル
        /// アイテム数の確認に使用する
        /// </summary>
        private readonly UniRxInventory _inventory;

        /// <summary>
        /// 現在選択されているインデックスを保持するReactiveProperty
        /// </summary>
        private readonly ReactiveProperty<int> _selectedIndex =
            new ReactiveProperty<int>(0);
    }
}
