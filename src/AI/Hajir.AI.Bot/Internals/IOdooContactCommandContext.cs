using GN.Library.Odoo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GN.Portal.Crm.Data.Odoo.Internals
{
	public interface IOdooContactCommandContext 
	{
		OdooContact NewContact();
		OdooAccount GetOdooAccountById(int id);
		OdooContact GetOdooContactById(int id);
		OdooContact GetOdooContactSynchData(string data);
		OdooAccount GetOdooAccountBySynchData(string data);
		OdooAccount NewAccount();
		OdooAccount GetMissingPartner();
		void UpsertAccount(OdooAccount account);
		IOdooConnection GetConnetion(bool refresh=false);
	}

	class OdooContactCommandContext : IOdooContactCommandContext
	{

		private IOdooConnection connection;
		private readonly IOdooConnectionManager connectionManager;
		public OdooContactCommandContext(IOdooConnectionManager connectionManager)
		{
			this.connectionManager = connectionManager;
		}

		private int? GetId(object id)
		{
			if (id == null)
				return null;
			if (id.GetType() == typeof(int))
				return (int)id;
			if (id.GetType() == typeof(int?))
				return (int?)id;
			return null;
		}

		public IOdooConnection GetConnetion(bool refersh = false)
		{
			if (this.connection == null || refersh)
			{
				this.connection = this.connectionManager.GetDefaultConnection(true);
				//this.connection = this.connectionManager.CreateConnection(new OdooConnectionString("")
				//{
				//	Url = settings.ServerUrl,
				//	DbName = settings.DbName,
				//	Password = settings.Password,
				//	UserName = settings.UserName,
				//	ServerCertificateValidation = true
				//});
			}
			return this.connection;
		}
		public bool AccountExists(object id)
		{
			var _id = this.GetId(id);

			return _id.HasValue && _id.Value > -1
				? this.GetConnetion().CreateQuery<OdooAccount>().Execute(q =>

				{
					q.Filter.Equal("id", _id.Value);
				}, limit: 1).Count() > 0
				: false;

		}

		public bool ContactExists(object id)
		{
			var _id = this.GetId(id);

			return _id.HasValue && _id.Value > -1
				? this.GetConnetion().CreateQuery<OdooContact>().Execute(q =>

				{
					q.Filter.Equal("id", _id.Value);
				}, limit: 1).Count() > 0
				: false;

		}

		//public IAccountDataModel CreateOrUpdateAccount(IAccountDataModel account)
		//{
		//	OdooAccount result = account.IntId > 0
		//					? this.GetConnetion().CreateQuery<OdooAccount>().GetById(account.IntId) ?? this.GetConnetion().NewAccount()
		//					: this.GetConnetion().NewAccount();
		//	if (result == null)
		//	{
		//		throw new Exception($"Failed to get or create odoo account");
		//	}
		//	try
		//	{
		//		if (result.Update(account))
		//			result.Save();
		//	}
		//	catch
		//	{
		//		var fff = 1;
		//		throw;
		//	}
		//	return result?.ToAccount();
		//}

		//public IContactDataModel CreateOrUpdateContact(IContactDataModel contact)
		//{
		//	OdooContact result = contact.IntId > 0
		//		? this.GetConnetion().CreateQuery<OdooContact>().GetById(contact.IntId) ?? this.GetConnetion().NewContact()
		//		: this.GetConnetion().NewContact();
		//	if (result == null)
		//	{

		//	}
		//	result.Update(contact);
		//	result.Save();
		//	return result;



		//}

		public void DeleteAccount(object id)
		{
			var _id = this.GetId(id);
			if (_id.HasValue && _id.Value > -1)
			{
				this.GetConnetion().GetRpcConnection().Remove(OdooAccount.Schema.LogicalName, new int[] { _id.Value });
			}
		}

		public void DeleteContact(object id)
		{
			var _id = this.GetId(id);
			if (_id.HasValue && _id.Value > -1)
			{
				this.GetConnetion().GetRpcConnection().Remove(OdooContact.Schema.LogicalName, new int[] { _id.Value });
			}
		}

		//public IAccountDataModel GetAccountById(object id)
		//{
		//	var _id = this.GetId(id);
		//	return _id == null || _id < 0
		//		? null
		//		: this.GetConnetion().CreateQuery<OdooAccount>().GetById(_id.Value);
		//}

		//public IContactDataModel GetContactById(object id)
		//{
		//	var _id = this.GetId(id);
		//	return _id == null || _id < 0
		//		? null
		//		: this.GetConnetion().CreateQuery<OdooContact>().GetById(_id.Value);// ToContact();

		//}

		//public IContactDataModel LoadContact(object id)
		//{
		//	throw new NotImplementedException();
		//}

		public OdooContact NewContact()
		{
			return this.GetConnetion().NewContact();
		}

		public OdooAccount GetOdooAccountById(int id)
		{
			return id > 0
				? this.GetConnetion()
					.CreateQuery<OdooAccount>()
					.GetById(id)
				: null;

		}

		public OdooAccount NewAccount()
		{
			return this.GetConnetion().NewAccount();
		}

		public void UpsertAccount(OdooAccount account)
		{
			account.Save();
		}

		public OdooContact GetOdooContactById(int id)
		{
			return id > 0
				? this.GetConnetion()
					.CreateQuery<OdooContact>()
					.GetById(id)
				: null;

		}

		public OdooAccount GetOdooAccountBySynchData(string data)
		{
			return string.IsNullOrWhiteSpace(data) || data.Length < 3
				? null
				: this.GetConnetion().CreateQuery<OdooAccount>().GetBySynchData(data);

		}

		public OdooContact GetOdooContactSynchData(string data)
		{
			var result = string.IsNullOrWhiteSpace(data) || data.Length < 3
				? null
				: this.GetConnetion().CreateQuery<OdooContact>().GetBySynchData(data);
			return result;
		}

		public OdooAccount GetMissingPartner()
		{
			return this.GetConnetion()
				.CreateQuery<OdooAccount>()
				.GetMissingPartnerAccount();
		}
	}
}
