using UnityEngine;

namespace ActionSample.StateMachine
{
    /// <summary>
    /// プレイヤーの空中（落下・滞空）状態を表すステート。
    /// 空中制御（エアコントロール）と着地判定を行います。
    /// </summary>
    public class PlayerAirState : PlayerState
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">プレイヤーコントローラー</param>
        public PlayerAirState(PlayerController context) : base(context) { }

        /// <summary>
        /// ステート開始時の処理。
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            // 特に初期化処理は不要だが、アニメーション制御などがあればここに追加
        }

        /// <summary>
        /// フレーム毎のロジック更新。
        /// </summary>
        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // グラップル入力があればGrappleへ
            if (Context.InputHandler.GrappleInput && Context.GrappleController != null)
            {
                Context.StateMachine.ChangeState(Context.GrappleState);
            }
        }

        /// <summary>
        /// 物理演算の更新。
        /// </summary>
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            // 空中移動制御
            AirControl();

            // 着地判定
            // なぜこの処理が必要なのか: 地面に接触したら通常の移動・待機ステートに戻すため
            // ジャンプ直後の誤検知を防ぐため、上昇中（Y速度が正）は着地判定を行わない、または厳密にする
            if (Context.Rigidbody.linearVelocity.y <= 0.1f && Context.IsGrounded())
            {
                if (Context.InputHandler.MovementInput.sqrMagnitude > 0.01f)
                {
                    Context.StateMachine.ChangeState(Context.WalkState);
                }
                else
                {
                    Context.StateMachine.ChangeState(Context.IdleState);
                }
            }
        }

        /// <summary>
        /// 空中での移動制御
        /// </summary>
        private void AirControl()
        {
            // 入力方向の計算
            Vector3 worldInput = Context.transform.TransformDirection(Context.InputHandler.MovementInput);

            // 移動力を加える（空中制御）
            // なぜこの処理が必要なのか: 空中でも多少の軌道修正を可能にするため（AirMultiplierで制限）
            if (worldInput.magnitude > 0.1f)
            {
                Context.Rigidbody.AddForce(worldInput.normalized * Context.MoveSpeed * Context.AirMultiplier, ForceMode.Force);
            }
            
            // 必要であれば最大速度制限などをここで行う
        }
    }
}
