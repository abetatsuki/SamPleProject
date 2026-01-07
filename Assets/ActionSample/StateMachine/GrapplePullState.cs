using UnityEngine;

namespace ActionSample.StateMachine
{
    /// <summary>
    /// グラップルの「プル（直線移動）」挙動を担当するサブステート。
    /// 物理演算を無視し、ターゲット地点への高速な直線移動を行います。
    /// </summary>
    public class GrapplePullState : PlayerState
    {
        private PlayerGrappleState _parentState;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">プレイヤーコントローラー</param>
        /// <param name="parentState">親となるグラップルステート</param>
        public GrapplePullState(PlayerController context, PlayerGrappleState parentState) : base(context)
        {
            _parentState = parentState;
        }

        public override void Enter()
        {
            base.Enter();
            
            // Pull開始：物理ジョイントを破棄し、直線移動を開始
            Context.GrappleController.ExecutePull();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // 到達判定
            // なぜこの処理が必要なのか: 目標地点に着いたらアクションを終了させるため
            if (Context.GrappleController.IsAtGrapplePoint())
            {
                Debug.Log("Grapple: Reached Target (Pull Finished)");
                
                // 到達時の挙動：少し跳ねさせる（着地しやすくするため）
                Context.Rigidbody.linearVelocity = Vector3.up * 5f;
                
                _parentState.FinishGrapple();
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            
            // 移動処理の継続
            // なぜこの処理が必要なのか: Rigidbodyの速度を毎フレーム更新し続け、外部要因（重力など）による減速を防いでターゲットへ確実に届けるため
            Context.GrappleController.ExecutePull();
        }
    }
}
