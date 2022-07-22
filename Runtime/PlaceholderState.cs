using System;
using System.Collections;
using System.Collections.Generic;
using Zenject;

namespace Workspace
{
    public abstract class PlaceholderState<T> : IState<T>, ICollection<IDisposable>
    {
        #region ChangeState

        IStateMachine<T> _stateMachine;

        [Inject]
        void Inject(IStateMachine<T> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        protected void ChangeState(T stateId) => _stateMachine.ChangeState(stateId);

        #endregion

        #region IState

        public abstract T Id { get; }

        void IState<T>.Begin()
        {
            OnBegin();
        }

        void IState<T>.Update()
        {
            OnUpdate();
        }

        void IState<T>.LateUpdate()
        {
            OnLateUpdate();
        }

        void IState<T>.End()
        {
            OnEnd();
            foreach (var disposable in _disposables) disposable.Dispose();
            _disposables.Clear();
        }

        protected virtual void OnBegin() { }
        protected virtual void OnUpdate() { }
        protected virtual void OnLateUpdate() { }
        protected virtual void OnEnd() { }

        #endregion

        #region ICollection

        readonly List<IDisposable> _disposables = new();
        IEnumerator<IDisposable> IEnumerable<IDisposable>.GetEnumerator() => _disposables.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _disposables.GetEnumerator();
        void ICollection<IDisposable>.Add(IDisposable item) => _disposables.Add(item);
        void ICollection<IDisposable>.Clear() => _disposables.Clear();
        bool ICollection<IDisposable>.Contains(IDisposable item) => _disposables.Contains(item);
        void ICollection<IDisposable>.CopyTo(IDisposable[] array, int arrayIndex) => _disposables.CopyTo(array, arrayIndex);
        bool ICollection<IDisposable>.Remove(IDisposable item) => _disposables.Remove(item);
        int ICollection<IDisposable>.Count => _disposables.Count;
        bool ICollection<IDisposable>.IsReadOnly => false;

        #endregion
    }
}
