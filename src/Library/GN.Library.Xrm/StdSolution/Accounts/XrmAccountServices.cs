using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm.StdSolution.Accounts
{
	public class XrmAccountServices : XrmEntityService<XrmAccount>
	{
		public XrmAccountServices(XrmAccount account) : base(account) { }
		public XrmAccountServices(IAppContext ctx)
		{

		}

		public void Test()
		{
			var aa = This.Name;
		}
		public void Deactivate()
		{
			
		}
	}
}
