using UnityEngine;

namespace ActionSample.StateMachine
{
    /// <summary>
    /// プレイヤーのジャンプ開始状態を表すステート。
    /// 上方向への瞬間的な力を加え、即座に空中ステートへ遷移します。
    /// </summary>
    public class PlayerJumpState : PlayerState
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">プレイヤーコントローラー</param>
        public PlayerJumpState(PlayerController context) : base(context) { }

        /// <summary>
        /// ステート開始時の処理。
        /// </summary>
        public override void Enter()
        {
            base.Enter();

            // ジャンプ処理
            // 接地状態から空中に飛び上がるため
            Jump();
        }

        /// <summary>
        /// フレーム毎のロジック更新。
        /// </summary>
        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // ジャンプ直後は空中状態へ遷移
            // ジャンプの瞬間的な力積を加えた後は、落下や空中制御を行うAirStateで管理するため
            Context.StateMachine.ChangeState(Context.AirState);
        }

        /// <summary>
        /// ジャンプ力を加える処理
        /// </summary>
        private void Jump()
        {
            // Y軸速度のリセット
            // 落下中などにジャンプした場合でも、常に一定の高さまで跳べるようにするため
            Context.Rigidbody.linearVelocity = new Vector3(Context.Rigidbody.linearVelocity.x, 0f, Context.Rigidbody.linearVelocity.z);

            // 上方向への力（インパルス）を加える
            Context.Rigidbody.AddForce(Vector3.up * Context.JumpForce, ForceMode.Impulse);
        }
    }
}
