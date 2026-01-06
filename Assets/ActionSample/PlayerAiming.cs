using UnityEngine;

namespace ActionSample
{
    public class PlayerAiming : MonoBehaviour
    {
        [Header("Look Settings")]
        public float MouseSensitivity = 2.0f;
        public float MaxLookAngle = 80f;
        [Header("Recoil Settings")]
        public float RecoilRecoverySpeed = 10f;
        
        [SerializeField]
        private Transform _mainCamera;
        
        private float _currentPitch = 0f;
        private float _recoilPitch = 0f;
        private float _recoilYaw = 0f;

        private void Awake()
        {
            
            // FPS視点ではカーソルをロックするのが一般的
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void UpdateLook(Vector2 lookInput)
        {
            if (_mainCamera == null) return;

            // 水平回転 (Player Body)
            // リコイルのYaw成分も考慮
            float yaw = lookInput.x * MouseSensitivity;
            transform.Rotate(0, yaw + _recoilYaw, 0);

            // 垂直回転 (Camera)
            float pitchDelta = -lookInput.y * MouseSensitivity;
            _currentPitch += pitchDelta;
            _currentPitch = Mathf.Clamp(_currentPitch, -MaxLookAngle, MaxLookAngle);

            // リコイル適用後の最終的なPitch
            float finalPitch = _currentPitch - _recoilPitch; // Recoilは銃が跳ね上がるのでマイナス（上向き）

            _mainCamera.transform.localEulerAngles = new Vector3(finalPitch, 0, 0);

            // リコイルの回復
            RecoverRecoil();
        }

        /// <summary>
        /// リコイルを追加します。
        /// </summary>
        /// <param name="vertical">縦方向の跳ね上がり量（正の値）</param>
        /// <param name="horizontal">横方向のブレ量</param>
        public void AddRecoil(float vertical, float horizontal)
        {
            _recoilPitch += vertical;
            _recoilYaw += horizontal;
        }

        /// <summary>
        /// 時間経過とともにリコイルを減衰させます。
        /// </summary>
        private void RecoverRecoil()
        {
            if (_recoilPitch > 0)
            {
                _recoilPitch -= Time.deltaTime * RecoilRecoverySpeed;
                if (_recoilPitch < 0) _recoilPitch = 0;
            }

            if (Mathf.Abs(_recoilYaw) > 0)
            {
                // 横方向は0に向かって減衰
                _recoilYaw = Mathf.Lerp(_recoilYaw, 0, Time.deltaTime * RecoilRecoverySpeed);
            }
        }
    }
}