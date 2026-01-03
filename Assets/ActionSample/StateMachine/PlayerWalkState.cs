using UnityEngine;

namespace ActionSample.StateMachine
{
    public class PlayerWalkState : PlayerState
    {
        public PlayerWalkState(PlayerController context) : base(context) { }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // 入力がなくなったらIdleへ
            if (ctx.InputHandler.MovementInput.sqrMagnitude < 0.01f)
            {
                ctx.StateMachine.ChangeState(ctx.IdleState);
            }
            // スライディング入力があればSlideへ
            else if (ctx.InputHandler.SlideTriggered)
            {
                ctx.StateMachine.ChangeState(ctx.SlideState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            
            // 移動処理
            Vector3 targetVelocity = ctx.InputHandler.MovementInput * ctx.MoveSpeed;
            targetVelocity.y = ctx.Rigidbody.linearVelocity.y; // 重力落下維持
            ctx.Rigidbody.linearVelocity = targetVelocity;
        }
    }
}
