using UnityEngine;

namespace ActionSample
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public Vector3 MovementInput { get; private set; }
        public bool SlideTriggered { get; private set; }
        public Vector3 MousePosition { get; private set; }

        private void Update()
        {
            // 入力の取得
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");
            MovementInput = new Vector3(x, 0, z).normalized;

            SlideTriggered = Input.GetKeyDown(KeyCode.LeftControl);
            MousePosition = Input.mousePosition;
        }
    }
}
