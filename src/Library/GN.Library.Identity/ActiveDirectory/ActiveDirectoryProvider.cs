using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.DirectoryServices;
using GN.Library.Shared.Internals;

namespace GN.Library.Identity.ActiveDirectory
{
	class ActiveDirectoryProvider : IAuthenticationProvider, IUserIdentityProvider
	{
		private readonly ILogger logger;
		private readonly ActiveDirectoryOptions options;
		private readonly IMemoryCache cache;
		private static string domain_names_cache = "active-directory-domain-names";
		private static string users_cacche = "active-directory-users";

		public ActiveDirectoryProvider(ILogger<ActiveDirectoryProvider> logger, ActiveDirectoryOptions options, IMemoryCache cache)
		{
			this.logger = logger;
			this.options = options;
			this.cache = cache;
		}
		internal IEnumerable<Domain> GetAllDomains()
		{
			IEnumerable<Domain> result = new List<Domain>();
			if (!cache.TryGetValue<IEnumerable<Domain>>(domain_names_cache, out result))
			{
				try
				{
					result = ActiveDirectoryExtensions.GetAllDOmains(this.options.AdminUserName, this.options.AdminPassword);
					cache.Set<IEnumerable<Domain>>(domain_names_cache, result, TimeSpan.FromMinutes(60));
				}
				catch (Exception err)
				{
					this.logger.LogError(
						$"An error occured while trying to retrive 'active directory domains' using options:{this.options}. Err:{err.Message} ");
					throw;
				}
			}
			return result ?? new List<Domain>();
		}
		private Domain GetDefautlDomain()
		{
			return GetAllDomains().FirstOrDefault();

		}
		private static bool DomainMatches(Domain domain, string domainName)
		{
			return domain != null && domainName != null && (
				domain.Name?.ToLowerInvariant() == domainName?.ToLowerInvariant() || domain.Name.ToLowerInvariant().StartsWith(domainName.ToLowerInvariant()));
		}
		public async Task<bool> Authenticate(string userName, string password)
		{
			var result = await Task.FromResult(false);
			try
			{
				var normalized_user_name = ActiveDirectoryExtensions.NormalizeUserName(userName);
				var domain = this.GetAllDomains()
					.FirstOrDefault(d => DomainMatches(d, normalized_user_name.DomianName));
				//if (userName == "admin@gnco.ir")
				//	return true;

				if (domain != null)
				{
					var controllerName = domain.DomainControllers.Count > 0 ? domain.DomainControllers[0].Name : null;
					result = ActiveDirectoryExtensions.ValidateUser(domain.Name, controllerName, userName, password);
				}
				else if (!string.IsNullOrWhiteSpace(normalized_user_name.DomianName))
				{
					/// Maybe we should try here with supplied domain name
					/// even though we have not found that domain!!!
					/// But it may have some performance issues since invalid domain names 
					/// will cause long searching delays...

				}

			}
			catch (Exception err)
			{
				this.logger.LogError(
					$"An error occured while trying to authentivate a user with active directory. UserName:'{userName}'. Error:{err.Message}");
			}

			return result;


		}
		private IEnumerable<ActiveDirectoryIdentity> DoGetAllUsers()
		{
			var result = new List<ActiveDirectoryIdentity>();
			foreach (var domain in this.GetAllDomains())
			{
				try
				{
					var propNames = this.options.PropertyNames.Split(',');
					foreach (var _user in ActiveDirectoryExtensions.SearchUsers(domain, propNames))
					{

						var user = _user as SearchResult;

						try
						{
							if (user != null)
							{
								var identity = new ActiveDirectoryIdentity { };

								//identity.DomainName = domain.Name;
								foreach (var _propName in user.Properties.PropertyNames)
								{
									try
									{

										if (_propName != null)
										{
											var propName = _propName.ToString();
											if (user.Properties.Contains(propName))
											{
												var values = user.Properties[propName];

												if (values.Count == 1)
												{
													identity.Attributes.Add(propName, values[0]);
												}
												else if (values.Count > 1)
												{
													var comma_seperated_values = "";
													foreach (var value in values)
													{
														if (value != null)
														{
															comma_seperated_values = comma_seperated_values + (comma_seperated_values == "" ? "" : ";") + value?.ToString();
														}
													}

													identity.Attributes.Add(propName, comma_seperated_values);
												}
												else
												{
													identity.Attributes.Add(propName, null);
												}
											}
										}
									}
									catch (Exception err)
									{
										this.logger.LogWarning(
											$"An error occured while trying to load active directory property :{_propName?.ToString()}, Err:{err.Message}");
									}
								}
								result.Add(identity);
							}
						}
						catch (Exception err)
						{
							this.logger.LogWarning(
								$"An error occured while trying to load user :{user?.Path}, Err:{err.Message}");

						}

					}
				}
				catch (Exception err)
				{
					this.logger.LogWarning(
						$"An error occured while trying to search users in this domain:{domain.Name}, Err:{err.Message}");
				}
			}
			//var serach = ActiveDirectoryExtensions.SearchUsers(this.GetAllDomains().ToArray()[0]);

			return result;

		}
		private object _lock = new object();
		private IEnumerable<UserIdentityEntity> GetAllUsers()
		{
			IEnumerable<UserIdentityEntity> result = new List<UserIdentityEntity>();
			if (!cache.TryGetValue<IEnumerable<UserIdentityEntity>>(domain_names_cache, out result))
			{
				try
				{
					lock (_lock)
					{
						if (!cache.TryGetValue<IEnumerable<UserIdentityEntity>>(domain_names_cache, out result))
						{
							result = this.DoGetAllUsers().Select(x => x.ToIdentityUser()).ToArray();
							if (result.Count() > 0)
								cache.Set<IEnumerable<UserIdentityEntity>>(domain_names_cache, result, TimeSpan.FromMinutes(60));
						}
					}
				}
				catch (Exception err)
				{
					this.logger.LogError(
						$"An error occured while trying to retrive 'active directory domains' using options:{this.options}. Err:{err.Message} ");
					throw;
				}
			}
			return result;
		}

