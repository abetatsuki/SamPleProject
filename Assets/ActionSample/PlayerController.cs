using UnityEngine;

namespace ActionSample
{
    // 依存関係を明示
    [RequireComponent(typeof(PlayerInputHandler))]
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerAiming))]
    public class PlayerController : MonoBehaviour
    {
        private PlayerInputHandler _input;
        private PlayerMovement _movement;
        private PlayerAiming _aiming;

        private void Awake()
        {
            // コンポーネントの取得
            _input = GetComponent<PlayerInputHandler>();
            _movement = GetComponent<PlayerMovement>();
            _aiming = GetComponent<PlayerAiming>();
        }

        private void Update()
        {
            // 入力に基づいてロジックを実行

            // スライディング入力のチェック (移動入力がある時のみ)
            if (_input.SlideTriggered && _input.MovementInput.magnitude > 0.1f)
            {
                _movement.StartSlide(_input.MovementInput);
            }

            // 照準の更新
            _aiming.UpdateAiming(_input.MousePosition);
        }

        private void FixedUpdate()
        {
            // 物理挙動の更新
            if (_movement.IsSliding)
            {
                _movement.HandleSlideUpdate();
            }
            else
            {
                _movement.Move(_input.MovementInput);
            }
        }
    }
}
