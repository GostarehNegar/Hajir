using Hajir.Crm.Blazor.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Hajir.Crm.Internals;

namespace Hajir.Crm.Blazor.Components
{
    public class BaseComponent : ComponentBase, IDisposable
    {
        protected List<IDisposable> _disposables = new List<IDisposable>();

        [Inject]
        public IServiceProvider ServiceProvider { get; set; }

        public IBlazorAppServices AppServices => this.ServiceProvider.GetService<IBlazorAppServices>();
        public void AddDisposable(IDisposable disposable) => _disposables.Add(disposable);
        public void Dispose()
        {
            _disposables.ForEach(x => x.Dispose());
        }
    }
    public class BaseComponent<T> : BaseComponent where T : class, new()
    {
        [Inject]
        public State<T> State { get; set; }

        public T Value => State.Value;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            AddDisposable(State.On((x) => InvokeAsync(StateHasChanged)));
        }

    }

    public class BaseCollectionComponent<T> : BaseComponent where T : class, new()
    {
        [Inject]
        public StateCollection<T> States { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            AddDisposable(States.On((x) => InvokeAsync(StateHasChanged)));
        }
    }
}
