using GN.Library.Xrm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Hajir.Crm.Xrm.Tests
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			var cc = new XrmConnectionString("Url=http://192.168.20.15:5555/hajircrm;UserName=CRMIMPU01;Password=%H@ZH!r_1402&$;Domain=hsco");



			var o = new GN.Library.Xrm.XrmOrganizationService(cc);
			o.TestConnection();
			var q = o.CreateQuery("systemuser");
			var contacts = q.Take(10).ToArray();
			
		}
	}
}