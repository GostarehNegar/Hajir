using System;
using System.Collections.Generic;
using System.Linq;

namespace Hajir.Crm.Features.Common
{
    public class State<T> where T : class, new()
    {
        public class MyDisposable : IDisposable
        {
            private readonly Action dispose;

            internal MyDisposable(Action dispose)
            {
                this.dispose = dispose;
            }
            public void Dispose()
            {
                dispose.Invoke();
            }
        }

        public T Value { get; private set; }
        public delegate void StateChanged();
        public event StateChanged OnChanged;
        //public IObservable<T> Observable;
        public IDisposable On(Action<T> handler)
        {
            StateChanged _handler = () => handler(Value);
            OnChanged += _handler;
            return new MyDisposable(() =>
            {
                OnChanged -= _handler;
            });
        }
        public State() : this(new T())
        {
        }
        public State(T state)
        {
            Value = state;
            //Observable = System.Reactive.Linq.Observable.Create<T>(o =>
            //{
            //    StateChanged handler = () => o.OnNext(Value);
            //    OnChanged += handler;
            //    return () =>
            //    {
            //        OnChanged -= handler;
            //    };
            //});
        }
        public void SetState(T state)
        {
            Value = state;
            OnChanged?.Invoke();
        }
        public void SetState(Func<T, T> func)
        {
            SetState(func(Value));
        }
        public void SetState(Action<T> action)
        {
            action?.Invoke(Value);
            SetState(Value);
        }
    }

    public class StateCollection<T> where T : class, new()
    {
        private List<State<T>> items = new List<State<T>>();
        public delegate void StateChanged();
        public event StateChanged OnChanged;
        //public IObservable<IEnumerable<T>> Observable;
        public IEnumerable<State<T>> States => items;

        public IEnumerable<T> Items => items.Select(x => x.Value).ToArray();
        public StateCollection() : this(new List<T>()) { }

        public IDisposable On(Action<IEnumerable<T>> handler)
        {
            StateChanged _handler = () => handler(Items);
            OnChanged += _handler;
            return new State<T>.MyDisposable(() =>
            {
                OnChanged -= _handler;

            });

        }
        public StateCollection(IEnumerable<T> items)
        {
            //this.Value = state;
            //Observable = System.Reactive.Linq.Observable.Create<IEnumerable<T>>(o =>
            //{
            //    StateChanged handler = () => o.OnNext(Items);
            //    OnChanged += handler;
            //    return () =>
            //    {
            //        OnChanged -= handler;
            //    };
            //});
        }
        public void SetState(Action<List<State<T>>> configure)
        {
            configure?.Invoke(items);
            OnChanged?.Invoke();
        }
        public void Add(T item)
        {
            items.Add(new State<T>(item));
            OnChanged?.Invoke();
        }

    }

}
