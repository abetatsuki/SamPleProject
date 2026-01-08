using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ActionSample.Input
{
    /// <summary>
    /// ゲーム全体の入力を定義・管理するクラス。
    /// Unity Input SystemのActionをプログラムで定義し、InputActionEntityでラップして提供します。
    /// </summary>
    public class GameInput : IDisposable
    {
        /// <summary>
        /// コンストラクタ。
        /// 入力アクションの定義と初期化を行います。
        /// </summary>
        public GameInput()
        {
            // アクションマップの作成
            // 入力アクションを論理的なグループ（プレイヤー操作など）にまとめるため
            _inputActionAsset = ScriptableObject.CreateInstance<InputActionAsset>();
            _playerMap = _inputActionAsset.AddActionMap("Player");

            // Moveアクションの定義 (WASD)
            // プレイヤーの移動操作（前後左右）を定義するため
            var moveAction = _playerMap.AddAction("Move", InputActionType.Value);
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
            
            Move = new InputActionEntity<Vector2>(moveAction);

            // Lookアクションの定義 (Mouse Delta)
            // マウス操作による視点移動を定義するため
            var lookAction = _playerMap.AddAction("Look", InputActionType.Value, "<Mouse>/delta");
            Look = new InputActionEntity<Vector2>(lookAction);

            // Fireアクションの定義 (Mouse Left)
            // 攻撃操作（左クリック）を定義するため
            var fireAction = _playerMap.AddAction("Fire", InputActionType.Button, "<Mouse>/leftButton");
            Fire = new InputActionEntity<float>(fireAction);

            // Aimアクションの定義 (Mouse Right)
            // エイム操作（右クリック）を定義するため
            var aimAction = _playerMap.AddAction("Aim", InputActionType.Button, "<Mouse>/rightButton");
            Aim = new InputActionEntity<float>(aimAction);

            // Reloadアクションの定義 (R Key)
            // リロード操作（Rキー）を定義するため
            var reloadAction = _playerMap.AddAction("Reload", InputActionType.Button, "<Keyboard>/r");
            Reload = new InputActionEntity<float>(reloadAction);

            // Grappleアクションの定義 (F Key)
            // グラップル操作（Fキー）を定義するため
            var grappleAction = _playerMap.AddAction("Grapple", InputActionType.Button, "<Keyboard>/f");
            Grapple = new InputActionEntity<float>(grappleAction);

            // Slideアクションの定義 (C Key)
            // 単発のスライディング（ダッシュ）操作を定義するため
            var slideAction = _playerMap.AddAction("Slide", InputActionType.Button, "<Keyboard>/c");
            Slide = new InputActionEntity<float>(slideAction);

            // Slidingアクションの定義 (Left Ctrl)
            // 継続的なスライディング（滑走）操作を定義するため
            var slidingAction = _playerMap.AddAction("Sliding", InputActionType.Button, "<Keyboard>/leftCtrl");
            Sliding = new InputActionEntity<float>(slidingAction);
        }

        /// <summary>移動入力 (WASD)。</summary>
        public InputActionEntity<Vector2> Move { get; private set; }

        /// <summary>視点操作入力 (Mouse Delta)。</summary>
        public InputActionEntity<Vector2> Look { get; private set; }

        /// <summary>発射入力 (Mouse Left)。</summary>
        public InputActionEntity<float> Fire { get; private set; }

        /// <summary>エイム入力 (Mouse Right)。</summary>
        public InputActionEntity<float> Aim { get; private set; }

        /// <summary>リロード入力 (R Key)。</summary>
        public InputActionEntity<float> Reload { get; private set; }

        /// <summary>グラップル入力 (F Key)。</summary>
        public InputActionEntity<float> Grapple { get; private set; }

        /// <summary>スライディング入力 (C Key)。</summary>
        public InputActionEntity<float> Slide { get; private set; }

        /// <summary>滑走入力 (Left Ctrl)。</summary>
        public InputActionEntity<float> Sliding { get; private set; }


        /// <summary>
        /// 全ての入力を有効化します。
        /// </summary>
        public void Enable()
        {
            // 定義した入力アクションを監視開始するため
            _playerMap.Enable();
            
            Move.Enable();
            Look.Enable();
            Fire.Enable();
            Aim.Enable();
            Reload.Enable();
            Grapple.Enable();
            Slide.Enable();
            Sliding.Enable();
        }

        /// <summary>
        /// 全ての入力を無効化します。
        /// </summary>
        public void Disable()
        {
            // 入力アクションの監視を停止するため
            _playerMap.Disable();

            Move.Disable();
            Look.Disable();
            Fire.Disable();
            Aim.Disable();
            Reload.Disable();
            Grapple.Disable();
            Slide.Disable();
            Sliding.Disable();
        }


        /// <summary>
        /// リソースを解放します。
        /// </summary>
        public void Dispose()
        {
            // 各Entityのイベント購読解除とAssetの破棄を行うため
            Move.Dispose();
            Look.Dispose();
            Fire.Dispose();
            Aim.Dispose();
            Reload.Dispose();
            Grapple.Dispose();
            Slide.Dispose();
            Sliding.Dispose();

            if (_inputActionAsset != null)
            {
                // ScriptableObjectなのでDestroyが必要だが、ランタイム生成の場合はnull参照解除で十分な場合もある。
                // Unity Objectの破棄はMain Threadで行う必要があるが、Disposeは非メインスレッドから呼ばれる可能性を考慮し、
                // ここでは明示的なDestroyは行わず参照を切る（GC任せにするか、Unityのライフサイクル管理に委ねる）。
                // ただし、Input Systemの作法としてDisableは呼ぶべき。
                _inputActionAsset = null;
            }
        }


        private InputActionAsset _inputActionAsset;
        private InputActionMap _playerMap;

    }
}
