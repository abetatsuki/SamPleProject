using UnityEngine;

namespace ActionSample
{
    /// <summary>
    /// プレイヤーの視点操作（Aiming）を制御するクラス
    /// </summary>
    public class PlayerAiming
    {
        public float MouseSensitivity { get; set; } = 2.0f;
        public float MaxLookAngle { get; set; } = 80f;

        private Transform _playerBody;
        private Transform _mainCamera;
        private RecoilController _recoilController;

        private float _currentPitch = 0f;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="playerBody">プレイヤーのTransform（水平回転用）</param>
        /// <param name="mainCamera">カメラのTransform（垂直回転用）</param>
        /// <param name="recoilController">リコイルコントローラー</param>
        public PlayerAiming(Transform playerBody, Transform mainCamera, RecoilController recoilController)
        {
            _playerBody = playerBody;
            _mainCamera = mainCamera;
            _recoilController = recoilController;

            Initialize();
        }

        private void Initialize()
        {
            // FPS視点ではカーソルをロックするのが一般的
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        /// <summary>
        /// 視点を更新します。
        /// </summary>
        /// <param name="lookInput">入力ベクトル</param>
        public void UpdateLook(Vector2 lookInput)
        {
            if (_mainCamera == null || _playerBody == null) return;

            // 水平回転 (Player Body)
            // リコイルのYaw成分も考慮
            float yaw = lookInput.x * MouseSensitivity;
            _playerBody.Rotate(0, yaw + _recoilController.CurrentRecoilYaw, 0);

            // 垂直回転 (Camera)
            float pitchDelta = -lookInput.y * MouseSensitivity;
            _currentPitch += pitchDelta;
            _currentPitch = Mathf.Clamp(_currentPitch, -MaxLookAngle, MaxLookAngle);

            // リコイル適用後の最終的なPitch
            float finalPitch = _currentPitch - _recoilController.CurrentRecoilPitch; // Recoilは銃が跳ね上がるのでマイナス（上向き）

            _mainCamera.localEulerAngles = new Vector3(finalPitch, 0, 0);
        }

        /// <summary>
        /// リコイルを追加します（RecoilControllerへ委譲）。
        /// </summary>
        /// <param name="vertical">縦方向</param>
        /// <param name="horizontal">横方向</param>
        public void AddRecoil(float vertical, float horizontal)
        {
            _recoilController?.AddRecoil(vertical, horizontal);
        }
    }
}