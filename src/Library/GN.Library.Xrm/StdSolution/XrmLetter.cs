using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.StdSolution
{
	public class XrmLetter : XrmActivity<XrmLetter, DefaultStateCodes, DefaultStatusCodes>
	{
		public new class Schema : XrmActivity<XrmLetter, DefaultStateCodes, DefaultStatusCodes>.Schema
		{
			public const string LogicalName = "letter";
		}

		public XrmLetter() : base(Schema.LogicalName)
		{

		}
	}
	public class XrmLetterRepository : XrmRepository<XrmLetter>
	{
		public XrmLetterRepository(IXrmDataServices dataContext) : base(dataContext)
		{

		}
	}


}
