using GN.Library.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Entities
{
	public class HajirProductEntity : DynamicEntity
	{
		public new class Schema : DynamicEntity.Schema
		{
			public const int BaseOptionSetIndex = 130770000;
			public const string RHS_SolutionPerfix = "rhs";
			public const string LogicalName = "product";
			public const string ProductId = LogicalName + "id";
			public const string Name = "name";
			public const string ProductNumber = "productnumber";
			public const string ProductType = RHS_SolutionPerfix + "producttype";
			public const string TypeProducts_deprecated = "rhs_typeproducts";
			public const string SupportedBatteries = "rhs_supportedbattries";
			public const string NumberOfFloors = "rhs_numberoffloors";


            public enum ProductTypes
			{
				UPS = BaseOptionSetIndex,
				Stabilizer = BaseOptionSetIndex + 1,
				Battery = BaseOptionSetIndex + 2,
				Cabinet = BaseOptionSetIndex + 3,
				Inverter = BaseOptionSetIndex + 4,
				Battery_Pack = BaseOptionSetIndex + 5,
				Switch_ATS = BaseOptionSetIndex + 6,
				SMP_Card = BaseOptionSetIndex + 7,
				Genrator = BaseOptionSetIndex + 8,
				Parallel_Card = BaseOptionSetIndex + 9,
				Battery_Connector = BaseOptionSetIndex + 10,
				Other = BaseOptionSetIndex + 11,
			}
		}
		public string ProductNumber { get => this.GetAttributeValue<string>(Schema.ProductNumber); set => this.SetAttributeValue(Schema.ProductNumber, value); }



	}
}
