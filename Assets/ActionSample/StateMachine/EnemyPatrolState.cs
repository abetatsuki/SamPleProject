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
            // Start moving towards the nearest or next waypoint
            if (ctx.Waypoints == null || ctx.Waypoints.Length == 0)
            {
                ctx.StateMachine.ChangeState(ctx.IdleState);
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

            Transform target = ctx.Waypoints[currentWaypointIndex];
            Vector3 direction = (target.position - ctx.transform.position).normalized;
            float distance = Vector3.Distance(ctx.transform.position, target.position);

            // Move towards target
            // Note: Simple transform movement. Use Rigidbody or NavMeshAgent for better physics/navigation.
            ctx.transform.position += direction * ctx.MoveSpeed * Time.deltaTime;
            
            // Face the target
            if (direction != Vector3.zero)
            {
                ctx.transform.rotation = Quaternion.LookRotation(direction);
            }

            // Check if reached
            if (distance < 0.2f)
            {
                StartWait(1.0f); // Wait 1 second at each waypoint
            }
        }

        private void StartWait(float duration)
        {
            isWaiting = true;
            waitTimer = duration;
        }

        private void NextWaypoint()
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % ctx.Waypoints.Length;
        }
    }
}
