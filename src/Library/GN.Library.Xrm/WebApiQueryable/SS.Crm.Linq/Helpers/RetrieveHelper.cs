// Decompiled with JetBrains decompiler
// Type: SS.Crm.Linq.Helpers.RetrieveHelper
// Assembly: SS.Crm.Linq, Version=1.0.6939.26200, Culture=neutral, PublicKeyToken=1eea6d0e8f401bee
// MVID: CA16C0DC-D52F-484E-A539-505517B5F7DC
// Assembly location: C:\Users\babak\Desktop\SS.Crm.Linq.dll

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;

namespace SS.Crm.Linq.Helpers
{
  internal class RetrieveHelper
  {
    internal static EntityCollection RetrieveEntities(IOrganizationService service, QueryExpression query, bool retrieveAllEntities = false)
    {
      PagingInfo pageInfo = query.PageInfo;
      if ((pageInfo != null ? ((uint) pageInfo.Count > 0U ? 1 : 0) : 1) != 0)
        retrieveAllEntities = false;
      EntityCollection entityCollection = service.RetrieveMultiple((QueryBase) query);
      while (entityCollection.MoreRecords & retrieveAllEntities)
      {
        IList<Entity> entities = (IList<Entity>) entityCollection.Entities;
        query.PageInfo.PagingCookie = entityCollection.PagingCookie;
        if (query.PageInfo.PageNumber == 0)
          ++query.PageInfo.PageNumber;
        ++query.PageInfo.PageNumber;
        entityCollection = service.RetrieveMultiple((QueryBase) query);
        entityCollection.Entities.AddRange((IEnumerable<Entity>) entities);
      }
      return entityCollection;
    }
  }
}
