using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Odoo.Models
{
	[OdooModel(Schema.ModelName)]
	public class OdooAccountAccount : OdooEntity
	{
		public new class Schema : OdooEntity.Schema
		{
			public const string ModelName = "account.account";
			public const string code = "code";
			public const string name = "name";
		}
		public OdooAccountAccount() : base(Schema.ModelName) { }
	}

	public static class OdooAccountAccountExtensions
	{

		public static IEnumerable<T> GetByCode<T>(this IOdooQueryable<T> repo, string code, params string[] columns) where T : OdooAccountAccount
		{
			return repo.Execute(q => {
				q.Filter.Like(OdooAccountAccount.Schema.code, code);
				q.AddField(columns);
			});

		}
	}
}
