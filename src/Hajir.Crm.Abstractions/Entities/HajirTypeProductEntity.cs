using GN.Library.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Entities
{
	public class HajirTypeProductEntity : DynamicEntity
	{
		public new class Schema : DynamicEntity.Schema
		{
			public const string SolutionPerfix = HajirCrmConstants.RahsamSolutionPerfix;
			public const string LogicalName = SolutionPerfix + "typeproduct";
			public const string HajirTypeProductId = LogicalName + "id";
		}
	}
}
