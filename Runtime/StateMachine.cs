using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace Workspace
{
    class StateMachine<T> : IStateMachine<T>, ITickable, ILateTickable
    {
        Dictionary<T, IState<T>> _stateMap;

        IState<T> _currentState;

        [Inject]
        void Inject(IEnumerable<IState<T>> states)
        {
            if (states == null) throw new ArgumentNullException();

            _stateMap = states.ToDictionary(state => state.Id);
        }

        void IStateMachine<T>.ChangeState(T stateId)
        {
            if (! _stateMap.ContainsKey(stateId)) throw new KeyNotFoundException();

            _currentState?.End();
            _currentState = _stateMap[stateId];
            _currentState.Begin();
        }

        void ITickable.Tick()
        {
            _currentState?.Update();
        }

        void ILateTickable.LateTick()
        {
            _currentState?.LateUpdate();
        }
    }
}
