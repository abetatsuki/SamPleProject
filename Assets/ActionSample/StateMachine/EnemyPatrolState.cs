using UnityEngine;

namespace ActionSample.StateMachine
{
    public class EnemyPatrolState : EnemyState
    {
        private int currentWaypointIndex = 0;
        private float waitTimer;
        private bool isWaiting;

        public EnemyPatrolState(EnemyController context) : base(context) { }

        public override void Enter()
        {
            base.Enter();
            ctx.NavAgent.isStopped = false;
            
            // Start moving towards the nearest or next waypoint
            if (ctx.Waypoints == null || ctx.Waypoints.Length == 0)
            {
                ctx.StateMachine.ChangeState(ctx.IdleState);
            }
            else
            {
                SetDestinationToCurrentWaypoint();
            }
        }

        public override void Exit()
        {
            base.Exit();
            // Stop movement when exiting patrol (e.g., when stunned)
            if (ctx.NavAgent.enabled)
            {
                ctx.NavAgent.isStopped = true;
            }
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (isWaiting)
            {
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0f)
                {
                    isWaiting = false;
                    NextWaypoint();
                }
                return;
            }

            // Check if agent has reached the destination
            if (!ctx.NavAgent.pathPending && ctx.NavAgent.remainingDistance <= ctx.NavAgent.stoppingDistance)
            {
                if (!ctx.NavAgent.hasPath || ctx.NavAgent.velocity.sqrMagnitude == 0f)
                {
                    StartWait(1.0f); // Wait 1 second at each waypoint
                }
            }
        }

        private void SetDestinationToCurrentWaypoint()
        {
            if (ctx.Waypoints.Length == 0) return;
            ctx.NavAgent.SetDestination(ctx.Waypoints[currentWaypointIndex].position);
        }

        private void StartWait(float duration)
        {
            isWaiting = true;
            waitTimer = duration;
        }

        private void NextWaypoint()
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % ctx.Waypoints.Length;
            SetDestinationToCurrentWaypoint();
        }
    }
}
