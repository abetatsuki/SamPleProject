using UnityEngine;

namespace ActionSample.StateMachine
{
    /// <summary>
    /// 敵の待機状態を表すステート。
    /// アイドルアニメーションの再生や、プレイヤー索敵（将来実装）などの待機中の振る舞いを定義します。
    /// </summary>
    public class EnemyIdleState : EnemyState
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">敵コントローラー</param>
        public EnemyIdleState(EnemyController context) : base(context) { }

        /// <summary>
        /// ステート開始時の処理。
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            // 将来の実装: アイドルアニメーションのトリガーなどをここで行う
        }

        /// <summary>
        /// フレーム毎のロジック更新。
        /// </summary>
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            // 将来の実装: プレイヤーが範囲内に入ったかチェックし、追跡ステート（ChaseState）へ遷移する処理などを記述
        }
    }
}
