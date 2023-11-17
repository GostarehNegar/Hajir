using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.StdSolution
{
	public class XrmFax : XrmActivity<XrmLetter, DefaultStateCodes, DefaultStatusCodes>
	{
		public new class Schema : XrmActivity<XrmLetter, DefaultStateCodes, DefaultStatusCodes>.Schema
		{
			public const string LogicalName = "fax";
		}

		public XrmFax() : base(Schema.LogicalName)
		{

		}
	}
	public class XrmFaxRepository : XrmRepository<XrmFax>
	{
		public XrmFaxRepository(IXrmDataServices dataContext) : base(dataContext)
		{

		}
	}
}
