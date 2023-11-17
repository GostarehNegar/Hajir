using System;
using System.Collections.Generic;
using System.Text;
using GN.Library.Contracts_Deprecated;

namespace GN.Library.Xrm.Services.Bus
{
	class SystemFunctions
	{
		public static XrmSystemMessageReplyModel Add(XrmSystemMessageModel model)
		{
			var reply = model.CreateReply();
			reply.StrResult = model.Str1 + model.Str2;
			reply.IntResult = model.Int1 + model.Int2;
			reply.FloatResult = model.Float1 + model.Float2;
			reply.DecimalResult = model.Decimal1 + model.Decimal2;
			reply.Failed = false;
			return reply;
		}
	}
}
