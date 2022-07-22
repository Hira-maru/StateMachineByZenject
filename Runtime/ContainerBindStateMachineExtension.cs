using Zenject;

namespace Workspace
{
    public static class ContainerBindStateMachineExtension
    {
        public static void BindStateMachine<T>(this DiContainer container)
        {
            container.Bind(typeof( IStateMachine<T> ), typeof( ITickable ), typeof( ILateTickable ))
                .FromSubContainerResolve()
                .ByInstaller<StateMachineInstaller<T>>()
                .AsCached();
        }
    }
}
