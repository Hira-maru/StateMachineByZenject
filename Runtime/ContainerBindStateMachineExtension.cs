using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Zenject;

namespace Workspace
{
    public static class ContainerBindStateMachineExtension
    {
        public static void BindStateMachine<T>(this DiContainer container, T defaultStateId)
        {
            container.Bind(typeof( IStateMachine<T> ), typeof( IInitializable ), typeof( ITickable ), typeof( ILateTickable ), typeof( IDisposable ))
                .FromSubContainerResolve()
                .ByMethod(InstallSubContainer)
                .AsCached()
                .NonLazy();

            void InstallSubContainer(DiContainer subContainer)
            {
                subContainer.BindInterfacesTo<StateMachine<T>>().AsCached().WithArguments(defaultStateId);

                foreach (var stateType in FindStateTypes())
                {
                    subContainer.BindInterfacesTo(stateType).AsCached();
                }
            }

            static IEnumerable<Type> FindStateTypes()
            {
                var stateInterfaceType = typeof( IState<T> );
                return Assembly.GetAssembly(typeof( T ))
                    .GetTypes()
                    .Where(type => ! type.IsAbstract)
                    .Where(type => type.GetInterfaces().Any(typeInterface => typeInterface == stateInterfaceType));
            }
        }
    }
}
