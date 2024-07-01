using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Odoo.Models
{
	[OdooModel(Schema.ModelName)]
	public class OdooJournal:OdooEntity
	{
		public new class Schema : OdooEntity.Schema
		{
			public const string ModelName = "account.journal";
			public const string name = "name";
			public const string code = "code";
			public const string type = "type";
			public const string default_credit_account_id = "default_credit_account_id";
			public const string default_debit_account_id = "default_debit_account_id";
			public const string company_id = "company_id";

			public enum TypeCodes
			{
				sale,
				purchase,
				cash,
				bank,
				general
			}

		}
		public OdooJournal() : base(Schema.ModelName)
		{

		}
		[OdooColumn(Schema.name)]
		public string Name { get => this.GetAttributeValue<string>(Schema.name); set => this.SetAttributeValue(Schema.name, value); }
		[OdooColumn(Schema.code)]
		public string Code { get => this.GetAttributeValue<string>(Schema.code); set => this.SetAttributeValue(Schema.code, value); }

		[OdooColumn(Schema.type)]
		public string Type => this.GetAttributeValue<string>(Schema.type);

		[OdooColumn(Schema.default_credit_account_id)]
		public int? default_credit_account_id => this.GetAttributeValue<int?>(Schema.default_credit_account_id);

		[OdooColumn(Schema.default_debit_account_id)]
		public int? default_debit_account_id => this.GetAttributeValue<int?>(Schema.default_credit_account_id);

		[OdooColumn(Schema.company_id)]
		public int? Company_Id { get => this.GetAttributeValue<int?>(Schema.company_id); set => this.SetAttributeValue(Schema.company_id, value); }



	}

	public static class OdooJournalExtensions
	{
		public static IEnumerable<T> GetHournalsByType<T>(this IOdooQueryable<T> repo, OdooJournal.Schema.TypeCodes typeCode) where T : OdooJournal
		{
			return repo.Execute(q =>
			{
				q.Filter.Equal(OdooJournal.Schema.type, typeCode.ToString());
			});
		}
	}
}
