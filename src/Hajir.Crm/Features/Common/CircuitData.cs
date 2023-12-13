using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Hajir.Crm.Features.Common
{
    public class CircuitData
    {
        public string CircuitId { get; set; }
        public IServiceProvider ServiceProvider { get; set; }
        //public User User => ServiceProvider.GetCurrentUserService().User;
    }
}
