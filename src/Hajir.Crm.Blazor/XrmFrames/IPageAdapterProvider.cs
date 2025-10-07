using GN.Library.Shared.Entities;
using Hajir.Crm.Blazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.XrmFrames
{
    public interface IXrmFrame
    {
        IServiceProvider ServiceProvider { get; }
        public XrmFrameAdapter Adapter { get; }
    }
    class XrmUserSettings
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
    }
    public static class XrmFrameAdapterExtensions
    {

        public const int DEFAULT_TIMEOUT = 15000;
        public static XrmFrameAdapter GetAdapter(this BaseComponent component)
        {
            return component.ServiceProvider.GetService<XrmFrameAdapter>();
        }
        public static bool IsOnXrmFrame(this IXrmFrame adapter)
        {
            return true;
        }
        public static NameValueCollection GetQueryParameters(this BaseComponent component)
        {
            try
            {
                var uri = new Uri(component.ServiceProvider.GetService<NavigationManager>().Uri);
                return System.Web.HttpUtility.ParseQueryString(uri.Query);
            }
            catch { }
            return new NameValueCollection();
        }
        public static Task<T> Evaluate<T>(this IXrmFrame frame, string expression, int Timeout = DEFAULT_TIMEOUT)
        {
            return frame.Adapter.Evaluate<T>(expression, Timeout);
        }

        public static Task<T> EvaluateAttibuteMethod<T>(this IXrmFrame frame,string attributeName, string method, int timeOut = DEFAULT_TIMEOUT)
        {
            return frame.Adapter.EvaluateAttibuteMethod<T>(attributeName, method, timeOut);
        }
        public static Task<T> EvaluateEntityMethod<T>(this IXrmFrame frame,string method, int timeOut = DEFAULT_TIMEOUT)
        {
            return frame.Adapter.EvaluateEntityMethod<T>(method, timeOut);
        }
        public static async Task SetLookupValue(this IXrmFrame frame,string attributeName, string id, string entityType)
        {
            var exp = $"var aval=[]; var val = {{}};val.id = '{id}';val.entityType = '{entityType}';aval.push(val); parent.Xrm.Page.getAttribute('{attributeName}').setValue(aval);";
            await frame.Adapter.Evaluate<int>(exp);

        }
        
        public static async Task<DynamicEntityReference> GetLookupValue(this IXrmFrame frame, string attributeName)
        {
            var exp = $"parent.Xrm.Page.getAttribute('{attributeName}').getValue();";
            var values = await frame.Evaluate<DynamicEntityReference[]>(exp);

            return values?.FirstOrDefault();
        }
        
        public static async Task<CurrentUser> GetCurrentUser(this IXrmFrame frame )
        {
            var user = frame.ServiceProvider.GetUserContext().CurrentUser();
            if (user != null)
            {
                return user;
            }
            try
            {
                var res = await frame.Adapter.Evaluate<string>(
                    $"parent.Xrm.Utility.getGlobalContext().userSettings.userName +'::'+parent.Xrm.Utility.getGlobalContext().userSettings.userId;",5000);
                var parts = res.Split("::");

                user = new CurrentUser
                {
                    UserName = parts[0],
                    UserId = parts.Length > 1 ? parts[1] : null
                };
            }
            catch { }
            frame.ServiceProvider.GetUserContext().CurrentUser(user);
            return user;

        }
        public static async Task SetAttributeValue(this IXrmFrame frame, string attributeName, object value)
        {
            if (value == null)
            {
                await frame.Adapter.Evaluate($"parent.Xrm.Page.getAttribute('{attributeName}').setValue(null);");
            }
            else if (value.GetType() == typeof(string))
            {
                await frame.Adapter.Evaluate($"parent.Xrm.Page.getAttribute('{attributeName}').setValue('{value}');");
            }
            else if (value.GetType() == typeof(bool) || value.GetType() == typeof(bool?))
            {
                await frame.Adapter.Evaluate($"parent.Xrm.Page.getAttribute('{attributeName}').setValue({value.ToString().ToLowerInvariant()});");
            }
            else if (value.GetType() == typeof(DateTime) || value.GetType() == typeof(DateTime?))
            {
                var dt = ((DateTime)value).ToString("s");
                await frame.Adapter.Evaluate(
                    $"var d= new Date('{dt}'); parent.Xrm.Page.getAttribute('{attributeName}').setValue(d);");
            }
            else if (value.GetType().IsEnum)
            {
                var ienum = (int)value;
                await frame.Adapter.Evaluate($"parent.Xrm.Page.getAttribute('{attributeName}').setValue({ienum});");
            }
            else
            {
                await frame.Adapter.Evaluate($"parent.Xrm.Page.getAttribute('{attributeName}').setValue({value});");
            }
        }

    }
}
