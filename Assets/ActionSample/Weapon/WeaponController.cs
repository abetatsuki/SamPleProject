using UnityEngine;
using ActionSample.Weapon.StateMachine;

namespace ActionSample
{
    /// <summary>
    /// 武器（銃）の制御を行うクラス。
    /// 発射、リロード、リコイル、ADS（覗き込み）などの機能を管理し、ステートマシンと連携して動作します。
    /// </summary>
    public class WeaponController : MonoBehaviour
    {
        [Header("References")]
        /// <summary>
        /// プレイヤーの入力ハンドラー。
        /// </summary>
        public PlayerInputHandler InputHandler;
        
        /// <summary>
        /// メインカメラへの参照（RaycastやFOV変更に使用）。
        /// </summary>
        public Camera MainCamera;
        
        /// <summary>
        /// 銃口のTransform（弾の生成位置）。
        /// </summary>
        public Transform Muzzle;
        
        /// <summary>
        /// 弾のプレハブ。
        /// </summary>
        public GameObject BulletPrefab;

        [Header("Weapon Stats")]
        /// <summary>
        /// マガジンの最大弾数。
        /// </summary>
        public int MaxAmmo = 30;
        
        /// <summary>
        /// 現在の弾数。
        /// </summary>
        public int CurrentAmmo;
        
        /// <summary>
        /// 所持している総弾数。
        /// </summary>
        public int TotalAmmo = 90;
        
        /// <summary>
        /// 連射速度（秒単位の間隔）。
        /// </summary>
        public float FireRate = 0.1f; 
        
        /// <summary>
        /// リロードにかかる時間（秒）。
        /// </summary>
        public float ReloadTime = 1.5f;
        
        /// <summary>
        /// 射程距離。
        /// </summary>
        public float Range = 100f;
        
        /// <summary>
        /// 1発あたりのダメージ。
        /// </summary>
        public float Damage = 10f;
        
        [Header("Recoil Settings")]
        /// <summary>
        /// 縦方向のリコイル量（カメラの跳ね上がり）。
        /// </summary>
        public float RecoilVertical = 2.0f;
        
        /// <summary>
        /// 横方向のリコイル量（左右のブレ範囲）。
        /// </summary>
        public float RecoilHorizontal = 0.25f;

        [Header("ADS Settings")]
        /// <summary>
        /// 通常時の視野角（FOV）。
        /// </summary>
        public float NormalFov = 60f;
        
        /// <summary>
        /// ADS（覗き込み）時の視野角。
        /// </summary>
        public float AdsFov = 40f;
        
        /// <summary>
        /// ADSへの移行速度。
        /// </summary>
        public float AdsSpeed = 10f;
        
        /// <summary>
        /// 腰だめ撃ち時の武器位置。
        /// </summary>
        public Vector3 HipPosition;
        
        /// <summary>
        /// ADS時の武器位置。
        /// </summary>
        public Vector3 AdsPosition;

        [Header("Visual Recoil Settings")]
        /// <summary>
        /// リコイルからの復帰速度（見た目）。
        /// </summary>
        public float VisualReturnSpeed = 10f;
        
        /// <summary>
        /// リコイル発生時の反応速度（スナップ感）。
        /// </summary>
        public float VisualSnappiness = 30f;
        
        /// <summary>
        /// 見た目上のリコイル力ベクトル。
        /// </summary>
        public Vector3 VisualRecoilForce = new Vector3(0, 0, -0.1f);
        
        /// <summary>
        /// 見た目上のリコイル最大値（制限）。
        /// </summary>
        public Vector3 VisualMaxRecoil = new Vector3(0.1f, 0.1f, 0.5f);

        /// <summary>
        /// 武器のステートマシン。
        /// </summary>
        public StateMachine.StateMachine StateMachine { get; private set; }
        
        /// <summary>
        /// 待機ステート。
        /// </summary>
        public WeaponIdleState IdleState { get; private set; }
        
        /// <summary>
        /// 発射ステート。
        /// </summary>
        public WeaponFireState FireState { get; private set; }
        
        /// <summary>
        /// リロードステート。
        /// </summary>
        public WeaponReloadState ReloadState { get; private set; }
        
