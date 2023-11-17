using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm
{
	public class EntityReferenceEx
	{
		public Guid Id { get; private set; }
		public string LogicalName { get; private set; }
		public string Name { get; private set; }

		public EntityReferenceEx(Guid id, string logicalName)
		{
			this.Id = id;
			this.LogicalName = logicalName;
		}
		public EntityReferenceEx(EntityReference  reference)
		{
			if (reference != null)
			{
				this.Id = reference.Id;
				this.Name = reference.Name;
				LogicalName = reference.LogicalName;
			}
		}
		public EntityReference ToEntityReference()
		{
			return new EntityReference(this.LogicalName, this.Id);
		}
		
	}
}
