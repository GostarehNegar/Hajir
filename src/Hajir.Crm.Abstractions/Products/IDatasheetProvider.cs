using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Products
{
    public interface IDatasheetProvider
    {
        Task<Datasheet[]> GetDatasheets();
    }
}
