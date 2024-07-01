using GN.Library.Odoo.Internal.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Odoo
{
	public class OdooField : RpcField
	{
		public OdooField Clone()
		{
			return new OdooField
			{
				FieldName = this.FieldName,
				Value = null,
				Changed = true,
				Help = this.Help,
				String = this.String,
				Type = this.Type
			};
		}
	}
}
