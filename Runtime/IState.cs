namespace Workspace
{
    public interface IState<out T>
    {
        T Id { get; }
        void Begin();
        void Update();
        void LateUpdate();
        void End();
    }
}