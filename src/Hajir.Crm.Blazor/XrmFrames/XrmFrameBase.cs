


using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.XrmFrames
{
    public class XrmFrameBase : ComponentBase, IAsyncDisposable
    {
        protected List<IDisposable> _disposables = new List<IDisposable>();
        [Inject]
        public IServiceProvider ServiceProvider { get; set; }

        private const int DEFAULT_TIMEOUT = XrmFrameAdapter.DEFAULT_TIMEOUT;

        protected XrmFrameAdapter Adapter;
        private bool _adapterInitialized;
        protected Exception LastError;

        public const string XrmFrames = "xrmframes";
        public string ErrorMessage { get; set; }

        public XrmFrameBase()
        {

        }
        protected override void OnInitialized()
        {
            this.Adapter = new XrmFrameAdapter(ServiceProvider);
            base.OnInitialized();
        }


        protected override Task OnInitializedAsync()
        {

            return base.OnInitializedAsync();
        }
        public virtual Task<bool> XrmInitializeAsync()
        {
            return Task.FromResult(false);
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && !this._adapterInitialized)
            {
                var id = this.EntityId;
                await this.Adapter.Evaluate<int>("2*2");
                this._adapterInitialized = true;
                await base.OnAfterRenderAsync(firstRender);
                if (await XrmInitializeAsync())
                {
                    StateHasChanged();
                }

            }

        }
        protected string TypeName => GetQueryParameters().Get("typename");
        protected string OrgName => GetQueryParameters().Get("orgname");
        protected string DataParam => GetQueryParameters().Get("data");

        protected Guid? EntityId
        {
            get
            {
                return Guid.TryParse(GetQueryParameters().Get("id"), out var _id)
                    ? _id
                    : null;
            }
        }
        protected NameValueCollection GetQueryParameters()
        {
            try
            {
                var uri = new Uri(this.ServiceProvider.GetService<NavigationManager>().Uri);
                return System.Web.HttpUtility.ParseQueryString(uri.Query);
            }
            catch { }
            return new NameValueCollection();
        }
        public async Task SetAttributeValue(string attributeName, string value)
        {

        }
        public async Task<string> GetAttibuteType(string attributeName)
        {
            var result = await this
                .Adapter
                .Evaluate<string>($"parent.Xrm.Page.getAttribute('{attributeName}').getAttributeType();");
            return result;
        }
        public async Task<string> GetControlType(string name)
        {
            var result = await this
                .Adapter
                .Evaluate<string>($"parent.Xrm.Page.getAttribute('{name}').getControlType();");
            return result;
        }
        public async Task<T> GetAttributeValue<T>(string attributeName)
        {
            try
            {
                var result = await this.Adapter.Evaluate<T>($"parent.Xrm.Page.getAttribute('{attributeName}').getValue();");
                return result;
            }
            catch (Exception err)
            {
                throw;
            }

        }
        public static bool IsXrmPage(string url)
        {
            return url != null && url.ToLowerInvariant().Contains("xrmframes");
        }
        public static void Redirect(string url)
        {

        }
        public static void HandleNotFound(NavigationManager navigation)
        {
            navigation.NavigateTo("/xrmframes/quotedetail");
            var uri = navigation.Uri;

        }

        public async ValueTask DisposeAsync()
        {
            if (this.Adapter != null)
                await this.Adapter.DisposeAsync();
            this._disposables.ForEach(x => x.Dispose());
        }
        public Task<T> Evaluate<T>(string expression, int Timeout = DEFAULT_TIMEOUT)
        {
            return this.Adapter.Evaluate<T>(expression, Timeout);
        }

        public Task<T> EvaluateAttibuteMethod<T>(string attributeName, string method, int timeOut = DEFAULT_TIMEOUT)
        {
            return this.Adapter.EvaluateAttibuteMethod<T>(attributeName, method, timeOut);
        }
        public Task<T> EvaluateEntityMethod<T>(string method, int timeOut = DEFAULT_TIMEOUT)
        {
            return this.Adapter.EvaluateEntityMethod<T>(method, timeOut);
        }

        public Task<Guid> GetId()
        {
            return this.Adapter.EvaluateEntityMethod<Guid>("getId()", DEFAULT_TIMEOUT);
        }
        public async Task SafeExecute(Func<Task> func)
        {
            try
            {
                await func();
            }
            catch (Exception err)
            {
                this.LastError = err.GetBaseException();
            }

        }

        protected void AddDisposable(IDisposable disp) => this._disposables.Add(disp);
    }
    public class XrmFrameBase<T> : XrmFrameBase where T : class, new()
    {
        public State<T> State { get; set; } = new State<T>();
        public T Value => this.State.Value;
        protected void SetState(State<T> state)
        {
            this.State = state;
            AddDisposable(state.On((x) => InvokeAsync(StateHasChanged)));
        }
    }
}
