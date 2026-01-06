using UnityEngine;
using ActionSample.Weapon.StateMachine;

namespace ActionSample
{
    public class WeaponController : MonoBehaviour
    {
        [Header("References")]
        public PlayerInputHandler InputHandler;
        public Camera MainCamera;
        public Transform Muzzle;
        public GameObject BulletPrefab;

        [Header("Weapon Stats")]
        public int MaxAmmo = 30;
        public int CurrentAmmo;
        public int TotalAmmo = 90;
        public float FireRate = 0.1f; // Seconds between shots
        public float ReloadTime = 1.5f;
        public float Range = 100f;
        public float Damage = 10f;
        
        [Header("Recoil Settings")]
        public float RecoilVertical = 2.0f;
        public float RecoilHorizontal = 0.25f;

        [Header("ADS Settings")]
        public float NormalFov = 60f;
        public float AdsFov = 40f;
        public float AdsSpeed = 10f;
        public Vector3 HipPosition;
        public Vector3 AdsPosition;

        [Header("Visual Recoil Settings")]
        public float VisualReturnSpeed = 10f;
        public float VisualSnappiness = 30f;
        public Vector3 VisualRecoilForce = new Vector3(0, 0, -0.1f);

        // State Machine
        public StateMachine.StateMachine StateMachine { get; private set; }
        public WeaponIdleState IdleState { get; private set; }
        public WeaponFireState FireState { get; private set; }
        public WeaponReloadState ReloadState { get; private set; }
        
        // Dependencies
        public PlayerController PlayerController { get; private set; }

        private GunVisualRecoilProcessor _visualRecoil;

        private void Awake()
        {
            CurrentAmmo = MaxAmmo;
            if (MainCamera == null) MainCamera = Camera.main;

            // PlayerControllerを取得
            // 武器がPlayerの子階層にあると想定
            PlayerController = GetComponentInParent<PlayerController>();

            // Initialize State Machine
            StateMachine = new StateMachine.StateMachine();
            IdleState = new WeaponIdleState(this);
            FireState = new WeaponFireState(this);
            ReloadState = new WeaponReloadState(this);

            // 見た目上のリコイル計算クラスの初期化
            // なぜこの処理が必要か: Pure Class として定義した計算ロジックをインスタンス化し、
            // 武器ごとの設定値（戻り速度、キビキビ具合）を適用するため。
            _visualRecoil = new GunVisualRecoilProcessor(VisualReturnSpeed, VisualSnappiness);

            // Set initial position
            if (HipPosition == Vector3.zero) HipPosition = transform.localPosition;
        }

        public void ApplyRecoil()
        {
            if (PlayerController != null && PlayerController.RecoilController != null)
            {
                // 横ブレはランダムにする
                float horizontalRecoil = Random.Range(-RecoilHorizontal, RecoilHorizontal);
                PlayerController.RecoilController.AddRecoil(RecoilVertical, horizontalRecoil);
            }

            // 見た目上の反動を発生させる
            // なぜこの処理が必要か: 銃が発射された瞬間にモデルを後ろに跳ねさせる演出を加えるため。
            _visualRecoil.PlayRecoil(VisualRecoilForce);
        }

        private void Start()
        {
            StateMachine.Initialize(IdleState);
        }

        private void Update()
        {
            // 見た目上のリコイル状態を更新
            // なぜこの処理が必要か: 前フレームからの経過時間に基づいて、跳ねた銃を徐々に元の位置に戻す計算を行うため。
            _visualRecoil.Tick(Time.deltaTime);

            StateMachine.CurrentState.LogicUpdate();
            HandleADS();
        }

        private void HandleADS()
        {
            bool isAiming = InputHandler.AimInput;

            // Target Values
            float targetFov = isAiming ? AdsFov : NormalFov;
            Vector3 targetPos = isAiming ? AdsPosition : HipPosition;

            // Smooth Transition
            if (MainCamera != null)
            {
                MainCamera.fieldOfView = Mathf.Lerp(MainCamera.fieldOfView, targetFov, Time.deltaTime * AdsSpeed);
            }

            // ADSによる位置補間と、リコイルによるオフセットを組み合わせて適用
            // なぜこの処理が必要か: ADS（覗き込み）の移動計算とは別に、射撃時の微細な反動アニメーションを
            // 加算合成することで、より動的で自然な銃の挙動を表現するため。
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * AdsSpeed) + _visualRecoil.CurrentOffset;
        }
    }
}
