using UnityEngine;

namespace ActionSample.StateMachine
{
    public class EnemyStunState : EnemyState
    {
        private float timer;

        public EnemyStunState(EnemyController context) : base(context) { }

        public override void Enter()
        {
            base.Enter();
            timer = ctx.StunDuration;
            
            if (ctx.MeshRenderer != null)
            {
                ctx.MeshRenderer.material.color = Color.blue;
            }
            Debug.Log("Enemy Entered Stun State");
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                ctx.StateMachine.ChangeState(ctx.IdleState);
            }
        }

        public override void Exit()
        {
            base.Exit();
            
            if (ctx.MeshRenderer != null)
            {
                ctx.MeshRenderer.material.color = ctx.OriginalColor;
            }
            Debug.Log("Enemy Exited Stun State");
        }
    }
}
