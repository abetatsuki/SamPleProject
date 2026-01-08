using UnityEngine;

namespace ActionSample.StateMachine
{
    /// <summary>
    /// 敵の気絶状態を表すステート。
    /// 一定時間行動不能にし、見た目（色）を変え、時間が経過したら元の状態に戻します。
    /// </summary>
    public class EnemyStunState : EnemyState
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">敵コントローラー</param>
        public EnemyStunState(EnemyController context) : base(context) { }

        /// <summary>
        /// ステート開始時の処理。
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            
            // 気絶時間を設定
            _timer = Context.StunDuration;
            
            // エージェントの移動を完全に停止
            // 気絶中は一切動けないようにするため。速度もゼロにする。
            if (Context.NavAgent != null && Context.NavAgent.isActiveAndEnabled)
            {
                Context.NavAgent.isStopped = true;
                Context.NavAgent.velocity = Vector3.zero;
            }

            // 見た目の変化（青色にする）
            // プレイヤーに気絶中であることを視覚的に伝えるため
            if (Context.MeshRenderer != null)
            {
                Context.MeshRenderer.material.color = Color.blue;
            }
            
            // デバッグ用ログ（開発確認用）
            Debug.Log("Enemy Entered Stun State");
        }

        /// <summary>
        /// ステート終了時の処理。
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            
            // 色を元に戻す
            // 気絶状態が終わったことを視覚的に反映するため
            if (Context.MeshRenderer != null)
            {
                Context.MeshRenderer.material.color = Context.OriginalColor;
            }
            
            // デバッグ用ログ（開発確認用）
            Debug.Log("Enemy Exited Stun State");
        }

        /// <summary>
        /// フレーム毎のロジック更新。
        /// </summary>
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            // タイマー減算
            _timer -= Time.deltaTime;

            // 気絶時間の終了判定
            // 一定時間経過後に敵を行動可能な状態（巡回または待機）に復帰させるため
            if (_timer <= 0f)
            {
                if (Context.Waypoints != null && Context.Waypoints.Length > 0)
                {
                    Context.StateMachine.ChangeState(Context.PatrolState);
                }
                else
                {
                    Context.StateMachine.ChangeState(Context.IdleState);
                }
            }
        }

        private float _timer;
    }
}
