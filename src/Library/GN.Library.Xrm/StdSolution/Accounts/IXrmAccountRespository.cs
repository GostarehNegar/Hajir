using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm.StdSolution.Accounts
{
	public interface IXrmAccountRespository : IXrmRepository<XrmAccount>
	{
	}

	class XrmAccountRepository : XrmRepository<XrmAccount>, IXrmAccountRespository
	{
		public XrmAccountRepository(IXrmDataServices ctx) : base(ctx)
		{

		}
	}
}
