using UnityEngine;

namespace ActionSample.StateMachine
{
    public class EnemyIdleState : EnemyState
    {
        public EnemyIdleState(EnemyController context) : base(context) { }

        public override void Enter()
        {
            base.Enter();
            // Future logic: Trigger idle animation, etc.
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            // Future logic: Check for player in range -> Transition to ChaseState
        }
    }
}
