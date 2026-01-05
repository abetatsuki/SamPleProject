using UnityEngine;

namespace ActionSample
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private const string HorizontalAxis = "Horizontal";
        private const string VerticalAxis = "Vertical";
        private const string MouseXAxis = "Mouse X";
        private const string MouseYAxis = "Mouse Y";

        public Vector3 MovementInput { get; private set; }
        public bool AimInput { get; private set; }
        public bool SlideTriggered { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool ReloadInput {get; private set; }
        public bool FireInput { get; private set; }

        private void Update()
        {
            // 入力の取得
            float x = Input.GetAxisRaw(HorizontalAxis);
            float z = Input.GetAxisRaw(VerticalAxis);
            MovementInput = new Vector3(x, 0, z).normalized;
            
            LookInput = new Vector2(Input.GetAxis(MouseXAxis), Input.GetAxis(MouseYAxis));

            AimInput = Input.GetMouseButton(1);
            ReloadInput = Input.GetKeyDown(KeyCode.R);
            FireInput = Input.GetMouseButton(0);
            SlideTriggered = Input.GetKeyDown(KeyCode.LeftControl);
        }
    }
}
