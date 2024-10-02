using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Hajir.Crm
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

    public interface IStateManager
    {
        State<T> GetState<T>(string name = null, Func<State<T>> constructor = null) where T : class, new();
    }
    internal class StateManager : IStateManager
    {
        private ConcurrentDictionary<string, object> items = new ConcurrentDictionary<string, object>();
        private string GetKey<T>(string name) => typeof(T).FullName + "-" + name ?? "noname";
        public State<T> GetState<T>(string name = null, Func<State<T>> constructor = null) where T : class, new()
        {
            var key = this.GetKey<T>(name);
            if (items.TryGetValue(key, out var state) && state is State<T> result)
            {
                return result;
            }
            result = constructor == null
                ? new State<T>()
                : constructor();
            items.TryAdd(key, result);
            return result;
        }
        public State<T> SetState<T>(string name = null, Func<State<T>> constructor = null) where T : class, new()
        {
            var key = this.GetKey<T>(name);
            var result = constructor == null
                ? new State<T>()
                : constructor();
            items.AddOrUpdate(key, result, (a, b) => result);
            return result;
        }
    }
}
