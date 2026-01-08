using UnityEngine;

namespace ActionSample.StateMachine
{
    /// <summary>
    /// プレイヤーの歩行（移動）状態を表すステート。
    /// 入力に応じた移動処理と、待機・スライディングへの遷移判定を行います。
    /// </summary>
    public class PlayerWalkState : PlayerState
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">プレイヤーコントローラー</param>
        public PlayerWalkState(PlayerController context) : base(context) { }

        /// <summary>
        /// フレーム毎のロジック更新。
        /// </summary>
        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // 入力判定：移動入力がなくなったらIdleへ
            // プレイヤーがキーを離した瞬間に移動を止め、待機状態に戻すため
            if (Context.InputHandler.MovementInput.sqrMagnitude < 0.01f)
            {
                Context.StateMachine.ChangeState(Context.IdleState);
            }
            // 入力判定：スライディング入力（Cキー）があればSlideへ
            // 単発のスライディング（回避や短いダッシュ）を実行するため
            else if (Context.InputHandler.SlideTriggered)
            {
                Context.StateMachine.ChangeState(Context.SlideState);
            }
            // 入力判定：滑走入力（左Ctrlキー）があればSlidingへ
            // 斜面での滑走や継続的なスライディングを実行するため
            else if (Context.InputHandler.SlidingTriggered)
            {
                Context.StateMachine.ChangeState(Context.SlidingState);
            }
            // 入力判定：ジャンプ入力があればJumpへ
            // 移動中に障害物を飛び越えたり段差を登るため
            else if (Context.InputHandler.JumpTriggered)
            {
                Context.StateMachine.ChangeState(Context.JumpState);
            }
            // 入力判定：グラップル入力があればGrappleへ
            // 移動中から即座にグラップルアクションへ移行するため
            else if (Context.InputHandler.GrappleInput && Context.GrappleController != null)
            {
                Context.StateMachine.ChangeState(Context.GrappleState);
            }
        }

        /// <summary>
        /// 物理演算の更新（移動処理）。
        /// </summary>
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            
            // ローカル入力ベクトルをワールド座標系に変換
            // プレイヤーの現在の向き（またはカメラの向き）に基づいた方向に移動させるため
            // Context.transform.TransformDirection はローカル(X, Z)をワールド方向へ変換する
            Vector3 worldInput = Context.transform.TransformDirection(Context.InputHandler.MovementInput);
            
            // 現在の移動速度を決定
            // エイム中（構え中）は精密な操作が必要なため、通常より遅い移動速度を適用するため
            float currentSpeed = Context.InputHandler.AimInput ? Context.AimMoveSpeed : Context.MoveSpeed;
            
            // ターゲット速度の計算
            Vector3 targetVelocity = worldInput * currentSpeed;
            
            // Y軸（重力）の維持
            // 移動計算でY軸を上書きしてしまうと、落下やジャンプができなくなるため
            // 既存のY軸速度（重力加速度の結果）をそのまま適用する
            targetVelocity.y = Context.Rigidbody.linearVelocity.y; 
            
            // 最終的な速度をRigidbodyに適用
            Context.Rigidbody.linearVelocity = targetVelocity;
        }
    }
}
