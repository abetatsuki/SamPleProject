using UnityEngine;
using ActionSample.Input;

namespace ActionSample
{
    /// <summary>
    /// プレイヤーからの入力を管理するクラス。
    /// UnityのInputクラスから生の入力を取得し、ゲームロジックで使いやすい形式で提供します。
    /// </summary>
    public class PlayerInputHandler : MonoBehaviour
    {
        /// <summary>
        /// コンストラクタ。
        /// MonoBehaviourのため、初期化はAwakeなどで行います。
        /// </summary>
        public PlayerInputHandler() { }

        /// <summary>
        /// 移動入力ベクトル（正規化済み）。
        /// X: 横移動, Z: 前後移動
        /// </summary>
        public Vector3 MovementInput { get; private set; }

        /// <summary>
        /// エイム（照準）入力の状態。
        /// </summary>
        public bool AimInput { get; private set; }

        /// <summary>
        /// スライディング入力（Cキー）がトリガーされた瞬間の状態。
        /// </summary>
        public bool SlideTriggered { get; private set; }

        /// <summary>
        /// 視点操作入力ベクトル。
        /// X: マウスX, Y: マウスY
        /// </summary>
        public Vector2 LookInput { get; private set; }

        /// <summary>
        /// リロード入力の状態。
        /// </summary>
        public bool ReloadInput { get; private set; }

        /// <summary>
        /// 射撃入力の状態。
        /// </summary>
        public bool FireInput { get; private set; }

        /// <summary>
        /// グラップル入力が押されている間の状態。
        /// </summary>
        public bool GrappleInputHeld { get; private set; }

        /// <summary>
        /// グラップル入力がトリガーされた瞬間の状態。
        /// </summary>
        public bool GrappleInput { get; private set; }

        /// <summary>
        /// 滑走入力（左Ctrlキー）がトリガーされた瞬間の状態。
        /// </summary>
        public bool SlidingTriggered { get; private set; }

        /// <summary>
        /// 滑走入力（左Ctrlキー）が押されている間の状態。
        /// </summary>
        public bool SlidingInputHeld { get; private set; }


        private GameInput _gameInput;

        // トリガー入力の一時保存用フラグ
        private bool _slideTriggeredBuffer;
        private bool _slidingTriggeredBuffer;
        private bool _reloadInputBuffer;
        private bool _grappleInputBuffer;


        private void Awake()
        {
            // Input Systemの初期化
            // 新しいInput Systemのラッパークラスを生成し、イベントを購読するため
            _gameInput = new GameInput();

            // 移動入力
            _gameInput.Move.Performed += v =>
            {
                // Vector2(x, y) を Vector3(x, 0, z) に変換
                MovementInput = new Vector3(v.x, 0, v.y);
            };
            _gameInput.Move.Canceled += v => MovementInput = Vector3.zero;

            // 視点操作
            _gameInput.Look.Performed += v => LookInput = v;
            _gameInput.Look.Canceled += v => LookInput = Vector2.zero;

            // エイム (長押し)
            _gameInput.Aim.Started += _ => AimInput = true;
            _gameInput.Aim.Canceled += _ => AimInput = false;

            // 射撃 (長押し)
            _gameInput.Fire.Started += _ => FireInput = true;
            _gameInput.Fire.Canceled += _ => FireInput = false;

            // グラップル (長押し)
            _gameInput.Grapple.Started += _ => GrappleInputHeld = true;
            _gameInput.Grapple.Canceled += _ => GrappleInputHeld = false;

            // スライディング (Cキー: トリガー)
            _gameInput.Slide.Performed += _ => _slideTriggeredBuffer = true;

            // 滑走 (左Ctrlキー: トリガー & 長押し)
            _gameInput.Sliding.Performed += _ => _slidingTriggeredBuffer = true;
            _gameInput.Sliding.Started += _ => SlidingInputHeld = true;
            _gameInput.Sliding.Canceled += _ => SlidingInputHeld = false;

            // リロード (トリガー)
            _gameInput.Reload.Performed += _ => _reloadInputBuffer = true;

            // グラップル (トリガー)
            _gameInput.Grapple.Performed += _ => _grappleInputBuffer = true;
        }

        private void OnEnable()
        {
            // オブジェクト有効時にInput Systemも有効化するため
            _gameInput.Enable();
        }

        private void OnDisable()
        {
            // オブジェクト無効時にInput Systemも無効化するため
            _gameInput.Disable();
        }

        private void OnDestroy()
        {
            // リソースの解放を行うため
            _gameInput.Dispose();
        }

        private void Update()
        {
            // トリガー入力の適用とリセット
            // GetKeyDownのように1フレームだけtrueになる挙動を再現するため

            SlideTriggered = _slideTriggeredBuffer;
            _slideTriggeredBuffer = false;

            SlidingTriggered = _slidingTriggeredBuffer;
            _slidingTriggeredBuffer = false;

            ReloadInput = _reloadInputBuffer;
            _reloadInputBuffer = false;

            GrappleInput = _grappleInputBuffer;
            _grappleInputBuffer = false;
        }

    }
}