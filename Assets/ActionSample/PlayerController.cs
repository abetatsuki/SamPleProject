using UnityEngine;
using ActionSample.StateMachine;
using Unity.VisualScripting;

namespace ActionSample
{
    /// <summary>
    /// プレイヤーキャラクターの総合的な制御を行うクラス。
    /// 移動、視点操作、ステートマシンの管理を担当します。
    /// </summary>
    [RequireComponent(typeof(PlayerInputHandler))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour,IInitializable
    {
        /// <summary>
        /// 通常時の移動速度
        /// </summary>
        [Header("Movement Settings")]
        public float MoveSpeed = 5f;

        /// <summary>
        /// エイム（構え）時の移動速度
        /// </summary>
        public float AimMoveSpeed = 2.5f;

        /// <summary>
        /// スライディング時の速度（Cキー: Slide）
        /// </summary>
        [Header("Slide Settings (C Key)")]
        public float SlideSpeed = 15f;

        /// <summary>
        /// スライディングの継続時間（秒）
        /// </summary>
        public float SlideDuration = 0.8f;

        /// <summary>
        /// スライディング中の空気抵抗（減速）
        /// </summary>
        public float SlideDrag = 0.5f;

        /// <summary>
        /// スライディング時のY軸スケール（しゃがみ効果）
        /// </summary>
        public float SlideYScale = 0.5f;

        /// <summary>
        /// 滑走時の速度（Left Ctrl: Sliding）
        /// </summary>
        [Header("Sliding Settings (Left Ctrl)")]
        public float SlidingSpeed = 15f;

        /// <summary>
        /// 平地での滑走継続時間（秒）
        /// </summary>
        public float SlidingDuration = 1.0f;

        /// <summary>
        /// 滑走中の空気抵抗
        /// </summary>
        public float SlidingDrag = 0.1f;

        /// <summary>
        /// 滑走時のY軸スケール
        /// </summary>
        public float SlidingYScale = 0.5f;

        public float GravityScale = 10f;

        /// <summary>
        /// マウス感度
        /// </summary>
        [Header("Aiming Settings")]
        public float MouseSensitivity = 2.0f;

        /// <summary>
        /// 上下の最大視点角度
        /// </summary>
        public float MaxLookAngle = 80f;
        
        /// <summary>
        /// リコイル（反動）からの復帰速度
        /// </summary>
        [Header("Recoil Settings")]
        public float RecoilRecoverySpeed = 10f;

        [Header("Slope Settings")]
        public float MaxSlopeAngle = 40f;
        public float PlayerHeight = 2f;

        /// <summary>
        /// 物理挙動コンポーネントへの参照
        /// </summary>
        public Rigidbody Rigidbody { get; private set; }

        /// <summary>
        /// 入力ハンドラーへの参照
        /// </summary>
        public PlayerInputHandler InputHandler { get; private set; }

        /// <summary>
        /// 視点操作制御クラスへの参照
        /// </summary>
        public PlayerAiming Aiming { get; private set; }

        /// <summary>
        /// リコイル制御クラスへの参照
        /// </summary>
        public RecoilController RecoilController { get; private set; }

        /// <summary>
        /// グラップル制御コンポーネントへの参照
        /// </summary>
        public GrappleController GrappleController { get; private set; }

        /// <summary>
        /// ステートマシンへの参照
        /// </summary>
        public StateMachine.StateMachine StateMachine { get; private set; }

        /// <summary>
        /// 現在スライディング中かどうかを判定するプロパティ
        /// </summary>
        public bool IsSliding => StateMachine.CurrentState == SlideState;

        /// <summary>
        /// 待機ステート
        /// </summary>
        public PlayerIdleState IdleState { get; private set; }

        /// <summary>
        /// 歩行ステート
        /// </summary>
        public PlayerWalkState WalkState { get; private set; }

        /// <summary>
        /// スライディングステート（短距離）
        /// </summary>
        public PlayerSlideState SlideState { get; private set; }

        /// <summary>
        /// スライディングステート（斜面滑走）
        /// </summary>
        public PlayerSlidingState SlidingState { get; private set; }

        /// <summary>
        /// グラップルステート
        /// </summary>
        public PlayerGrappleState GrappleState { get; private set; }

        /// <summary>
        /// 初期化処理。
        ///</summary>
        public void Initialize()
        {
            GetComponents();
            if (_mainCamera == null) _mainCamera = Camera.main.transform;

            RecoilController = new RecoilController();
            RecoilController.RecoilRecoverySpeed = RecoilRecoverySpeed;

            Aiming = new PlayerAiming(transform, _mainCamera, RecoilController);
            Aiming.MouseSensitivity = MouseSensitivity;
            Aiming.MaxLookAngle = MaxLookAngle;

            StateMachine = new StateMachine.StateMachine();
            IdleState = new PlayerIdleState(this);
            WalkState = new PlayerWalkState(this);
            SlideState = new PlayerSlideState(this);
            SlidingState = new PlayerSlidingState(this);
            GrappleState = new PlayerGrappleState(this);

            StateMachine.Initialize(IdleState);
        }

        /// <summary>
        /// 斜面上にいるかどうかを判定します。
        /// </summary>
        /// <returns>斜面上ならtrue</returns>
        public bool OnSlope()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, PlayerHeight * 0.5f + 0.3f))
            {
                float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
                return angle < MaxSlopeAngle && angle != 0;
            }

