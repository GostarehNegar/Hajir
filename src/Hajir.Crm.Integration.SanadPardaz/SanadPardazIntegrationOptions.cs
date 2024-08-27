using System;

namespace Hajir.Crm.Integration.SanadPardaz
{
    public class SanadPardazIntegrationOptions
    {
        /// <summary>
        /// Url to SanadPardaz api server.
        /// </summary>
        public string ApiUrl { get; set; }

        internal string GetBaseApiAddress()
        {
            var result = this.ApiUrl;
            if (!result.Trim().EndsWith("/"))
            {
                result = result + "/";
            }

            
            return $"{result}api/v2/";
        }

        public SanadPardazIntegrationOptions Validate()
        {
            this.ApiUrl = Uri.IsWellFormedUriString(this.ApiUrl, uriKind: UriKind.Absolute)
                ? this.ApiUrl
                : "http://192.168.20.7:81/";
            if (!this.ApiUrl.Trim().EndsWith("/"))
            {
                this.ApiUrl = this.ApiUrl + "/";
            }
            return this;

        }
    }
}