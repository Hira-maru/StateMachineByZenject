using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Zenject;

namespace Workspace
{
    sealed class StateMachineInstaller<T> : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<StateMachine<T>>().AsCached();

            foreach (var stateType in FindStateTypes())
            {
                Container.BindInterfacesTo(stateType).AsCached();
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
