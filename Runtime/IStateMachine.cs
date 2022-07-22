namespace Workspace
{
    public interface IStateMachine<in T>
    {
        void ChangeState(T stateId);
    }
}