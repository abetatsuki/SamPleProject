using UnityEngine;
using ActionSample.StateMachine;

namespace ActionSample
{
    [RequireComponent(typeof(PlayerInputHandler))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerAiming))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float MoveSpeed = 5f;

        [Header("Slide Settings")]
        public float SlideSpeed = 15f;
        public float SlideDuration = 0.8f;
        public float SlideDrag = 0.5f;

        // 公開プロパティ (Dependency Injection for States)
        public Rigidbody Rigidbody { get; private set; }
        public PlayerInputHandler InputHandler { get; private set; }
        public PlayerAiming Aiming { get; private set; }
        public StateMachine.StateMachine StateMachine { get; private set; }

        // ステートのインスタンス
        public PlayerIdleState IdleState { get; private set; }
        public PlayerWalkState WalkState { get; private set; }
        public PlayerSlideState SlideState { get; private set; }

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            InputHandler = GetComponent<PlayerInputHandler>();
            Aiming = GetComponent<PlayerAiming>();

            // ステートマシンの初期化
            StateMachine = new StateMachine.StateMachine();
            IdleState = new PlayerIdleState(this);
            WalkState = new PlayerWalkState(this);
            SlideState = new PlayerSlideState(this);
        }

        private void Start()
        {
            StateMachine.Initialize(IdleState);
        }

        private void Update()
        {
            // 現在のステートのロジック更新
            StateMachine.CurrentState.LogicUpdate();

            // 照準はステートに関わらず更新（必要ならステート内に移動も可）
            Aiming.UpdateAiming(InputHandler.MousePosition);
        }

        private void FixedUpdate()
        {
            // 現在のステートの物理更新
            StateMachine.CurrentState.PhysicsUpdate();
        }
    }
}