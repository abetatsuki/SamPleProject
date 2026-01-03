using UnityEngine;

namespace ActionSample.StateMachine
{
    public abstract class PlayerState : IState
    {
        protected PlayerController ctx;

        public PlayerState(PlayerController context)
        {
            this.ctx = context;
        }

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void LogicUpdate() 
        {
            // 共通の入力処理や状態遷移チェックがあればここに書く
        }
        public virtual void PhysicsUpdate() { }
    }
}
