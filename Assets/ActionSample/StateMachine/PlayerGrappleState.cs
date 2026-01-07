using UnityEngine;

namespace ActionSample.StateMachine
{
    /// <summary>
    /// グラップリングアクション中のステート。
    /// 入力時間に応じて「スイング（物理振り子）」と「プル（直線移動）」の2つの挙動を切り替えます。
    /// </summary>
    public class PlayerGrappleState : PlayerState
    {
        private enum GrappleMode
        {
            Swing, // 物理演算による振り子運動
            Pull   // ターゲットへの直線吸い寄せ
        }

        private GrappleMode _currentMode;
        private float _grappleStartTime;
        
        /// <summary>
        /// 短押し判定の閾値（秒）。
        /// これより早く離すとプル、押し続けるとスイングになります。
        /// </summary>
        private const float TapThreshold = 0.25f;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">プレイヤーコントローラー</param>
        public PlayerGrappleState(PlayerController context) : base(context) { }

        /// <summary>
        /// ステート開始時の処理。
        /// </summary>
        public override void Enter()
        {
            base.Enter();

            // デフォルトはSwingモードで開始
            // なぜこの処理が必要なのか: ボタンを押した瞬間に物理接続を行い、即座にフィードバック（ロープ生成）を返すため
            _currentMode = GrappleMode.Swing;
            _grappleStartTime = Time.time;

            // グラップルの開始試行
            if (!Context.GrappleController.StartGrapple())
            {
                // 失敗したら即終了
                TransitionToMovementState();
            }
        }

        /// <summary>
        /// ステート終了時の処理。
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            // どちらのモードであっても最後は必ずグラップル処理を停止する
            Context.GrappleController.StopGrapple();
        }

        /// <summary>
        /// フレーム毎のロジック更新。
        /// モードに応じた終了判定や遷移判定を行います。
        /// </summary>
        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (_currentMode == GrappleMode.Swing)
            {
                // Swingモード中のロジック：入力解除の監視

                // ボタンが離された場合
                if (!Context.InputHandler.GrappleInputHeld)
                {
                    float duration = Time.time - _grappleStartTime;

                    // 押していた時間が短ければ「Pull」へ移行
                    // なぜこの処理が必要なのか: プレイヤーが「移動」を意図してチョン押しした場合に対応するため
                    if (duration < TapThreshold)
                    {
                        Debug.Log("Grapple: Switch to Pull Mode");
                        _currentMode = GrappleMode.Pull;
                    }
                    else
                    {
                        // 長押ししていた場合は「Swing」終了として扱う（慣性ジャンプ）
                        Debug.Log("Grapple: End Swing");
                        TransitionToMovementState();
                    }
                }
            }
            else if (_currentMode == GrappleMode.Pull)
            {
                // Pullモード中のロジック：到達判定

                // ターゲットに到達したら終了
                // なぜこの処理が必要なのか: 直線移動が完了したため
                if (Context.GrappleController.IsAtGrapplePoint())
                {
                    Debug.Log("Grapple: Reached Target");
                    // 勢いを殺すか残すかは調整次第だが、ここでは少し上に跳ねさせて着地しやすくする
                    Context.Rigidbody.linearVelocity = Vector3.up * 5f;
                    TransitionToMovementState();
                }
            }
        }

        /// <summary>
        /// 物理更新タイミングの処理。
        /// 現在のモードに応じた物理操作をコントローラーに依頼します。
        /// </summary>
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if (_currentMode == GrappleMode.Swing)
            {
                // Swing中：Air Control（空中制御）を適用
                // なぜこの処理が必要なのか: 物理ジョイントにぶら下がっている状態で、プレイヤーの入力を反映させるため
                float horizontal = Context.InputHandler.MovementInput.x;
                float vertical = Context.InputHandler.MovementInput.z;
                Context.GrappleController.ApplyAirControl(horizontal, vertical);
            }
            else
            {
                // Pull中：ターゲットへの強制移動を実行
                // なぜこの処理が必要なのか: Rigidbodyの速度を毎フレーム更新して、ターゲットへ誘導するため
                Context.GrappleController.ExecutePull();
            }
        }

        /// <summary>
        /// 移動系のステートへの遷移を管理します。
        /// </summary>
        private void TransitionToMovementState()
        {
            // 接地や入力状況に応じて遷移先を決定
            // 基本的にはIdleかWalkに戻す
            if (Context.InputHandler.MovementInput != Vector3.zero)
            {
                Context.StateMachine.ChangeState(Context.WalkState);
            }
            else
            {
                Context.StateMachine.ChangeState(Context.IdleState);
            }
        }
    }
}
