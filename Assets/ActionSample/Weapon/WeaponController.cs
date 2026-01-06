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
        public Vector3 VisualMaxRecoil = new Vector3(0.1f, 0.1f, 0.5f);

        public StateMachine.StateMachine StateMachine { get; private set; }
        public WeaponIdleState IdleState { get; private set; }
        public WeaponFireState FireState { get; private set; }
        public WeaponReloadState ReloadState { get; private set; }
        
        public PlayerController PlayerController { get; private set; }

        private GunVisualRecoilProcessor _visualRecoil;

        private void Awake()
        {
            CurrentAmmo = MaxAmmo;
            if (MainCamera == null) MainCamera = Camera.main;

            PlayerController = GetComponentInParent<PlayerController>();

            StateMachine = new StateMachine.StateMachine();
            IdleState = new WeaponIdleState(this);
            FireState = new WeaponFireState(this);
            ReloadState = new WeaponReloadState(this);

            // 見た目上のリコイル計算クラスの初期化
            // なぜこの処理が必要か: リコイルの最大値を制限パラメータとして追加し、
            // 連続射撃時に銃が画面外へ突き抜けるのを防ぐため。
            _visualRecoil = new GunVisualRecoilProcessor(VisualReturnSpeed, VisualSnappiness, VisualMaxRecoil);

            if (HipPosition == Vector3.zero) HipPosition = transform.localPosition;
        }

        public void ApplyRecoil()
        {
            if (PlayerController != null && PlayerController.RecoilController != null)
            {
                float horizontalRecoil = Random.Range(-RecoilHorizontal, RecoilHorizontal);
                PlayerController.RecoilController.AddRecoil(RecoilVertical, horizontalRecoil);
            }

            _visualRecoil.PlayRecoil(VisualRecoilForce);
        }

        private void Start()
        {
            StateMachine.Initialize(IdleState);
        }

        private void Update()
        {
            _visualRecoil.Tick(Time.deltaTime);

            StateMachine.CurrentState.LogicUpdate();
            HandleADS();
        }

        private void HandleADS()
        {
            bool isAiming = InputHandler.AimInput;

            float targetFov = isAiming ? AdsFov : NormalFov;
            Vector3 targetPos = isAiming ? AdsPosition : HipPosition;

            if (MainCamera != null)
            {
                MainCamera.fieldOfView = Mathf.Lerp(MainCamera.fieldOfView, targetFov, Time.deltaTime * AdsSpeed);
            }

            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * AdsSpeed) + _visualRecoil.CurrentOffset;
        }
    }
}
