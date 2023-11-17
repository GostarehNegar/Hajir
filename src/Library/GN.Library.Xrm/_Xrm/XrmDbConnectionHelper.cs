using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Data
{
	/// <summary>
	/// Provides methods to get 'Organization' and 'Discovery' services.
	/// Use 'GetOrganizationService' with a connection string name to get 
	/// an OrganizationServiceProxy.
	/// Note the 'Ex' version of methods use more sophisticated authentication
	/// realms such as LiveId.
	/// </summary>
	/// <remarks>
	/// Full story:
	/// Finally I think I understood this mess around the IOrganizationService:
	///
	/// * OrganizationService and OrganizationServiceProxy
	/// IOrganizationService is only an interface, there are tow implementatations,
	/// OrganizationService which is in Microsoft.Xrm.Client which is somehow retired
	/// and the OrganizationServiceProxy from Microsoft.Xrm.Sdk. Naturally we will
	/// use the latter implementation. 
	/// Note: There is a CrmServiceClient in 'Xrm Tooling'. Maybe sometime in future
	/// we will swicth to this implementation.
	/// 
	/// * Connection String
	/// Probably the most convinient way to connect to crm is using the notion
	/// of a connection string.(https://msdn.microsoft.com/en-us/library/mt608573.aspx)
	/// There is a 'CrmConnection' class in 'Microsoft.Xrm.Client' that may be used
	/// to parse a connection string that later may be used to create an 'OrganizationService'.
	/// We will use our own connection string manager.
	/// Note that that Microsoft.Xrm.Client is no longer available in CRM 2016.
	/// 
	/// * Authentication
	/// The 'AuthenticateWithNoHelp' sample in SDK provides additional tools
	/// for authentication providers other than ActiveDirectory (e.g. LiveId). 
	/// See GetCredentials in the methods.
	/// In current implementation we will use only 'ActiveDirectory' and may igonre
	/// other methods of authentication. By the way we still include the mmethods
	/// for future usage. The 'GetOrganizationServiceProxyEx' and 'GetDiscoveryServiceEx'
	/// support mulitple authentication types.
	/// 
	/// Reference: http://msxrmtools.com/notes/details/348/multiple-ways-to-connect-to-microsoft-dynamics-crm-using-organization-service-proxy-and-context
	/// </remarks>
	public class CrmOrganizationServiceHelper
	{
		protected static ILogger log = typeof(CrmOrganizationServiceHelper).GetLogger();

		#region Private Methods
		private static TProxy GetProxy<TService, TProxy>(
			IServiceManagement<TService> serviceManagement,
			AuthenticationCredentials authCredentials)
			where TService : class
			where TProxy : ServiceProxy<TService>
		{
			Type classType = typeof(TProxy);

			if (serviceManagement.AuthenticationType !=
				AuthenticationProviderType.ActiveDirectory)
			{
				AuthenticationCredentials tokenCredentials =
					serviceManagement.Authenticate(authCredentials);
				// Obtain discovery/organization service proxy for Federated, LiveId and OnlineFederated environments. 
				// Instantiate a new class of type using the 2 parameter constructor of type IServiceManagement and SecurityTokenResponse.
				return (TProxy)classType
					.GetConstructor(new Type[] { typeof(IServiceManagement<TService>), typeof(SecurityTokenResponse) })
					.Invoke(new object[] { serviceManagement, tokenCredentials.SecurityTokenResponse });
			}

			// Obtain discovery/organization service proxy for ActiveDirectory environment.
			// Instantiate a new class of type using the 2 parameter constructor of type IServiceManagement and ClientCredentials.
			return (TProxy)classType
				.GetConstructor(new Type[] { typeof(IServiceManagement<TService>), typeof(ClientCredentials) })
				.Invoke(new object[] { serviceManagement, authCredentials.ClientCredentials });
		}
		private static AuthenticationCredentials GetCredentials<TService>(IServiceManagement<TService> service, AuthenticationProviderType endpointType
						, string _domain, string _userName, string _password)
		{
			AuthenticationCredentials authCredentials = new AuthenticationCredentials();
			var helper = GlobalContext.Utils.GetActiveDirectoryHelper();

			switch (endpointType)
			{
				case AuthenticationProviderType.ActiveDirectory:
					authCredentials.ClientCredentials.Windows.ClientCredential =
						string.IsNullOrWhiteSpace(_domain) || string.IsNullOrWhiteSpace(_userName) || !helper.Authenticate(_userName, _password, _domain)
						? System.Net.CredentialCache.DefaultNetworkCredentials
						: new System.Net.NetworkCredential(_userName,
							_password,
							_domain);
					break;
				case AuthenticationProviderType.LiveId:
					authCredentials.ClientCredentials.UserName.UserName = _userName;
					authCredentials.ClientCredentials.UserName.Password = _password;
					authCredentials.SupportingCredentials = new AuthenticationCredentials();
					authCredentials.SupportingCredentials.ClientCredentials =
						Microsoft.Crm.Services.Utility.DeviceIdManager.LoadOrRegisterDevice();
					break;
				default: // For Federated and OnlineFederated environments.                    
					authCredentials.ClientCredentials.UserName.UserName = _userName;
					authCredentials.ClientCredentials.UserName.Password = _password;
					// For OnlineFederated single-sign on, you could just use current UserPrincipalName instead of passing user name and password.
					// authCredentials.UserPrincipalName = UserPrincipal.Current.UserPrincipalName;  // Windows Kerberos

					// The service is configured for User Id authentication, but the user might provide Microsoft
					// account credentials. If so, the supporting credentials must contain the device credentials.
					if (endpointType == AuthenticationProviderType.OnlineFederation)
					{
						IdentityProvider provider = service.GetIdentityProvider(authCredentials.ClientCredentials.UserName.UserName);
						if (provider != null && provider.IdentityProviderType == IdentityProviderType.LiveId)
						{
							authCredentials.SupportingCredentials = new AuthenticationCredentials();
							authCredentials.SupportingCredentials.ClientCredentials =
								Microsoft.Crm.Services.Utility.DeviceIdManager.LoadOrRegisterDevice();
						}
					}

					break;
			}

			return authCredentials;
		}

		#endregion

		public static OrganizationServiceProxy TryGetOrganizationServiceProxy(string connectionName = "Xrm")
		{
			connectionName = string.IsNullOrWhiteSpace(connectionName) ? "Xrm" : connectionName;
			try
			{
				return GetOrganizationServiceProxy(connectionName);
			}
			catch (Exception err)
			{
				log.ErrorFormat(
					"Failed to connect to 'Organization Service' using the connection string '{0}'. " +
				"Please fix that this connection string in the configuration file.", connectionName);
			}
			return null;
		}
		/// <summary>
		/// Returns an 'OrganizationServiceProxy' using a conection string.
		/// See https://msdn.microsoft.com/en-us/library/mt608573.aspx for connection string format.
		/// </summary>
		/// <param name="connectionName"> Name of the 'connection string' in application configuration file.</param>
		/// <returns></returns>
		public static OrganizationServiceProxy GetOrganizationServiceProxy(string connectionName = "Xrm")
		{
			OrganizationServiceProxy result = null;
			var config = GlobalContext.Utils.GetConnectionStringManager().GetAll()
				.Where(x => string.Compare(x.Name, connectionName, true) == 0).FirstOrDefault();
			if (config == null)
			{
				throw new ArgumentException(string.Format(
					"Invalid Connection String Name: {0}. Please make sure this connection string exists in the 'app.config' file.", connectionName));
			}
			result = GetOrganizationServiceProxy(
				config.GetValue("Url"), config.GetValue("Domain"), config.GetValue("UserName"), config.GetValue("Password"));
			return result;
		}

		/// <summary>
		/// Returns an 'OrganizationServiceProxy' using a conection string using multiple authentication
		/// methods. While 'GetOrganizationServiceProxy' only supports 'ActiveDirectory', this implementaion
		/// supports 'LiveId and Active Directory Federation'.
		/// See https://msdn.microsoft.com/en-us/library/mt608573.aspx for connection string format.
		/// </summary>
		/// <param name="connectionName"> Name of the 'connection string' in application configuration file.</param>
		/// <returns></returns>
		public static OrganizationServiceProxy GetOrganizationServiceProxyEx(string connectionName = "Xrm")
		{
			OrganizationServiceProxy result = null;
			var config = GlobalContext.Utils.GetConnectionStringManager().GetAll()
				.Where(x => string.Compare(x.Name, connectionName, true) == 0).FirstOrDefault();
			if (config == null)
			{
				throw new ArgumentException(string.Format(
					"Invalid Connection String Name: {0}. Please make sure this connection string exists in the 'app.config' file.", connectionName));
			}
			result = GetOrganizationServiceProxyEx(
				config.GetValue("Url"), config.GetValue("Domain"), config.GetValue("UserName"), config.GetValue("Password"));
			return result;
		}

		/// <summary>
		/// Returns an 'OrganizationServiceProxy'.
		/// </summary>
		/// <param name="serverUrl"></param>
		/// <param name="domain"></param>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static OrganizationServiceProxy GetOrganizationServiceProxy(string serverUrl, string domain, string userName, string password)
		{
			var url = XrmUri.TryCreate(serverUrl, null);
			if (url == null || !url.HasOrganizationName || url.GetOrganizationServiceUri() == null)
			{
				throw new ArgumentException(string.Format(
						"Invalid  Organization Url :'{0}'. Valid 'Organization Urls' include organization name  e.g. http://myserver/myorganization. ", serverUrl));
			}
			var activetDirectoryHelper = GlobalContext.Utils.GetActiveDirectoryHelper();
			domain = string.IsNullOrWhiteSpace(domain)
				? activetDirectoryHelper.GetRootDomainName()
				: domain;
			var clientCredentials = new ClientCredentials();
			clientCredentials.Windows.ClientCredential = !string.IsNullOrWhiteSpace(domain) && !string.IsNullOrWhiteSpace(userName) &&
				!string.IsNullOrWhiteSpace(password) && activetDirectoryHelper.Authenticate(userName, password, domain)
					? new System.Net.NetworkCredential(userName, password, domain)
					: System.Net.CredentialCache.DefaultNetworkCredentials;
			var result = new OrganizationServiceProxy(url.GetOrganizationServiceUri(), null, clientCredentials, null);
			return result;
		}

		/// <summary>
		/// Returns an 'OrganizationServiceProxy using more sophisticated authentication.
		/// While 'GetOrganizationServiceProxy' only supports 'ActiveDirectory. This implemntation
		/// provides other authentication methods.
		/// </summary>
		/// <param name="serverUrl"></param>
		/// <param name="domain"></param>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static OrganizationServiceProxy GetOrganizationServiceProxyEx(string serverUrl, string domain, string userName, string password)
		{
			var uri = XrmUri.TryCreate(serverUrl, null);
			if (uri == null || !uri.HasOrganizationName)
			{
				throw new ArgumentException(string.Format(
					"Invalid  Organization Url :'{0}'. Valid 'Organization Urls' include organization name  e.g. http://myserver/myorganization. ", serverUrl));
			}
			if (GlobalContext.Utils.GetActiveDirectoryHelper().Authenticate(userName, password, domain))
			{
				log.WarnFormat(
					"Invalid User Credentials. We failed to authenticate User: '{0}' on Domain: '{1}'. We will continue to authenticate probably using Current User Credentails."
					, userName, domain);
			}
			IServiceManagement<IOrganizationService> orgServiceManagement =
				ServiceConfigurationFactory.CreateManagement<IOrganizationService>(
				uri.GetOrganizationServiceUri());
			AuthenticationProviderType endpointType = orgServiceManagement.AuthenticationType;
			AuthenticationCredentials credentials = GetCredentials(orgServiceManagement, endpointType, domain, userName, password);
			OrganizationServiceProxy result =
					GetProxy<IOrganizationService, OrganizationServiceProxy>(orgServiceManagement, credentials);
			return result;
		}

		/// <summary>
		/// Returns a 'DiscoveryServiceProxy' for the connection string specified.
		/// </summary>
		/// <param name="connectionName"></param>
		/// <returns></returns>
		public static DiscoveryServiceProxy GetDiscoveryService(string connectionName = "Xrm")
		{
			DiscoveryServiceProxy result = null;
			/// Using the CrmConnection class
			/// we will parse the connection string in the configuration file.
			/// Note that HomeRealmUri && DeviceCredentials are often NULL.
			//  Microsoft.Xrm.Client.CrmConnection con = new Microsoft.Xrm.Client.CrmConnection(connectionName);
			//  var uri = new Uri(con.ServiceUri.AbsoluteUri + "/XRMServices/2011/Organization.svc");
			//  result = new OrganizationServiceProxy(uri, con.HomeRealmUri, con.ClientCredentials, con.DeviceCredentials);
			var config = GlobalContext.Utils.GetConnectionStringManager().GetAll()
				.Where(x => string.Compare(x.Name, connectionName, true) == 0).FirstOrDefault();
			if (config != null)
			{
				result = GetDiscoveryService(
					config.GetValue("Url"), config.GetValue("Domain"), config.GetValue("UserName"), config.GetValue("Password"));
			}
			return result;
		}

		/// <summary>
		/// Returns a 'DiscoveryServiceProxy'.
		/// </summary>
		/// <param name="connectionName"></param>
		/// <returns></returns>

		public static DiscoveryServiceProxy GetDiscoveryService(string serverUrl, string domain, string userName, string password)
		{
			var url = XrmUri.TryCreate(serverUrl, null);
			var activetDirectoryHelper = GlobalContext.Utils.GetActiveDirectoryHelper();
			domain = string.IsNullOrWhiteSpace(domain)
				? activetDirectoryHelper.GetRootDomainName()
				: domain;
			var clientCredentials = new ClientCredentials();
			clientCredentials.Windows.ClientCredential = !string.IsNullOrWhiteSpace(domain) && !string.IsNullOrWhiteSpace(userName) &&
				!string.IsNullOrWhiteSpace(password) && activetDirectoryHelper.Authenticate(userName, password, domain)
					? new System.Net.NetworkCredential(userName, password, domain)
					: System.Net.CredentialCache.DefaultNetworkCredentials;
			var result = new DiscoveryServiceProxy(url.GetDiscoveryServiceUri(), null, clientCredentials, null);
			return result;
		}

		/// <summary>
		/// Returns a 'DiscoveryServiceProxy' for the connection string specified.
		/// Note that this implementation supports more authentication types.
		/// </summary>
		/// <param name="connectionName"></param>
		/// <returns></returns>
		public static DiscoveryServiceProxy GetDiscoveryServiceEx(string connectionName = "Xrm")
		{
			DiscoveryServiceProxy result = null;
			var config = GlobalContext.Utils.GetConnectionStringManager().GetAll()
				.Where(x => string.Compare(x.Name, connectionName, true) == 0).FirstOrDefault();
			if (config != null)
			{
				result = GetDiscoveryServiceEx(
					config.GetValue("Url"), config.GetValue("Domain"), config.GetValue("UserName"), config.GetValue("Password"));
			}
			return result;
		}

		/// <summary>
		/// Returns a 'DiscoveryServiceProxy' for the connection string specified.
		/// Note that this implementation supports more authentication types.
		/// </summary>

		public static DiscoveryServiceProxy GetDiscoveryServiceEx(string serverUrl, string domain, string userName, string password)
		{
			var uri = XrmUri.TryCreate(serverUrl, null);
			if (uri == null || uri.GetDiscoveryServiceUri() == null)
			{
				throw new ArgumentException(string.Format(
					"Invalid Url: {0}. We will not be able to connect to Discovery Services.", serverUrl));
			}

			IServiceManagement<IDiscoveryService> serviceManagement = ServiceConfigurationFactory.CreateManagement<IDiscoveryService>(uri.GetDiscoveryServiceUri());
			AuthenticationProviderType endpointType = serviceManagement.AuthenticationType;
			// Set the credentials.
			AuthenticationCredentials authCredentials = GetCredentials(serviceManagement, endpointType, domain, userName, password);
			DiscoveryServiceProxy discoveryProxy = GetProxy<IDiscoveryService, DiscoveryServiceProxy>(serviceManagement, authCredentials);
			return discoveryProxy;
		}

		public static OrganizationDetailCollection DiscoverOrganizations(
			IDiscoveryService service)
		{
			if (service == null) throw new ArgumentNullException("service");
			RetrieveOrganizationsRequest orgRequest = new RetrieveOrganizationsRequest();
			RetrieveOrganizationsResponse orgResponse =
				(RetrieveOrganizationsResponse)service.Execute(orgRequest);

			return orgResponse.Details;
		}

		public static void Test()
		{
			var ggg = GetOrganizationServiceProxy("Xrm");
			var discovery = GetDiscoveryService("Xrm");

		}
	}

	internal class XrmUri : Uri
	{

		public XrmUri(string path, string organizationName)
			: base(path)
		{
			this.OrganizationName = !string.IsNullOrWhiteSpace(organizationName)
				? organizationName
				: GetOrganizationNameFromUri(this);
		}

		public string OrganizationName { get; private set; }
		public bool HasOrganizationName { get { return !string.IsNullOrEmpty(OrganizationName); } }


		public static string GetOrganizationNameFromUri(Uri uri)
		{
			return uri == null
				? null
				: uri.Segments.Length < 2
					? null
					: string.IsNullOrEmpty(uri.Segments[1])
						? null
						: uri.Segments[1].ToLowerInvariant().StartsWith("xrmservices")
							? null
							: uri.Segments[1].ToLowerInvariant().EndsWith("/")
								? uri.Segments[1].Substring(0, uri.Segments[1].Length - 1)
								: uri.Segments[1];
		}

		public XrmUri GetOrganizationServiceUri()
		{
			var str = string.Format("{0}://{1}/{2}/XRMServices/2011/Organization.svc", this.Scheme, this.Host, this.OrganizationName);
			return new XrmUri(str, null);

		}
		public XrmUri GetDiscoveryServiceUri()
		{
			var str = string.Format("{0}://{1}/XRMServices/2011/Discovery.svc", this.Scheme, this.Host);
			return new XrmUri(str, OrganizationName);
		}
		public static XrmUri TryCreate(string path, string organizationName)
		{
			try
			{
				return new XrmUri(path, organizationName);
			}
			catch { }
			return null;

		}
	}

}
