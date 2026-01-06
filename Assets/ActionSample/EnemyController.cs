using UnityEngine;
using UnityEngine.AI;
using ActionSample.StateMachine;

namespace ActionSample
{
    /// <summary>
    /// 敵キャラクターの制御クラス。
    /// ナビゲーションメッシュを使用した移動、ステートマシンによる状態遷移、およびプレイヤーとの衝突判定を管理します。
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyController : MonoBehaviour
    {
        /// <summary>
        /// 気絶状態の持続時間（秒）。
        /// </summary>
        [Header("Settings")]
        public float StunDuration = 2.0f;

        /// <summary>
        /// 移動速度。
        /// </summary>
        public float MoveSpeed = 3.0f;

        /// <summary>
        /// 巡回ポイントの配列。
        /// </summary>
        public Transform[] Waypoints;

        /// <summary>
        /// NavMeshAgentコンポーネントへの参照。
        /// </summary>
        public NavMeshAgent NavAgent { get; private set; }

        /// <summary>
        /// MeshRendererコンポーネントへの参照。
        /// </summary>
        public MeshRenderer MeshRenderer { get; private set; }

        /// <summary>
        /// 元のマテリアルカラー（点滅演出などで元に戻すために使用）。
        /// </summary>
        public Color OriginalColor { get; private set; }

        // State Machine
        /// <summary>
        /// ステートマシン本体。
        /// </summary>
        public StateMachine.StateMachine StateMachine { get; private set; }

        /// <summary>
        /// 待機ステート。
        /// </summary>
        public EnemyIdleState IdleState { get; private set; }

        /// <summary>
        /// 気絶ステート。
        /// </summary>
        public EnemyStunState StunState { get; private set; }

        /// <summary>
        /// 巡回ステート。
        /// </summary>
        public EnemyPatrolState PatrolState { get; private set; }

        private void Awake()
        {
            // NavMeshAgentの取得と設定
            // なぜこの処理が必要なのか: AIによる自動経路探索と移動を行うため
            NavAgent = GetComponent<NavMeshAgent>();
            
            // 設定した移動速度をNavMeshAgentに反映
            // なぜこの処理が必要なのか: インスペクターでの設定値と実際の移動速度を同期させるため
            NavAgent.speed = MoveSpeed;

            // MeshRendererの取得
            // なぜこの処理が必要なのか: 被弾時や状態変化時に色を変える演出を行うため
            MeshRenderer = GetComponent<MeshRenderer>();
            if (MeshRenderer != null)
            {
                OriginalColor = MeshRenderer.material.color;
            }

            // ステートマシンの初期化
            // なぜこの処理が必要なのか: 敵の複雑な振る舞い（巡回、待機、気絶など）を状態ごとに管理するため
            StateMachine = new StateMachine.StateMachine();
            IdleState = new EnemyIdleState(this);
            StunState = new EnemyStunState(this);
            PatrolState = new EnemyPatrolState(this);
        }

        private void Start()
        {
            // 初期ステートの決定
            // なぜこの処理が必要なのか: ウェイポイントが設定されている場合は巡回を、そうでない場合は待機を開始させるため
            if (Waypoints != null && Waypoints.Length > 0)
            {
                StateMachine.Initialize(PatrolState);
            }
            else
            {
                StateMachine.Initialize(IdleState);
            }
        }

        private void Update()
        {
            // ステートごとのロジック更新
            // なぜこの処理が必要なのか: 現在の状態に応じた行動（プレイヤー追跡、待機時間計測など）を実行するため
            StateMachine.CurrentState.LogicUpdate();
        }

        private void FixedUpdate()
        {
            // ステートごとの物理更新
            // なぜこの処理が必要なのか: 物理演算が必要な処理を安定したフレームレートで実行するため
            StateMachine.CurrentState.PhysicsUpdate();
        }

        private void OnCollisionEnter(Collision collision)
        {
            // プレイヤー以外の衝突は無視
            // なぜこの処理が必要なのか: 不要な衝突判定処理をスキップして負荷を下げるため
            if (!collision.gameObject.CompareTag(GameConstants.PlayerTag)) return;
            
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            
            // スライディング中のプレイヤーとの衝突判定
            // なぜこの処理が必要なのか: スライディング攻撃を受けた場合に敵を気絶状態へ遷移させるため
            if (player != null && player.IsSliding)
            {
                StateMachine.ChangeState(StunState);
            }
        }
    }
}
