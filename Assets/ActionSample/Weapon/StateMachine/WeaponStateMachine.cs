namespace ActionSample.Weapon.StateMachine
{
    public class WeaponStateMachine
    {
        public IWeaponState CurrentState { get; private set; }

        public void Initialize(IWeaponState startingState)
        {
            CurrentState = startingState;
            startingState.Enter();
        }

        public void ChangeState(IWeaponState newState)
        {
            CurrentState.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }
    }
}
