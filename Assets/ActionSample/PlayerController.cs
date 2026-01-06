using UnityEngine;
using ActionSample.StateMachine;

namespace ActionSample
{
    [RequireComponent(typeof(PlayerInputHandler))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerInputHandler))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float MoveSpeed = 5f;
        public float AimMoveSpeed = 2.5f;

        [Header("Slide Settings")]
        public float SlideSpeed = 15f;
        public float SlideDuration = 0.8f;
        public float SlideDrag = 0.5f;
        
        [Header("Aiming Settings")]
        public float MouseSensitivity = 2.0f;
        public float MaxLookAngle = 80f;
        
        [Header("Recoil Settings")]
        public float RecoilRecoverySpeed = 10f;
        
        [SerializeField]
        private Transform _mainCamera;

        // 公開プロパティ (Dependency Injection for States)
        public Rigidbody Rigidbody { get; private set; }
        public PlayerInputHandler InputHandler { get; private set; }
        public PlayerAiming Aiming { get; private set; }
        public RecoilController RecoilController { get; private set; }
        public StateMachine.StateMachine StateMachine { get; private set; }
        public bool IsSliding => StateMachine.CurrentState == SlideState;

        // ステートのインスタンス
        public PlayerIdleState IdleState { get; private set; }
        public PlayerWalkState WalkState { get; private set; }
        public PlayerSlideState SlideState { get; private set; }

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            InputHandler = GetComponent<PlayerInputHandler>();
            
            if (_mainCamera == null) _mainCamera = Camera.main.transform;

            // Pureクラスの初期化
            RecoilController = new RecoilController();
            RecoilController.RecoilRecoverySpeed = RecoilRecoverySpeed;
            
            Aiming = new PlayerAiming(transform, _mainCamera, RecoilController);
            Aiming.MouseSensitivity = MouseSensitivity;
            Aiming.MaxLookAngle = MaxLookAngle;

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
            // 時間経過(Recoil)
            RecoilController.Update(Time.deltaTime);
            
            // 現在のステートのロジック更新
            StateMachine.CurrentState.LogicUpdate();

            // 照準(視点)はステートに関わらず更新
            Aiming.UpdateLook(InputHandler.LookInput);
        }

        private void FixedUpdate()
        {
            // 現在のステートの物理更新
            StateMachine.CurrentState.PhysicsUpdate();
        }

        private void OnValidate()
        {
             // インスペクターでの値変更をランタイム反映させるための簡易対応
             if (RecoilController != null)
             {
                 RecoilController.RecoilRecoverySpeed = RecoilRecoverySpeed;
             }
             if (Aiming != null)
             {
                 Aiming.MouseSensitivity = MouseSensitivity;
                 Aiming.MaxLookAngle = MaxLookAngle;
             }
        }
    }
}