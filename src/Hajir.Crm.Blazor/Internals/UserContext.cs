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

        public CurrentUser CurrentUser(CurrentUser user = null)
        {
            if (user != null)
            {
                this.Data["current_user"] = user;
            }
            return this.Data.TryGetValue("current_user", out var _res) && _res is CurrentUser 
                ? (_res as CurrentUser)
                :null;
        }
    }
    public class UserContextContainer
    {
        public UserContext Context { get; set; } = new UserContext();
    }
}
