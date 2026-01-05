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
                int ammoNeeded = ctx.MaxAmmo - ctx.CurrentAmmo;
                int ammoToLoad = Mathf.Min(ammoNeeded, ctx.TotalAmmo);

                ctx.CurrentAmmo += ammoToLoad;
                ctx.TotalAmmo -= ammoToLoad;

                Debug.Log($"Reload Complete! Ammo: {ctx.CurrentAmmo}/{ctx.TotalAmmo}");
                ctx.StateMachine.ChangeState(ctx.IdleState);
            }
        }
    }
}
