using UnityEngine;
using UniRx;
using UnityEngine.InputSystem;
using System;

namespace InventorySample
{
    public class PlayerInputSystem
    {
        public PlayerInputSystem(PlayerInput playerInput)
        {
            _playerInput = playerInput;
            _selectAction = _playerInput.actions[SelectActionName];
            _selectAction.started += OnSelectAction;

        }

        public IObservable<int> OnSelect => _onSelectSubject;


        public void OnSelectAction(InputAction.CallbackContext ctx)
        {
            float value = ctx.ReadValue<float>();
            int delta = Mathf.RoundToInt(value);
            if (delta != 0)
            {
                _onSelectSubject.OnNext(delta);
            }
        }



        private readonly Subject<int> _onSelectSubject = new Subject<int>();
        private PlayerInput _playerInput;
        private InputAction _selectAction;
        private const string SelectActionName = "Select";

    }
}