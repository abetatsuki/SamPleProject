using UnityEngine;

namespace ActionSample.Weapon.StateMachine
{
    public class WeaponIdleState : WeaponState
    {
        public WeaponIdleState(WeaponController context) : base(context) { }

        public override void Enter()
        {
            base.Enter();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Reload Input
            if (ctx.InputHandler.ReloadInput)
            {
                if (ctx.CurrentAmmo < ctx.MaxAmmo)
                {
                    ctx.StateMachine.ChangeState(ctx.ReloadState);
                    return;
                }
            }

            // Fire Input
            if (ctx.InputHandler.FireInput)
            {
                if (ctx.CurrentAmmo > 0)
                {
                    ctx.StateMachine.ChangeState(ctx.FireState);
                }
                else
                {
                    // Empty Click sound?
                    // Auto Reload?
                    ctx.StateMachine.ChangeState(ctx.ReloadState);
                }
            }
        }
    }
}
