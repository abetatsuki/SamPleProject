namespace ActionSample.Weapon.StateMachine
{
    public interface IWeaponState
    {
        void Enter();
        void Exit();
        void LogicUpdate();
    }
}