            return false;
        }

        /// <summary>
        /// 斜面に沿った移動方向を取得します。
        /// </summary>
        /// <param name="direction">元の移動方向</param>
        /// <returns>斜面に沿ったベクトル</returns>
        public Vector3 GetSlopeMoveDirection(Vector3 direction)
        {
            return Vector3.ProjectOnPlane(direction, _slopeHit.normal).normalized;
        }

        [SerializeField]
        private Transform _mainCamera;
        private RaycastHit _slopeHit;

        private void Awake()
        {
           Initialize();
        }

        private void Start()
        {

        }

        private void Update()
        {
            // リコイルの時間経過による減衰処理
            // なぜこの処理が必要なのか: 射撃による反動を時間経過とともに元の位置に戻すため
            RecoilController.Update(Time.deltaTime);
            
            // 現在のステートのロジック更新を実行
            // なぜこの処理が必要なのか: ステートごとの固有の振る舞い（入力処理や状態遷移判定など）を実行するため
            StateMachine.CurrentState.LogicUpdate();

            // 視点の更新処理
            // なぜこの処理が必要なのか: ステートに関わらず視点操作は常に行えるようにするため
            Aiming.UpdateLook(InputHandler.LookInput);
        }

        private void FixedUpdate()
        {
            // 現在のステートの物理更新を実行
            // なぜこの処理が必要なのか: 移動などの物理演算を伴う処理をFixedUpdateのタイミングで安定して行うため
            StateMachine.CurrentState.PhysicsUpdate();
        }

        private void OnCollisionEnter(Collision collision)
        {
            // グラップル中に何かにぶつかったらステートを解除する
            // なぜこの処理が必要なのか: 元のコードの挙動を再現し、壁にぶつかった際に即座に制御を戻すため
            if (StateMachine.CurrentState == GrappleState)
            {
               // GrappleState.OnCollision();
            }
        }

        private void GetComponents()
        {
            Rigidbody = GetComponent<Rigidbody>();
            InputHandler = GetComponent<PlayerInputHandler>();
            GrappleController = GetComponent<GrappleController>();
        }
        private void OnValidate()
        {
             // インスペクターでの値変更をランタイム反映させるための簡易対応
             // なぜこの処理が必要なのか: プレイモード中にパラメータを調整した際、即座に反映させて調整効率を上げるため
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