
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Text;
using System.Linq;

namespace GN.Library.Authorization.ActiveDirectory
{
	public class ActiveDirectoryHelper_Deprecated
	{
		public ActiveDirectoryHelper_Deprecated()
		{

		}

		public static List<ActiveDirectoryUser> find(Domain domain)
		{
			var result = new List<ActiveDirectoryUser>();
			DirectoryEntry searchRoot = domain.GetDirectoryEntry();
			DirectorySearcher search = new DirectorySearcher(searchRoot);
			search.Filter = "(&(objectClass=user)(objectCategory=person))";
			search.PropertiesToLoad.Add("samaccountname");
			search.PropertiesToLoad.Add("mail");
			search.PropertiesToLoad.Add("mobile");
			search.PropertiesToLoad.Add("usergroup");
			search.PropertiesToLoad.Add("displayname");//first name
			search.PropertiesToLoad.Add("Title");
			search.PropertiesToLoad.Add("department");
			SearchResult _result;
			SearchResultCollection resultCol = search.FindAll();
			if (resultCol != null)
			{
				for (int counter = 0; counter < resultCol.Count; counter++)
				{
					{
						string UserNameEmailString = string.Empty;
						_result = resultCol[counter];
						if (_result.Properties.Contains("samaccountname") &&
							//_result.Properties.Contains("mail") &&
							//_result.Properties.Contains("mobile")&&
							_result.Properties.Contains("displayname"))
						{
							ActiveDirectoryUser objSurveyUsers = new ActiveDirectoryUser();
							//objSurveyUsers.directoryentery = _result.GetDirectoryEntry();
							//objSurveyUsers.path = _result.Path;
							//objSurveyUsers.DomainName = domain.Name;
							if (_result.Properties.Contains("mail"))
							{
								//objSurveyUsers.Email = (String)_result.Properties["mail"][0];
							}
							if (_result.Properties.Contains("mobile"))
							{
								//objSurveyUsers.mobile = (String)_result.Properties["mobile"][0];
							}
							if (_result.Properties.Contains("department"))
							{
								//objSurveyUsers.department = (String)_result.Properties["department"][0];
							}
							if (_result.Properties.Contains("title"))
							{
								// objSurveyUsers.title = (String)_result.Properties["title"][0];
							}

							//objSurveyUsers.UserName = (String)_result.Properties["samaccountname"][0];
							//objSurveyUsers.DisplayName = (String)_result.Properties["displayname"][0];
							result.Add(objSurveyUsers);
						}
					}
				}

			}

			return result;
		}
		public static string FixUserName(string userName, string domainName = null)
		{
			domainName = domainName;// ?? SysOptions.Current.DomainName;
			if (!string.IsNullOrWhiteSpace(userName))
			{
				if (userName.IndexOf('\\') > -1)
				{
					userName = domainName + userName.Substring(userName.IndexOf('\\'), userName.Length - userName.IndexOf("\\"));
				}
				else
				{
					userName = domainName + "\\" + userName;
				}
			}
			return userName;
		}
		public static List<ActiveDirectoryUser> GetAllUsers(string AdminUser, string AdminPasswprd)
		{
			var result = new List<ActiveDirectoryUser>();
			DirectoryContext ctx = new DirectoryContext(DirectoryContextType.Forest, AdminUser, AdminPasswprd);
			var forest = Forest.GetForest(ctx);
			var domains = forest.Domains;
			for (var i = 0; i < domains.Count; i++)
			{
				var domainusers = find(domains[i]);
				result.AddRange(domainusers);
			}
			return result;
		}
		public static IEnumerable<Domain> GetAllDOmains(string userName, string password)
		{
			List<Domain> result = new List<Domain>();
			DirectoryContext ctx = new DirectoryContext(DirectoryContextType.Forest, userName, password);
			var forest = Forest.GetForest(ctx);
			var domains = forest.Domains;
			for (var i = 0; i < domains.Count; i++)
			{
				result.Add(domains[i]);
			}
			return result;
		}
		public static bool ValidateUser(string domainName,string ldapserver, string userName, string pass)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(pass))
					return false;
				using (var pc = new PrincipalContext(ContextType.Domain, ldapserver))
				{
					//userName = userName.IndexOf("\\") < 0
					//	? domainName + "\\" + userName
					//	: userName;
					//userName = FixUserName(userName, domainName);
					var result = pc.ValidateCredentials(userName, pass, ContextOptions.SimpleBind);
					if (!result)
					{
						//Logger?.LogWarning(
						//	"userName:{0}, pass:{1}", userName, pass);
					}


					return result;
				}
			}
			catch (Exception err)
			{
				//Logger?.LogError(
				//	"An error occured while trying to authenticate user. Err: '{0}', DomainName:'{1}' ", err.GetBaseException().Message, domainName);
			}
			return false;
		}

	}
}
