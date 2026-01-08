using UnityEngine;

namespace ActionSample.StateMachine
{
    /// <summary>
    /// プレイヤーのスライディング状態を表すステート。
    /// 一時的な加速と姿勢制御、およびステート終了時の遷移処理を行います。
    /// </summary>
    public class PlayerSlideState : PlayerState
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">プレイヤーコントローラー</param>
        public PlayerSlideState(PlayerController context) : base(context) { }

        /// <summary>
        /// ステート開始時の処理。
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            
            // スライディングの持続時間を設定
            _slideTimer = Context.SlideDuration;

            // スライディング方向の決定
            // なぜこの処理が必要なのか: 移動入力がある場合はその方向へ、入力がない場合は向いている方向（前方）へ滑らせるため
            // 入力の有無で分岐し、ローカル入力をワールド座標へ変換する
            Vector3 worldInput = Context.InputHandler.MovementInput.magnitude > 0.1f 
                 ? Context.transform.TransformDirection(Context.InputHandler.MovementInput)
                 : Context.transform.forward;

            // 初速を与える
            // なぜこの処理が必要なのか: スライディング特有の瞬間的な加速を表現するため
            Context.Rigidbody.linearVelocity = worldInput * Context.SlideSpeed;
            
            // 空気抵抗（ドラッグ）の設定
            // なぜこの処理が必要なのか: スライディング後半にかけて徐々に減速させ、自然な停止感を出すため
            Context.Rigidbody.linearDamping = Context.SlideDrag;
        }

        /// <summary>
        /// ステート終了時の処理。
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            
            // スケール復元時の地面めり込み回避
            // なぜこの処理が必要なのか: スケールを戻すとコライダーが地面に食い込み、物理演算で大きく弾き飛ばされてしまうのを防ぐため
            float scaleDifference = _originalScale.y - Context.transform.localScale.y;
            if (scaleDifference > 0)
            {
                float heightOffset = scaleDifference * Context.PlayerHeight * 0.5f;
                Context.transform.position += Vector3.up * heightOffset;
            }

            // スケールを元に戻す
            // なぜこの処理が必要なのか: スライディング終了後に通常の立ち姿勢に戻すため
            Context.transform.localScale = _originalScale;

            // 空気抵抗をリセット
            // なぜこの処理が必要なのか: 通常移動（歩行など）に戻った際に、スライディング用の減速設定を引き継がないようにするため
            Context.Rigidbody.linearDamping = 0f;
        }

        /// <summary>
        /// フレーム毎のロジック更新。
        /// </summary>
        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // ジャンプ入力があればJumpへ
            if (Context.InputHandler.JumpTriggered)
            {
                Context.StateMachine.ChangeState(Context.JumpState);
            }

            // スライディング中は他の入力を受け付けない（キャンセル等の仕様があればここに追記）
            // 現状は強制的にタイマー終了まで滑り続ける仕様
        }

        /// <summary>
        /// 物理演算の更新。
        /// </summary>
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            // タイマー減算
            _slideTimer -= Time.fixedDeltaTime;

            // スライディング終了判定
            // なぜこの処理が必要なのか: 設定された時間が経過したら、次のアクション（歩行または待機）へ遷移させるため
            if (_slideTimer <= 0f)
            {
                // 入力があればWalk、なければIdleへ遷移
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

        private float _slideTimer;
    }
}
