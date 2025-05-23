﻿using Hajir.Crm.Blazor.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Hajir.Crm;
using Hajir.Crm.Blazor.ViewModels;

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
        public void SetError(Exception err)
        {
            this.ServiceProvider.GetState<ErrorModel>().SetState(new ErrorModel { Error = err });
        }
        public async Task SafeExecute(Func<Task> func)
        {
            try
            {
                await func();
            }
            catch (Exception err)
            {
                //this.LastError = err.GetBaseException();
                this.SetError(err);
            }

        }

        public void SafeExecute(Action func)
        {
            try
            {
                func();
            }
            catch (Exception err)
            {
                //this.LastError = err.GetBaseException();
                this.SetError(err);
            }

        }
    }
    public class BaseComponent<T> : BaseComponent where T : class, new()
    {
        [Parameter]
        public State<T> State { get; set; }

        public T Value => State.Value;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            //AddDisposable(State.On((x) => InvokeAsync(StateHasChanged)));
        }
        protected override async Task OnParametersSetAsync()
        {
            this.SetState(await LoadState());

            await base.OnParametersSetAsync();
        }
        public virtual Task<State<T>> LoadState()
        {
            return Task.FromResult(this.State ?? this.ServiceProvider.GetState<T>());
        }
        protected void SetState(State<T> state)
        {
            this.State = state;
            AddDisposable(State.On((x) => InvokeAsync(StateHasChanged)));
        }

    }

    public class BaseComponentEx<T> : BaseComponent where T : State<T>, new() 
    {
        [Parameter]
        public T State { get; set; }

        public T Value => State.Value;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            //AddDisposable(State.On((x) => InvokeAsync(StateHasChanged)));
        }
        protected override async Task OnParametersSetAsync()
        {
            this.SetState(await LoadState());

            await base.OnParametersSetAsync();
        }
        public virtual Task<T> LoadState()
        {
            return Task.FromResult(this.State ?? this.ServiceProvider.GetStateEx<T>());
        }
        protected void SetState(T state)
        {
            this.State = state;
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
