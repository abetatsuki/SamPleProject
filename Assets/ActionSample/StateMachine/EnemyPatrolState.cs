using UnityEngine;

namespace ActionSample.StateMachine
{
    /// <summary>
    /// 敵の巡回状態を表すステート。
    /// 設定されたウェイポイント間を順番に移動し、各ポイントで一定時間待機します。
    /// </summary>
    public class EnemyPatrolState : EnemyState
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">敵コントローラー</param>
        public EnemyPatrolState(EnemyController context) : base(context) { }

        /// <summary>
        /// ステート開始時の処理。
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            
            // エージェントの移動を再開
            // 他のステート（気絶など）で停止させられている可能性があるため
            Context.NavAgent.isStopped = false;
            
            // ウェイポイントの有無を確認
            if (Context.Waypoints == null || Context.Waypoints.Length == 0)
            {
                // ウェイポイントがない場合は待機ステートへ戻る
                // 移動先が存在しないのに巡回しようとしてエラーになるのを防ぐため
                Context.StateMachine.ChangeState(Context.IdleState);
            }
            else
            {
                // 最初のウェイポイントへ移動開始
                SetDestinationToCurrentWaypoint();
            }
        }

        /// <summary>
        /// ステート終了時の処理。
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            
            // 巡回中断時に移動を停止
            // ステートが変わったのに移動し続けるのを防ぐため（例：気絶時）
            if (Context.NavAgent.enabled)
            {
                Context.NavAgent.isStopped = true;
            }
        }

        /// <summary>
        /// フレーム毎のロジック更新。
        /// </summary>
        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // 待機中の処理
            if (_isWaiting)
            {
                _waitTimer -= Time.deltaTime;
                if (_waitTimer <= 0f)
                {
                    // 待機終了、次の目的地へ
                    // 一定時間の待機が終わったら、巡回を再開するため
                    _isWaiting = false;
                    NextWaypoint();
                }
                return;
            }

            // 目的地への到着判定
            // pathPending: パス計算中は判定しない
            // remainingDistance: 残り距離が停止距離以下になったら到着とみなす
            if (!Context.NavAgent.pathPending && Context.NavAgent.remainingDistance <= Context.NavAgent.stoppingDistance)
            {
                // パスを持っていない、または速度がゼロに近い場合も考慮
                if (!Context.NavAgent.hasPath || Context.NavAgent.velocity.sqrMagnitude == 0f)
                {
                    // 到着したら待機を開始
                    // ただ通り過ぎるのではなく、各ポイントで周囲を見渡すような挙動を作るため
                    StartWait(1.0f); // 1秒間待機
                }
            }
        }

        private int _currentWaypointIndex = 0;
        private float _waitTimer;
        private bool _isWaiting;

        /// <summary>
        /// 現在のインデックスに対応するウェイポイントを目的地に設定します。
        /// </summary>
        private void SetDestinationToCurrentWaypoint()
        {
            if (Context.Waypoints.Length == 0) return;
            Context.NavAgent.SetDestination(Context.Waypoints[_currentWaypointIndex].position);
        }

        /// <summary>
        /// 待機を開始します。
        /// </summary>
        /// <param name="duration">待機時間（秒）</param>
        private void StartWait(float duration)
        {
            _isWaiting = true;
            _waitTimer = duration;
        }

        /// <summary>
        /// 次のウェイポイントへインデックスを進め、移動を開始します。
        /// </summary>
        private void NextWaypoint()
        {
            // インデックスを更新（配列の最後まで行ったら0に戻るループ）
            // 巡回ルートを周回させ続けるため
            _currentWaypointIndex = (_currentWaypointIndex + 1) % Context.Waypoints.Length;
            SetDestinationToCurrentWaypoint();
        }
    }
}
