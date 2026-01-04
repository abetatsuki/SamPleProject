using UnityEngine;

namespace ActionSample.Weapon.StateMachine
{
    public abstract class WeaponState : IWeaponState
    {
        protected WeaponController ctx;

        public WeaponState(WeaponController context)
        {
            this.ctx = context;
        }

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void LogicUpdate() { }
    }
}
