using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm.WebCommands
{
	public interface IEntityCommandHandler
	{
		Task Handle(string command, XrmEntity entity);
	}
	class EntityCommandHandler : IEntityCommandHandler
	{
		public Task Handle(string command, XrmEntity entity)
		{
			return Task.FromResult(true);
		}
	}
}
