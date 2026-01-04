using UnityEngine;

namespace ActionSample.Weapon.StateMachine
{
    public class WeaponReloadState : WeaponState
    {
        private float reloadTimer;

        public WeaponReloadState(WeaponController context) : base(context) { }

        public override void Enter()
        {
            base.Enter();
            reloadTimer = ctx.ReloadTime;
            Debug.Log("Reloading...");
            // Play Reload Animation
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            reloadTimer -= Time.deltaTime;

            if (reloadTimer <= 0f)
            {
                ctx.CurrentAmmo = ctx.MaxAmmo;
                Debug.Log("Reload Complete!");
                ctx.StateMachine.ChangeState(ctx.IdleState);
            }
        }
    }
}
