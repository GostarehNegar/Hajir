// Decompiled with JetBrains decompiler
// Type: SS.Crm.Linq.ExpressionTreeVisitors.SubQueryModelVisitor
// Assembly: SS.Crm.Linq, Version=1.0.6939.26200, Culture=neutral, PublicKeyToken=1eea6d0e8f401bee
// MVID: CA16C0DC-D52F-484E-A539-505517B5F7DC
// Assembly location: C:\Users\babak\Desktop\SS.Crm.Linq.dll

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SS.Crm.Linq.ExpressionTreeVisitors
{
  internal class SubQueryModelVisitor : QueryModelVisitorBase
  {
    private ConditionExpression _condition;
    private List<object> _values;
    private EntityTypeAlias _entityTypeAlias;
    private IOrganizationService _service;
    private Dictionary<string, string> _entityTypeAliases;

    public EntityTypeAlias EntityTypeAlias
    {
      get
      {
        return this._entityTypeAlias;
      }
    }

    public ConditionExpression Condition
    {
      get
      {
        return this._condition;
      }
    }

    public SubQueryModelVisitor(IOrganizationService service, Dictionary<string, string> entityTypeAliases)
    {
      this._service = service;
      this._entityTypeAliases = entityTypeAliases;
    }

    public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
    {
      if (fromClause.FromExpression is ConstantExpression)
      {
        ConstantExpression fromExpression = fromClause.FromExpression as ConstantExpression;
        if (fromExpression.Value is IEnumerable)
        {
          this._values = new List<object>();
          foreach (object obj in (IEnumerable) fromExpression.Value)
          {
            if (obj.GetType().IsEnum)
              this._values.Add((object) (int) obj);
            else
              this._values.Add(obj);
          }
        }
      }
      base.VisitMainFromClause(fromClause, queryModel);
    }

    public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
    {
      base.VisitResultOperator(resultOperator, queryModel, index);
      if (resultOperator is ContainsResultOperator && this._values != null)
      {
        ContainsResultOperator containsResultOperator = resultOperator as ContainsResultOperator;
        if (containsResultOperator.Item != null && containsResultOperator.Item is MemberExpression)
        {
          MemberExpression expression = containsResultOperator.Item as MemberExpression;
          AttributeLogicalNameAttribute logicalNameAttribute1 = ((IEnumerable<object>) expression.Member.GetCustomAttributes(typeof (AttributeLogicalNameAttribute), false)).FirstOrDefault<object>() as AttributeLogicalNameAttribute;
          if (logicalNameAttribute1 != null)
          {
            this._condition = new ConditionExpression()
            {
              AttributeName = logicalNameAttribute1.LogicalName
            };
            this._condition.Operator = ConditionOperator.In;
            this._condition.Values.AddRange(this._values.ToArray());
            this._entityTypeAlias = new EntityTypeAlias()
            {
              Alias = expression.GetAliasedEntityName()
            };
          }
          else if (expression.Expression.Type == typeof (EntityReference) && expression.Expression is MemberExpression)
          {
            AttributeLogicalNameAttribute logicalNameAttribute2 = ((IEnumerable<object>) ((MemberExpression) expression.Expression).Member.GetCustomAttributes(typeof (AttributeLogicalNameAttribute), false)).FirstOrDefault<object>() as AttributeLogicalNameAttribute;
            this._condition = new ConditionExpression()
            {
              AttributeName = logicalNameAttribute2.LogicalName
            };
            this._condition.Operator = ConditionOperator.In;
            this._condition.Values.AddRange(this._values.ToArray());
            this._entityTypeAlias = new EntityTypeAlias()
            {
              Alias = expression.GetAliasedEntityName()
            };
          }
        }
        else if (containsResultOperator.Item != null && containsResultOperator.Item is UnaryExpression)
        {
          UnaryExpression expression = containsResultOperator.Item as UnaryExpression;
          string attributeLogicalName = expression.GetAttributeLogicalName(this._service, this._entityTypeAliases);
          this._condition = new ConditionExpression()
          {
            AttributeName = attributeLogicalName
          };
          this._condition.Operator = ConditionOperator.In;
          this._condition.Values.AddRange(this._values.ToArray());
          this._entityTypeAlias = new EntityTypeAlias()
          {
            Alias = expression.Operand.GetAliasedEntityName()
          };
        }
      }
      if (this._condition == null)
        throw new NotImplementedException(string.Format("The CRM Linq query provider cannot handle this expression: {0}", (object) resultOperator.ToString()));
    }
  }
}
