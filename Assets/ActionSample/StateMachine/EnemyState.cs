using UnityEngine;

namespace ActionSample.StateMachine
{
    public abstract class EnemyState : IState
    {
        protected EnemyController ctx;

        public EnemyState(EnemyController context)
        {
            this.ctx = context;
        }

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void LogicUpdate() { }
        public virtual void PhysicsUpdate() { }
    }
}
