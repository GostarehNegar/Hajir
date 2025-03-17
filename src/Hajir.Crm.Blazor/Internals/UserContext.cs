using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Internals
{
    public class UserContext
    {
        public Dictionary<string, object> Data = new Dictionary<string, object>();
    }
    public class UserContextContainer
    {
        public UserContext Context { get; set; }
    }
}
