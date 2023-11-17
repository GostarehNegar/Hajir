using System;
using System.Net;
using System.Threading.Tasks;

namespace Xrm.Tools.WebAPI.Requests
{
    public class CRMWebAPIConfig
    {
        public string APIUrl { get; set; }
        public string AccessToken { get; set; }
        public bool ResolveUnicodeNames { get; set; }
        public Guid CallerID { get; set; }
        public Func<string, Task<string>> GetAccessToken { get; set; }
        public NetworkCredential   NetworkCredential { get; set; }
        public CRMWebAPILoggingOptions Logging { get; set; }
        /// <summary>
        /// Due to some unknown bug the httpclient normal awaiting fails in 
        /// Blazor server programs. This flag controls how httpclient is
        /// invoked
        ///     await httpclient.send()  
        /// when this flag is true it will be called synchronously:
        ///     httpclient.send().confgureawait().getAwaiter().getResult()
        /// </summary>
        public static bool UseHttpClientSynchronouslyDueToUnknownBugInAwaitingInBlazor { get; set; }

    }
}
