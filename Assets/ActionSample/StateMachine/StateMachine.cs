namespace ActionSample.StateMachine
{
    public interface IState
    {
        void Enter();
        void Exit();
        void LogicUpdate();
        void PhysicsUpdate();
    }

    public class StateMachine
    {
        public IState CurrentState { get; private set; }

        public void Initialize(IState startingState)
        {
            CurrentState = startingState;
            startingState.Enter();
        }

        public void ChangeState(IState newState)
        {
            CurrentState.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }
    }
}
