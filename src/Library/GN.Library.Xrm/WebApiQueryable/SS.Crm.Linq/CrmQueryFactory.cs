// Decompiled with JetBrains decompiler
// Type: SS.Crm.Linq.CrmQueryFactory
// Assembly: SS.Crm.Linq, Version=1.0.6939.26200, Culture=neutral, PublicKeyToken=1eea6d0e8f401bee
// MVID: CA16C0DC-D52F-484E-A539-505517B5F7DC
// Assembly location: C:\Users\babak\Desktop\SS.Crm.Linq.dll

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace SS.Crm.Linq
{
  public class CrmQueryFactory
  {
    public static CrmQueryable<T> Queryable<T>(IOrganizationService service, ColumnSet columns = null, bool retrieveAllRecords = false)
    {
      return new CrmQueryable<T>(service, (string) null, columns, retrieveAllRecords);
    }

    public static CrmQueryable<Entity> Queryable(IOrganizationService service, string entityLogicalName, ColumnSet columns = null, bool retrieveAllRecords = false)
    {
      return new CrmQueryable<Entity>(service, entityLogicalName, columns, retrieveAllRecords);
    }
  }
}
