using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Odoo
{
	[AttributeUsage(AttributeTargets.Class)]
	public class OdooModelAttribute : Attribute
	{
		public string Name { get; set; }
		public OdooModelAttribute(string Name)
		{
			this.Name = Name;
		}
	}
	[AttributeUsage(AttributeTargets.Property)]
	public class OdooColumnAttribute:Attribute
	{
		public string Name { get; set;		}
		public OdooColumnAttribute(string Name)
		{
			this.Name = Name;
		}


	}
}