        /// <summary>
        /// 親オブジェクトのプレイヤーコントローラー。
        /// </summary>
        public PlayerController PlayerController { get; private set; }

        /// <summary>
        /// リコイルを適用します。
        /// カメラの視点移動と武器モデルの揺れの両方を処理します。
        /// </summary>
        public void ApplyRecoil()
        {
            // カメラ視点へのリコイル適用
            if (PlayerController != null && PlayerController.RecoilController != null)
            {
                // 横方向はランダムに振ってブレを表現する
                // なぜこの処理が必要なのか: 毎回同じ方向だと不自然なため
                float horizontalRecoil = Random.Range(-RecoilHorizontal, RecoilHorizontal);
                PlayerController.RecoilController.AddRecoil(RecoilVertical, horizontalRecoil);
            }

            // 武器モデルへの見た目上のリコイル適用
            // なぜこの処理が必要なのか: 銃自体が手元で跳ねる動きを表現し、迫力を出すため
            _visualRecoil.PlayRecoil(VisualRecoilForce);
        }

        private GunVisualRecoilProcessor _visualRecoil;

        private void Awake()
        {
            // 初期弾数の設定
            CurrentAmmo = MaxAmmo;
            if (MainCamera == null) MainCamera = Camera.main;

            // プレイヤー参照の取得
            PlayerController = GetComponentInParent<PlayerController>();
            // インスペクターで未設定の場合、親から取得を試みる
            if (InputHandler == null && PlayerController != null)
            {
                InputHandler = PlayerController.InputHandler;
            }

            // ステートマシンの初期化
            // なぜこの処理が必要なのか: 射撃、リロード、待機などの状態遷移を管理するため
            StateMachine = new StateMachine.StateMachine();
            IdleState = new WeaponIdleState(this);
            FireState = new WeaponFireState(this);
            ReloadState = new WeaponReloadState(this);

            // 見た目上のリコイル計算クラスの初期化
            // なぜこの処理が必要なのか: リコイルの物理計算（カメラ）とは別に、銃モデルのアニメーション的な挙動（見た目）を計算するため
            _visualRecoil = new GunVisualRecoilProcessor(VisualReturnSpeed, VisualSnappiness, VisualMaxRecoil);

            // ADS設定の初期化
            // インスペクターで設定されていない場合、現在の位置を腰だめ位置とする
            if (HipPosition == Vector3.zero) HipPosition = transform.localPosition;
        }

        private void Start()
        {
            // 初期ステートの設定
            StateMachine.Initialize(IdleState);
        }

        private void Update()
        {
            // ビジュアルリコイルの更新（時間経過による減衰など）
            _visualRecoil.Tick(Time.deltaTime);

            // ステートマシンのロジック更新
            StateMachine.CurrentState.LogicUpdate();
            
            // ADS（覗き込み）処理の実行
            HandleADS();
        }

        // ---------------------------------------------------------
        // Private Methods
        // ---------------------------------------------------------

        /// <summary>
        /// ADS（Aim Down Sights：覗き込み）の挙動を制御します。
        /// </summary>
        private void HandleADS()
        {
            bool isAiming = InputHandler != null && InputHandler.AimInput;

            // 目標とするFOVと武器位置の決定
            float targetFov = isAiming ? AdsFov : NormalFov;
            Vector3 targetPos = isAiming ? AdsPosition : HipPosition;

            // カメラのFOVを滑らかに変更
            // なぜこの処理が必要なのか: 覗き込んだ際にズームする演出を行うため
            if (MainCamera != null)
            {
                MainCamera.fieldOfView = Mathf.Lerp(MainCamera.fieldOfView, targetFov, Time.deltaTime * AdsSpeed);
            }

            // 武器の位置を滑らかに変更し、リコイルオフセットを加算
            // なぜこの処理が必要なのか: 画面中央に銃を構える動きと、発砲時の跳ね上がり（VisualRecoil）を合成して反映するため
            // Lerpでスムーズに移動させつつ、CurrentOffset（リコイルによる瞬間的なズレ）を足し合わせている
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * AdsSpeed) + _visualRecoil.CurrentOffset;
        }
    }
}
