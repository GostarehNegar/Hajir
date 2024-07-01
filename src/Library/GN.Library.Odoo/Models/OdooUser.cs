using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using GN.Library.Data;

namespace GN.Library.Odoo.Models
{
	[OdooModel(Schema.LogicalName)]
	public class OdooUser: OdooEntity<OdooUser>
	{
		public new class Schema
		{
			public const string LogicalName = "res.users";
			public const string Name = "name";
			public const string Login = "login";

		}
		public OdooUser() : base(Schema.LogicalName) { }

		[OdooColumn(Schema.Name)]
		public string Name { get => this.GetAttributeValue<string>(Schema.Name); }

		[OdooColumn(Schema.Login)]
		public string Login { get => this.GetAttributeValue<string>(Schema.Login); set { } }

		
	}
	public static class OdooUserExtenstions
	{

		public static T GetByLoginName<T>(this IOdooQueryable<T> query, string loginName) where T:OdooUser
		{
			return query.Execute(q => q.Filter.ILike(OdooUser.Schema.Login, loginName)).FirstOrDefault();
			

			
		}
	}
}
