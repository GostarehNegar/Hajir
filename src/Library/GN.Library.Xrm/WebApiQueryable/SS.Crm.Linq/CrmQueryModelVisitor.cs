// Decompiled with JetBrains decompiler
// Type: SS.Crm.Linq.CrmQueryModelVisitor
// Assembly: SS.Crm.Linq, Version=1.0.6939.26200, Culture=neutral, PublicKeyToken=1eea6d0e8f401bee
// MVID: CA16C0DC-D52F-484E-A539-505517B5F7DC
// Assembly location: C:\Users\babak\Desktop\SS.Crm.Linq.dll

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using SS.Crm.Linq.ExpressionTreeVisitors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SS.Crm.Linq
{
  internal class CrmQueryModelVisitor : QueryModelVisitorBase
  {
    private Dictionary<string, string> _entityTypeAliases = new Dictionary<string, string>();
    private Dictionary<string, FilterExpression> _entityFilters = new Dictionary<string, FilterExpression>();
    private Dictionary<string, LinkEntity> _linkEntities = new Dictionary<string, LinkEntity>();
    private List<OrderExpression> _orderExpressions = new List<OrderExpression>();
    private Dictionary<string, ColumnSet> _columns = new Dictionary<string, ColumnSet>();
    private List<AnonymousBinding> _returnBindings = new List<AnonymousBinding>();
    private QueryModel _model;
    private QueryExpression _query;
    private string _mainEntityTypeName;
    private string _mainEntityTypeAlias;
    private ObservableCollection<ResultOperatorBase> _groupByOperators;
    private ColumnSet _mainEntityColumnSet;
    private IOrganizationService _service;
    private Type _returnType;

    public CrmQueryModelVisitor(IOrganizationService service, string entityLogicalName = null, ColumnSet columns = null)
    {
      this._service = service;
      this._mainEntityTypeName = entityLogicalName;
      this._mainEntityColumnSet = columns;
    }

    public Dictionary<string, string> EntityTypeAliases
    {
      get
      {
        return this._entityTypeAliases;
      }
    }

    public QueryExpression Query
    {
      get
      {
        return this._query;
      }
    }

    public Type ReturnType
    {
      get
      {
        return this._returnType;
      }
    }

    public ReadOnlyCollection<AnonymousBinding> ReturnBindings
    {
      get
      {
        return new ReadOnlyCollection<AnonymousBinding>((IList<AnonymousBinding>) this._returnBindings);
      }
    }

    public ObservableCollection<ResultOperatorBase> GroupByOperators
    {
      get
      {
        return this._groupByOperators;
      }
    }

    public string MainEntityTypeAlias
    {
      get
      {
        return this._mainEntityTypeAlias;
      }
    }

    public override void VisitQueryModel(QueryModel queryModel)
    {
      // ISSUE: unable to decompile the method.
    }

    public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
    {
      if (fromClause.FromExpression is SubQueryExpression)
      {
        if (!fromClause.ItemType.IsInterface || !fromClause.ItemType.Name.Contains("IGrouping"))
          throw new NotSupportedException("This type of query is not supported.");
        SubQueryExpression fromExpression = fromClause.FromExpression as SubQueryExpression;
        CrmQueryModelVisitor queryModelVisitor = new CrmQueryModelVisitor(this._service, (string) null, (ColumnSet) null);
        queryModelVisitor.VisitQueryModel(fromExpression.QueryModel);
        this._mainEntityTypeAlias = fromClause.ItemName;
        if (this._mainEntityColumnSet != null)
          this._columns.Add(this._mainEntityTypeAlias, this._mainEntityColumnSet);
        this._mainEntityTypeName = queryModelVisitor._mainEntityTypeName;
        this._groupByOperators = fromExpression.QueryModel.ResultOperators;
        this._entityFilters.Add(this._mainEntityTypeAlias, queryModelVisitor.Query.Criteria);
        GroupResultOperator resultOperator = fromExpression.QueryModel.ResultOperators[0] as GroupResultOperator;
        this.AddColumnToColumnset(this._mainEntityTypeAlias, resultOperator.KeySelector.GetAttributeLogicalName(this._service, this._entityTypeAliases));
        this.AddColumnToColumnset(this._mainEntityTypeAlias, resultOperator.ElementSelector.GetAttributeLogicalName(this._service, this._entityTypeAliases));
      }
      else
      {
        base.VisitMainFromClause(fromClause, queryModel);
        if (string.IsNullOrEmpty(this._mainEntityTypeName))
          this._mainEntityTypeName = fromClause.FromExpression.GetEntityTypeName();
        this._mainEntityTypeAlias = fromClause.ItemName;
        if (this._mainEntityColumnSet != null)
          this._columns.Add(this._mainEntityTypeAlias, this._mainEntityColumnSet);
        if (string.IsNullOrEmpty(this._mainEntityTypeName))
          throw new NotImplementedException("The 'From' object must contain the 'EntityLogicalNameAttribute' attribute.");
        this._entityTypeAliases.Add(this._mainEntityTypeAlias, this._mainEntityTypeName);
      }
    }

    public override void VisitOrderByClause(OrderByClause orderByClause, QueryModel queryModel, int index)
    {
      base.VisitOrderByClause(orderByClause, queryModel, index);
      foreach (Ordering ordering in (Collection<Ordering>) orderByClause.Orderings)
      {
        ExpressionTreeVisitor expressionTreeVisitor = new ExpressionTreeVisitor(this._service, this._entityTypeAliases);
        expressionTreeVisitor.Visit(ordering.Expression);
        if (!string.IsNullOrEmpty(expressionTreeVisitor.AttributeLogicalName))
        {
          if (expressionTreeVisitor.EntityTypeAlias.Alias != this._entityTypeAliases.Keys.ToList<string>()[0])
            throw new Exception(string.Format("An 'orderby' clause may only be set for the alias '{0}'", (object) this._entityTypeAliases.Keys.ToList<string>()[0]));
          this._orderExpressions.Add(new OrderExpression(expressionTreeVisitor.AttributeLogicalName, ordering.OrderingDirection == OrderingDirection.Asc ? OrderType.Ascending : OrderType.Descending));
        }
      }
    }

    public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
    {
      base.VisitWhereClause(whereClause, queryModel, index);
      ExpressionTreeVisitor expressionTreeVisitor = new ExpressionTreeVisitor(this._service, this._entityTypeAliases);
      expressionTreeVisitor.Visit(whereClause.Predicate);
      if (expressionTreeVisitor.Filter != null)
      {
        this._entityFilters.Add(expressionTreeVisitor.EntityTypeAlias.Alias, expressionTreeVisitor.Filter);
      }
      else
      {
        if (expressionTreeVisitor.Condition == null)
          return;
        Dictionary<string, FilterExpression> entityFilters = this._entityFilters;
        string alias = expressionTreeVisitor.EntityTypeAlias.Alias;
        FilterExpression filterExpression = new FilterExpression();
        filterExpression.Conditions.Add(expressionTreeVisitor.Condition);
        entityFilters.Add(alias, filterExpression);
      }
    }

    public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
    {
      if (selectClause.Selector is MemberInitExpression)
      {
        MemberInitExpression selector = selectClause.Selector as MemberInitExpression;
        this._returnType = selector.Type;
        if (selector.Bindings.Count > 0)
        {
          foreach (MemberBinding binding in selector.Bindings)
          {
            if (binding is MemberAssignment)
            {
              MemberAssignment memberAssignment = binding as MemberAssignment;
              string attributeLogicalName = memberAssignment.Expression.GetAttributeLogicalName(this._service, this._entityTypeAliases);
              this.AddColumnToColumnset(memberAssignment.Expression.GetAliasedEntityName(), attributeLogicalName);
              this._returnBindings.Add(new AnonymousBinding()
              {
                SourceExpression = memberAssignment.Expression,
                TargetMember = memberAssignment.Member
              });
            }
          }
        }
      }
      else if (selectClause.Selector is NewExpression)
      {
        NewExpression selector = selectClause.Selector as NewExpression;
        this._returnType = selector.Type;
        if (selector.Arguments.Count > 0)
        {
          foreach (Expression expression in selector.Arguments)
          {
            this._returnBindings.Add(new AnonymousBinding()
            {
              SourceExpression = expression
            });
            this.AddColumnToColumnset(expression.GetAliasedEntityName(), expression.GetAttributeLogicalName(this._service, this._entityTypeAliases));
          }
        }
        if (selector.Members.Count > 0)
        {
          for (int index = 0; index < selector.Members.Count; ++index)
          {
            MemberInfo member = selector.Members[index];
            this._returnBindings[index].TargetMember = member;
          }
        }
      }
      else if (selectClause.Selector is ConstantExpression)
        this._returnType = (selectClause.Selector as ConstantExpression).Type;
      else if (selectClause.Selector is QuerySourceReferenceExpression)
        this._returnType = (selectClause.Selector as QuerySourceReferenceExpression).Type;
      else if (selectClause.Selector is MemberExpression)
      {
        MemberExpression selector = selectClause.Selector as MemberExpression;
        this.AddColumnToColumnset(selector.GetAliasedEntityName(), selector.GetAttributeLogicalName(this._service, this._entityTypeAliases));
        this._returnType = selector.Type;
      }
      base.VisitSelectClause(selectClause, queryModel);
    }

    public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, int index)
    {
      LinkEntity entityFromJoinClause = CrmQueryModelVisitor.GetLinkEntityFromJoinClause(joinClause, queryModel, this._service, this._entityTypeAliases);
      this._linkEntities.Add(joinClause.ItemName, entityFromJoinClause);
      this._entityTypeAliases.Add(entityFromJoinClause.EntityAlias, entityFromJoinClause.LinkToEntityName);
    }

    private static LinkEntity GetLinkEntityFromJoinClause(JoinClause joinClause, QueryModel queryModel, IOrganizationService service, Dictionary<string, string> entityTypeAliases)
    {
      if (joinClause.InnerKeySelector is MemberExpression && joinClause.OuterKeySelector is MemberExpression)
      {
        MemberExpression innerKeySelector = joinClause.InnerKeySelector as MemberExpression;
        MemberExpression outerKeySelector = joinClause.OuterKeySelector as MemberExpression;
        string attributeLogicalName1 = innerKeySelector.GetAttributeLogicalName(service, entityTypeAliases);
        IOrganizationService service1 = service;
        Dictionary<string, string> entityTypeAliases1 = entityTypeAliases;
        string attributeLogicalName2 = outerKeySelector.GetAttributeLogicalName(service1, entityTypeAliases1);
        string entityLogicalName = innerKeySelector.GetEntityLogicalName(entityTypeAliases);
        Dictionary<string, string> entityTypeAliases2 = entityTypeAliases;
        return new LinkEntity(outerKeySelector.GetEntityLogicalName(entityTypeAliases2), entityLogicalName, attributeLogicalName2, attributeLogicalName1, JoinOperator.Inner)
        {
          EntityAlias = innerKeySelector.GetAliasedEntityName(),
          Columns = new ColumnSet(true)
        };
      }
      if (!(joinClause.InnerKeySelector is MethodCallExpression) || !(joinClause.OuterKeySelector is MethodCallExpression))
        throw new NotSupportedException("The supplied join clause is not supported.");
      MethodCallExpression innerKeySelector1 = joinClause.InnerKeySelector as MethodCallExpression;
      MethodCallExpression outerKeySelector1 = joinClause.OuterKeySelector as MethodCallExpression;
      string attributeLogicalName3 = innerKeySelector1.GetAttributeLogicalName(service, entityTypeAliases);
      IOrganizationService service2 = service;
      Dictionary<string, string> entityTypeAliases3 = entityTypeAliases;
      string attributeLogicalName4 = outerKeySelector1.GetAttributeLogicalName(service2, entityTypeAliases3);
      string entityTypeName = (joinClause.InnerSequence as ConstantExpression).GetEntityTypeName();
      return new LinkEntity(((ICrmQueryable) ((ConstantExpression) queryModel.MainFromClause.FromExpression).Value).EntityLogicalName, entityTypeName, attributeLogicalName4, attributeLogicalName3, JoinOperator.Inner)
      {
        EntityAlias = innerKeySelector1.GetAliasedEntityName(),
        Columns = new ColumnSet(true)
      };
    }

    private void AddColumnToColumnset(string alias, string columnName)
    {
      if (string.IsNullOrEmpty(alias) || string.IsNullOrEmpty(columnName))
        return;
      if (!this._columns.ContainsKey(alias))
        this._columns.Add(alias, new ColumnSet(false));
      if (this._columns[alias].Columns.Contains(columnName))
        return;
      this._columns[alias].AddColumn(columnName);
    }

    protected override void VisitBodyClauses(ObservableCollection<IBodyClause> bodyClauses, QueryModel queryModel)
    {
      base.VisitBodyClauses(bodyClauses, queryModel);
    }

    public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index)
    {
      base.VisitAdditionalFromClause(fromClause, queryModel, index);
      if (!(fromClause.FromExpression is SubQueryExpression))
        return;
      SubQueryExpression fromExpression = fromClause.FromExpression as SubQueryExpression;
      string itemName = fromClause.ItemName;
      if (fromExpression.QueryModel.ResultOperators.Count <= 0 || !(fromExpression.QueryModel.ResultOperators[0] is DefaultIfEmptyResultOperator) || (this._linkEntities.Count <= 0 || !this._linkEntities.ContainsKey(itemName)))
        return;
      this._linkEntities[itemName].JoinOperator = JoinOperator.LeftOuter;
    }

    public override void VisitGroupJoinClause(GroupJoinClause groupJoinClause, QueryModel queryModel, int index)
    {
      base.VisitGroupJoinClause(groupJoinClause, queryModel, index);
    }

    public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, GroupJoinClause groupJoinClause)
    {
      base.VisitJoinClause(joinClause, queryModel, groupJoinClause);
      LinkEntity entityFromJoinClause = CrmQueryModelVisitor.GetLinkEntityFromJoinClause(joinClause, queryModel, this._service, this._entityTypeAliases);
      entityFromJoinClause.JoinOperator = JoinOperator.LeftOuter;
      this._linkEntities.Add(groupJoinClause.ItemName, entityFromJoinClause);
    }
  }
}
