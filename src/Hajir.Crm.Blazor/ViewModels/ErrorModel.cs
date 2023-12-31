using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.ViewModels
{
    public class ErrorModel
    {
        public Exception Error { get;set; }
        public bool HasError=> Error!=null;
        public void Clear()=> this.Error = null;
    }
}
