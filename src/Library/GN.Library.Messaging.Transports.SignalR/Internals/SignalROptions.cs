using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Messaging.Transports.SignalR.Internals
{
	public class SignalROptions
	{
		public bool Disabled { get; set; }
		public string ServerUrl { get; set; }
		public bool UseOldProtocol { get; set; }
		public string GetHubUrl()
        {
			var ret = ValidateSignalRTransportConnectionString(this.ServerUrl);
			return ret;
        }

		//internal static string FixUrl(string url)
		//      {
		//	if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
		//	{
		//		url = "http://localhost:5000";
		//	}
		//	var uri = new Uri(url);
		//	return  $"{uri.Scheme}://{uri.Host}" + (uri.Port == 0 ? "" : $":{uri.Port}") + LibraryConventions.Constants.MessageBusHubUrl;

		//}
		public string ValidateSignalRTransportConnectionString(string url)
		{
			if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
			{
				url = "http://localhost:5000";
			}
			var uri = new Uri(url);
			return $"{uri.Scheme}://{uri.Host}" + (uri.Port == 0 ? "" : $":{uri.Port}") + LibraryConventions.Constants.MessageBusHubUrl;
		}
		public SignalROptions Validate()
        {
			this.ServerUrl = ValidateSignalRTransportConnectionString(this.ServerUrl);
			return this;
        }
        public override string ToString()
        {
			return $"ServerUrl:{GetHubUrl()}";
        }
    }
	public class SignalRHubOptions
	{
		public bool Enabled { get; set; }

		public SignalRHubOptions()
        {
			Enabled = false;
        }
		public SignalRHubOptions Validate()
        {
			return this;
        }
        public override string ToString()
        {
			return $" Enabled :{Enabled}";
        }
    }
}
