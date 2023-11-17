using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Remotion.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Remotion.Linq.Clauses;
using System.Collections.ObjectModel;

namespace SS.Crm.Linq.Proxies
{
    class CrmQueryModelVisitorProxy : QueryModelVisitorBase
    {
        class CBase
        {
            object instance;
            private BindingFlags all = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
            private PropertyInfo P_Query;
            private MethodInfo M_VisitBodyClauses;
            public CBase(IOrganizationService service, string entityLogicalName = null, ColumnSet ColumnSet = null)
            {
                var type = Type.GetType("SS.Crm.Linq.CrmQueryModelVisitor, SS.Crm.Linq");
                this.instance = Activator.CreateInstance(type, new object[]
                        {service,entityLogicalName,ColumnSet});

                this.P_Query = this.instance.GetType().GetProperty("Query", all);
                this.M_VisitBodyClauses = this.instance.GetType().GetMethod("VisitBodyClauses", all);

            }
            public void VisitQueryModel(QueryModel queryModel)
            {
                (this.instance as QueryModelVisitorBase).VisitQueryModel(queryModel);
            }
            public QueryExpression Query
            {
                get { return (QueryExpression)this.P_Query.GetValue(this.instance); }
            }
            public void VisitBodyClauses(ObservableCollection<IBodyClause> bodyClauses, QueryModel queryModel)
            {
                this.M_VisitBodyClauses.Invoke(this.instance, new object[] { bodyClauses, queryModel });
            }
            public void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
            {
                (this.instance as QueryModelVisitorBase).VisitWhereClause(whereClause, queryModel, index);
            }
        }

        CBase Base;
        public CrmQueryModelVisitorProxy(IOrganizationService service, string entityLogicalName = null, ColumnSet ColumnSet = null)
        {
            this.Base = new CBase(service, entityLogicalName, ColumnSet);
        }

        public override void VisitQueryModel(QueryModel queryModel)
        {
            this.Base.VisitQueryModel(queryModel);
        }
        protected override void VisitBodyClauses(ObservableCollection<IBodyClause> bodyClauses, QueryModel queryModel)
        {
            Base.VisitBodyClauses(bodyClauses, queryModel);
        }
        public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
        {
            base.VisitWhereClause(whereClause, queryModel, index);
        }

        public QueryExpression Query { get { return this.Base.Query; } }
    }
}
