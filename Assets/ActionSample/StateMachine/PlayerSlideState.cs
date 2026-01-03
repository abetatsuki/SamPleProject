using UnityEngine;

namespace ActionSample.StateMachine
{
    public class PlayerSlideState : PlayerState
    {
        private float slideTimer;

        public PlayerSlideState(PlayerController context) : base(context) { }

        public override void Enter()
        {
            base.Enter();
            slideTimer = ctx.SlideDuration;

            // スライディング開始 (入力方向 or 前方)
            Vector3 slideDir = ctx.InputHandler.MovementInput.magnitude > 0.1f 
                ? ctx.InputHandler.MovementInput 
                : ctx.transform.forward;

            ctx.Rigidbody.linearVelocity = slideDir * ctx.SlideSpeed;
            ctx.Rigidbody.linearDamping = ctx.SlideDrag;
        }

        public override void Exit()
        {
            base.Exit();
            ctx.Rigidbody.linearDamping = 0f;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            // スライディング中は他の入力を受け付けない（キャンセル等の仕様があればここに追記）
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            slideTimer -= Time.fixedDeltaTime;

            if (slideTimer <= 0f)
            {
                // 終了後は、入力があればWalk、なければIdle
                if (ctx.InputHandler.MovementInput.sqrMagnitude > 0.01f)
                {
                    ctx.StateMachine.ChangeState(ctx.WalkState);
                }
                else
                {
                    ctx.StateMachine.ChangeState(ctx.IdleState);
                }
            }
        }
    }
}
