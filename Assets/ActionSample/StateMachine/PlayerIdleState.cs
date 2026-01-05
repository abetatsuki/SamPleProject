using UnityEngine;

namespace ActionSample.StateMachine
{
    public class PlayerIdleState : PlayerState
    {
        public PlayerIdleState(PlayerController context) : base(context) { }

        public override void Enter()
        {
            // 停止させる
            Vector3 currentVel = ctx.Rigidbody.linearVelocity;
            ctx.Rigidbody.linearVelocity = new Vector3(0, currentVel.y, 0);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // 移動入力があればWalkへ
            if (ctx.InputHandler.MovementInput.sqrMagnitude > 0.01f)
            {
                ctx.StateMachine.ChangeState(ctx.WalkState);
            }
            if(ctx.InputHandler.SlideTriggered)
            {
                ctx.StateMachine.ChangeState(ctx.SlideState);
            }
        }
    }
}
