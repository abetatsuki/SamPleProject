using UnityEngine;
using ActionSample.StateMachine;

namespace ActionSample.Weapon.StateMachine
{
    public abstract class WeaponState : IState
    {
        protected WeaponController ctx;

        public WeaponState(WeaponController context)
        {
            this.ctx = context;
        }

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void LogicUpdate() { }
        public virtual void PhysicsUpdate() { }
    }
}
