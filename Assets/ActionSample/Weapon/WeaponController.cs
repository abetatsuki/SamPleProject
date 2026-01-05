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

        [Header("ADS Settings")]
        public float NormalFov = 60f;
        public float AdsFov = 40f;
        public float AdsSpeed = 10f;
        public Vector3 HipPosition;
        public Vector3 AdsPosition;

        // State Machine
        public StateMachine.StateMachine StateMachine { get; private set; }
        public WeaponIdleState IdleState { get; private set; }
        public WeaponFireState FireState { get; private set; }
        public WeaponReloadState ReloadState { get; private set; }

        private void Awake()
        {
            CurrentAmmo = MaxAmmo;
            if (MainCamera == null) MainCamera = Camera.main;

            // Initialize State Machine
            StateMachine = new StateMachine.StateMachine();
            IdleState = new WeaponIdleState(this);
            FireState = new WeaponFireState(this);
            ReloadState = new WeaponReloadState(this);

            // Set initial position
            if (HipPosition == Vector3.zero) HipPosition = transform.localPosition;
        }

        private void Start()
        {
            StateMachine.Initialize(IdleState);
        }

        private void Update()
        {
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
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * AdsSpeed);
        }
    }
}
