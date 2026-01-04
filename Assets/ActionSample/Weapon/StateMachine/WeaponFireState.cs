using UnityEngine;

namespace ActionSample.Weapon.StateMachine
{
    public class WeaponFireState : WeaponState
    {
        private float nextFireTime;

        public WeaponFireState(WeaponController context) : base(context) { }

        public override void Enter()
        {
            base.Enter();
            Fire();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            // Wait for fire rate
            if (Time.time >= nextFireTime)
            {
                // If button held (Automatic) or just finished (Semi-Auto logic handled by transitions)
                // For now, simple return to Idle to re-check input
                ctx.StateMachine.ChangeState(ctx.IdleState);
            }
        }

        private void Fire()
        {
            ctx.CurrentAmmo--;
            nextFireTime = Time.time + ctx.FireRate;

            // Firing Logic (Raycast, Projectile, etc.)
            Debug.Log($"Bang! Ammo: {ctx.CurrentAmmo}");
            
            // Visuals (Muzzle Flash, Recoil) would go here
            // ctx.ApplyRecoil();
        }
    }
}
