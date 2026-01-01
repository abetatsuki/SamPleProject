using UnityEngine;
using UniRx;
using UnityEngine.InputSystem;

namespace InventorySample
{
    public class PlayerInputSystem
    {
        public PlayerInputSystem(PlayerInput playerInput)
        {
            _playerInput = playerInput;
            _selectAction = _playerInput.actions[SelectActionName];
            _selectAction.started += Select;

        }


        public IReadOnlyReactiveProperty<int> SelectIndex => _selectIndex;

        public void Select(InputAction.CallbackContext ctx)
        {
            float value = ctx.ReadValue<float>();
            int delta = Mathf.RoundToInt(value);
            _selectIndex.Value += delta;
            Debug.Log(_selectIndex.Value);
        }



        private readonly ReactiveProperty<int> _selectIndex = new ReactiveProperty<int>(0);
        private PlayerInput _playerInput;
        private InputAction _selectAction;
        private const string SelectActionName = "Select";

    }
}