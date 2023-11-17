// Decompiled with JetBrains decompiler
// Type: SS.Crm.Linq.CrmQueryable`1
// Assembly: SS.Crm.Linq, Version=1.0.6939.26200, Culture=neutral, PublicKeyToken=1eea6d0e8f401bee
// MVID: CA16C0DC-D52F-484E-A539-505517B5F7DC
// Assembly location: C:\Users\babak\Desktop\SS.Crm.Linq.dll

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;
using System.Linq;
using System.Linq.Expressions;

namespace SS.Crm.Linq
{
  public class CrmQueryable<T> : QueryableBase<T>, ICrmQueryable
  {
    private string _entityLogicalName;

    public CrmQueryable(IOrganizationService service, string entityLogicalName = null, ColumnSet columns = null, bool retrieveAllRecords = false)
      : base((IQueryParser) QueryParser.CreateDefault(), CrmQueryable<T>.CreateExecutor(service, entityLogicalName, columns, retrieveAllRecords))
    {
      this._entityLogicalName = entityLogicalName;
      if (!string.IsNullOrEmpty(this._entityLogicalName))
        return;
      this._entityLogicalName = typeof (T).GetEntityLogicalName();
    }

    public CrmQueryable(IQueryProvider provider, Expression expression)
      : base(provider, expression)
    {
    }

    private static IQueryExecutor CreateExecutor(IOrganizationService service, string entityLogicalName = null, ColumnSet columns = null, bool retrieveAllRecords = false)
    {
      return (IQueryExecutor) new CrmQueryExecutor(service, entityLogicalName, columns, retrieveAllRecords);
    }

    public string EntityLogicalName
    {
      get
      {
        return this._entityLogicalName;
      }
    }
  }
}
