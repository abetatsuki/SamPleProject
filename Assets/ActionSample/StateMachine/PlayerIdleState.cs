using UnityEngine;

namespace ActionSample.StateMachine
{
    /// <summary>
    /// プレイヤーの待機状態を表すステート。
    /// 入力がない時の停止処理や、移動・スライディングへの遷移判定を行います。
    /// </summary>
    public class PlayerIdleState : PlayerState
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">プレイヤーコントローラー</param>
        public PlayerIdleState(PlayerController context) : base(context) { }

        /// <summary>
        /// ステート開始時の処理。
        /// </summary>
        public override void Enter()
        {
            // 水平方向の速度をゼロにする（重力の影響を受けるY軸速度は維持）
            // なぜこの処理が必要なのか: 前のステート（歩行など）の慣性を断ち切り、即座に停止させるため
            Vector3 currentVel = Context.Rigidbody.linearVelocity;
            Context.Rigidbody.linearVelocity = new Vector3(0, currentVel.y, 0);
        }

        /// <summary>
        /// フレーム毎のロジック更新。
        /// </summary>
        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // 移動入力のチェック
            // なぜこの処理が必要なのか: 入力されたら即座に歩行ステートへ遷移させるため
            if (Context.InputHandler.MovementInput.sqrMagnitude > 0.01f)
            {
                Context.StateMachine.ChangeState(Context.WalkState);
            }
            
            // スライディング入力のチェック
            // なぜこの処理が必要なのか: 停止状態からでもスライディング（しゃがみダッシュ等）を開始できるようにするため
            if(Context.InputHandler.SlideTriggered)
            {
                Context.StateMachine.ChangeState(Context.SlideState);
            }

            // グラップル入力のチェック
            // なぜこの処理が必要なのか: 待機状態から即座にグラップルアクションへ移行するため
            if (Context.InputHandler.GrappleInput && Context.GrappleController != null)
            {
                Context.StateMachine.ChangeState(Context.GrappleState);
            }
        }
    }
}
