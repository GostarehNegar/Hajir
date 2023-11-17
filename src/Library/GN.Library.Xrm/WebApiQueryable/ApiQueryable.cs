using Microsoft.Xrm.Sdk.Query;
using SS.Crm.Linq.Proxies;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.WebApiQueryable
{
    class ApiQueryable<T> : CrmQueryableProxy<T>
    {
        public ApiQueryable(IFetchXmlExecutor dataContext, XrmQueryOptions options, string entityLogicalName = null, ColumnSet columnSet = null, bool retrieveAllRecords = false) :
            base(dataContext, new OrganizationServiceProxy(), options, entityLogicalName, columnSet, retrieveAllRecords)
        {

        }
    }
}
