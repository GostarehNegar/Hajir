// Decompiled with JetBrains decompiler
// Type: SS.Crm.Linq.PublicExtensionMethods
// Assembly: SS.Crm.Linq, Version=1.0.6939.26200, Culture=neutral, PublicKeyToken=1eea6d0e8f401bee
// MVID: CA16C0DC-D52F-484E-A539-505517B5F7DC
// Assembly location: C:\Users\babak\Desktop\SS.Crm.Linq.dll

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.Linq;

namespace SS.Crm.Linq
{
  public static class PublicExtensionMethods
  {
    public static CrmQueryable<Entity> Queryable(this IOrganizationService service, string entityLogicalName, params string[] columns)
    {
      if (((IEnumerable<string>) columns).Any<string>())
        return service.Queryable(entityLogicalName, new ColumnSet(columns));
      return service.Queryable(entityLogicalName, (ColumnSet) null);
    }

    public static CrmQueryable<T> Queryable<T>(this IOrganizationService service, params string[] columns)
    {
      if (((IEnumerable<string>) columns).Any<string>())
        return service.Queryable<T>(new ColumnSet(columns));
      return service.Queryable<T>((ColumnSet) null);
    }

    public static CrmQueryable<Entity> Queryable(this IOrganizationService service, string entityLogicalName, ColumnSet columns)
    {
      return CrmQueryFactory.Queryable(service, entityLogicalName, columns, false);
    }

    public static CrmQueryable<T> Queryable<T>(this IOrganizationService service, ColumnSet columns)
    {
      return CrmQueryFactory.Queryable<T>(service, columns, false);
    }
  }
}
