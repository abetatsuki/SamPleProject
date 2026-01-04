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
            
            // Stop agent if it exists
            if (ctx.NavAgent != null && ctx.NavAgent.isActiveAndEnabled)
            {
                ctx.NavAgent.isStopped = true;
                ctx.NavAgent.velocity = Vector3.zero;
            }

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
                if (ctx.Waypoints != null && ctx.Waypoints.Length > 0)
                {
                    ctx.StateMachine.ChangeState(ctx.PatrolState);
                }
                else
                {
                    ctx.StateMachine.ChangeState(ctx.IdleState);
                }
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
