using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.ViewModels
{
    public class AlertModel
    {
        public string Message { get; set; }
        public bool HasMessage => Message != null;
        public void Clear()=> Message = null;
    }
}
