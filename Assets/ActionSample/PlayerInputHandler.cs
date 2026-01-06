using UnityEngine;

namespace ActionSample
{
    /// <summary>
    /// プレイヤーからの入力を管理するクラス。
    /// UnityのInputクラスから生の入力を取得し、ゲームロジックで使いやすい形式で提供します。
    /// </summary>
    public class PlayerInputHandler : MonoBehaviour
    {
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
        /// スライディング入力がトリガーされた瞬間の状態。
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
        public bool ReloadInput {get; private set; }

        /// <summary>
        /// 射撃入力の状態。
        /// </summary>
        public bool FireInput { get; private set; }

        private const string HorizontalAxis = "Horizontal";
        private const string VerticalAxis = "Vertical";
        private const string MouseXAxis = "Mouse X";
        private const string MouseYAxis = "Mouse Y";

        private void Update()
        {
            // 移動入力の取得
            // なぜこの処理が必要なのか: WASDキーやスティック入力による移動量を取得し、斜め移動の速度超過を防ぐために正規化するため
            float x = Input.GetAxisRaw(HorizontalAxis);
            float z = Input.GetAxisRaw(VerticalAxis);
            MovementInput = new Vector3(x, 0, z).normalized;
            
            // 視点移動入力の取得
            // なぜこの処理が必要なのか: マウスの移動量を取得して、カメラやプレイヤーの回転に反映させるため
            LookInput = new Vector2(Input.GetAxis(MouseXAxis), Input.GetAxis(MouseYAxis));

            // エイム入力の判定
            // なぜこの処理が必要なのか: 右クリック（または対応するボタン）が押されている間、精密射撃モードに切り替えるため
            AimInput = Input.GetMouseButton(1);
            
            // リロード入力の判定
            // なぜこの処理が必要なのか: Rキーが押された瞬間にリロードアクションを実行するため
            ReloadInput = Input.GetKeyDown(KeyCode.R);
            
            // 射撃入力の判定
            // なぜこの処理が必要なのか: 左クリックが押されている間、連続して射撃を行うため（オート連射などを想定）
            FireInput = Input.GetMouseButton(0);
            
            // スライディング入力の判定
            // なぜこの処理が必要なのか: 左Controlキーが押された瞬間に、スライディングアクションをトリガーするため
            SlideTriggered = Input.GetKeyDown(KeyCode.LeftControl);
        }
    }
}
