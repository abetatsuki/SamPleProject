using UnityEngine;

namespace ActionSample.StateMachine
{
    /// <summary>
    /// グラップルの「スイング（物理振り子）」挙動を担当するサブステート。
    /// 物理ジョイントによる移動と、プレイヤー入力による空中制御（Air Control）を行います。
    /// </summary>
    public class GrappleSwingState : PlayerState
    {
        private PlayerGrappleState _parentState;
        private float _startTime;
        private const float TapThreshold = 0.25f;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">プレイヤーコントローラー</param>
        /// <param name="parentState">親となるグラップルステート</param>
        public GrappleSwingState(PlayerController context, PlayerGrappleState parentState) : base(context)
        {
            _parentState = parentState;
        }

        public override void Enter()
        {
            base.Enter();
            _startTime = Time.time;
            
            // Swing開始：物理接続を試みる
            if (!Context.GrappleController.StartGrapple())
            {
                // 失敗時は親ステートに終了を依頼
                _parentState.FinishGrapple();
            }
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // 入力解除の監視
            if (!Context.InputHandler.GrappleInputHeld)
            {
                float duration = Time.time - _startTime;

                // 短押し（タップ）ならPullモードへ遷移
                // なぜこの処理が必要なのか: プレイヤーがスイングではなく高速移動を意図した操作に対応するため
                if (duration < TapThreshold)
                {
                    Debug.Log("Grapple: Switch to Pull SubState");
                    _parentState.SwitchToPull();
                }
                else
                {
                    // 長押しの後に離したならスイング終了（慣性ジャンプ）
                    Debug.Log("Grapple: End Swing");
                    _parentState.FinishGrapple();
                }
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            
            // 空中操作（Air Control）の適用
            float horizontal = Context.InputHandler.MovementInput.x;
            float vertical = Context.InputHandler.MovementInput.z;
            Context.GrappleController.ApplyAirControl(horizontal, vertical);
        }
    }
}
