using System;
using System.Linq;

namespace GN.Library.Xrm
{
    /// <summary>
    /// DbContext that can be used to query CRM databse through an sql connection.
    /// Additional entities can be added using 'AddEntity'.
    /// </summary>
    public interface IXrmDbContext :  IDisposable 
    {
        IXrmDbContext AddEntity<T>(bool includeRelatedEntities = false) where T : XrmEntity;
        IQueryable<T> Query<T>() where T : XrmEntity;
        bool Has<T>();

    }

}
