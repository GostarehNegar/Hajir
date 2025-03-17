using Hajir.Crm.Blazor.Services;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Hajir.Crm
{
    internal class StateManager : IStateManager, IScopedHostedService
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

        public Task InitializeAsync(IServiceProvider serviceProvider)
        {
            return Task.CompletedTask;
        }

        public Task OnConnectAsync(string circuitId)
        {
            return Task.CompletedTask;
        }

        public Task OnDisconnectAsync(string circuitId)
        {
            return Task.CompletedTask;
        }

        public Task OnAfterRenderAsync(IServiceProvider serviceProvider, bool isfirst)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
           
        }
    }
}