		private bool Matches (UserIdentityEntity user, string userName)
		{
			return user != null && string.Compare(user.UserName, userName, true) == 0;
		}
		public async Task<UserIdentityEntity> LoadUser(string userName)
		{
			var result = await Task.FromResult<UserIdentityEntity>(null);
			try
			{
				if (userName == "admin@gnco.ir")
				{
					result = new UserIdentityEntity
					{
						UserName = "admin@gnco.ir",
						Email = "admin@gnco.ir",
						Title = "admin",
						//GroupNames = new string[] { }
					};
					return result;
					

				}
				var user = ActiveDirectoryExtensions.NormalizeUserName(userName);
				var domain = this.GetAllDomains()
					.FirstOrDefault(d => DomainMatches(d, user.DomianName)) ?? GetDefautlDomain();
				var normal_user_name = $"{user.UserName}@{domain.Name}";
				var users = GetAllUsers();
				result = users
					.Where(x => !x.IsDisabled)
					.ToList()
					.FirstOrDefault(u => Matches(u, normal_user_name));
			}
			catch (Exception err)
			{
				this.logger.LogError(
					$"An error occured while trying to load user. Err:{err.Message}");
			}
			return result;
		}

		public async Task<IEnumerable<UserIdentityEntity>> FindByIpPhone(string ipPhone, bool includeDisabledUsers = false)
		{
			IEnumerable<UserIdentityEntity> result = await Task.FromResult(new List<UserIdentityEntity>());
			try
			{
				result = this.GetAllUsers()
					.Where(x => (!x.IsDisabled || includeDisabledUsers) && x.IpPhoneExtension == ipPhone)
					.ToArray();
			}
			catch (Exception err)
			{
				this.logger.LogError(
					$"An error occured while trying to 'FindByIpPhone' users. Err:{err.Message}");
				throw;
			}
			return result;
		}

		public Task<IEnumerable<UserIdentityEntity>> FindByEmail(string ipPhone, bool includeDisabledUsers = false)
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<UserIdentityEntity>> FindAll(bool includeDisabledUsers = false)
		{
			IEnumerable<UserIdentityEntity> result = await Task.FromResult(new List<UserIdentityEntity>());
			try
			{
				result = this.GetAllUsers()
					.Where(x => !x.IsDisabled || includeDisabledUsers)
					.ToArray();
			}
			catch (Exception err)
			{
				this.logger.LogError(
					$"An error occured while trying to 'FindAll' users. Err:{err.Message}");
				throw;
			}
			return result;
		}
	}
}
