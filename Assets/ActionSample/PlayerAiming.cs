using UnityEngine;

namespace ActionSample
{
    public class PlayerAiming : MonoBehaviour
    {
        [Header("Look Settings")]
        public float MouseSensitivity = 2.0f;
        public float MaxLookAngle = 80f;
        [SerializeField]
        private Transform _mainCamera;
        private float _currentPitch = 0f;

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
            float yaw = lookInput.x * MouseSensitivity;
            transform.Rotate(0, yaw, 0);

            // 垂直回転 (Camera)
            float pitchDelta = -lookInput.y * MouseSensitivity;
            _currentPitch += pitchDelta;
            _currentPitch = Mathf.Clamp(_currentPitch, -MaxLookAngle, MaxLookAngle);

            _mainCamera.transform.localEulerAngles = new Vector3(_currentPitch, 0, 0);
        }
    }
}