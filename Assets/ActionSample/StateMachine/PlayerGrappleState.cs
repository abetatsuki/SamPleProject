using UnityEngine;

namespace ActionSample.StateMachine
{
    /// <summary>
    /// グラップリングアクションを統括する親ステート。
    /// 内部にサブステートマシンを持ち、「スイング」と「プル」の具体的な挙動を委譲します。
    /// </summary>
    public class PlayerGrappleState : PlayerState
    {
        // サブステートマシン
        private StateMachine _subStateMachine;
        
        // サブステートのインスタンス
        private GrappleSwingState _swingState;
        private GrapplePullState _pullState;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">プレイヤーコントローラー</param>
        public PlayerGrappleState(PlayerController context) : base(context)
        {
            // サブステートマシンの初期化
            _subStateMachine = new StateMachine();
            
            // 各サブステートの生成（自身をParentとして渡す）
            _swingState = new GrappleSwingState(context, this);
            _pullState = new GrapplePullState(context, this);
        }

        /// <summary>
        /// ステート開始時の処理。
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            
            // 常にSwingステートから開始する
            // なぜこの処理が必要なのか: グラップルは常に「発射→接続（スイング）」から始まり、その後の入力時間でプルに派生するため
            _subStateMachine.Initialize(_swingState);
        }

        /// <summary>
        /// ステート終了時の処理。
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            
            // サブステートの終了処理も確実に行う
            if (_subStateMachine.CurrentState != null)
            {
                _subStateMachine.CurrentState.Exit();
            }

            // 念のためグラップル機能を停止（安全策）
            Context.GrappleController.StopGrapple();
        }

        /// <summary>
        /// フレーム毎のロジック更新。
        /// 現在のサブステートに委譲します。
        /// </summary>
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            _subStateMachine.CurrentState.LogicUpdate();
        }

        /// <summary>
        /// 物理更新タイミングの処理。
        /// 現在のサブステートに委譲します。
        /// </summary>
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            _subStateMachine.CurrentState.PhysicsUpdate();
        }

        // ========================================================================
        // サブステートから呼び出される公開メソッド (State Transitions)
        // ========================================================================

        /// <summary>
        /// Pullモード（直線移動）へ切り替えます。
        /// Swingステートから呼び出されます。
        /// </summary>
        public void SwitchToPull()
        {
            _subStateMachine.ChangeState(_pullState);
        }

        /// <summary>
        /// グラップルアクションを終了し、通常の移動ステートへ戻ります。
        /// 各サブステートから呼び出されます。
        /// </summary>
        public void FinishGrapple()
        {
            // 入力状況に応じて適切な移動ステートへ戻す
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