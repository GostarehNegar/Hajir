// Decompiled with JetBrains decompiler
// Type: SS.Crm.Linq.ExpressionTreeVisitors.ExpressionTreeVisitor
// Assembly: SS.Crm.Linq, Version=1.0.6939.26200, Culture=neutral, PublicKeyToken=1eea6d0e8f401bee
// MVID: CA16C0DC-D52F-484E-A539-505517B5F7DC
// Assembly location: C:\Users\babak\Desktop\SS.Crm.Linq.dll

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SS.Crm.Linq.ExpressionTreeVisitors
{
    internal class ExpressionTreeVisitor : ThrowingExpressionVisitor
    {
        private static List<ExpressionType> _validEvaluateExpressionTypes = new List<ExpressionType>()
    {
      ExpressionType.Equal,
      ExpressionType.NotEqual,
      ExpressionType.GreaterThan,
      ExpressionType.GreaterThanOrEqual,
      ExpressionType.LessThan,
      ExpressionType.LessThanOrEqual,
      ExpressionType.Not
    };
        private static List<ExpressionType> _validPostponeExpressionTypes = new List<ExpressionType>()
    {
      ExpressionType.Add,
      ExpressionType.Subtract,
      ExpressionType.Multiply,
      ExpressionType.Divide
    };
        private string _attributeLogicalName = string.Empty;
        private List<string> _validMethodNames = new List<string>()
    {
      "Contains",
      "StartsWith",
      "EndsWith",
      "Equals",
      "IsNullOrEmpty"
    };
        private object _value;
        private ConditionExpression _condition;
        private EntityTypeAlias _entityTypeAlias;
        private FilterExpression _filter;
        private Dictionary<string, string> _entityTypeAliases;
        private IOrganizationService _service;

        public object Value
        {
            get
            {
                return this._value;
            }
        }

        public string AttributeLogicalName
        {
            get
            {
                return this._attributeLogicalName;
            }
        }

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

        public FilterExpression Filter
        {
            get
            {
                return this._filter;
            }
        }

        public ExpressionTreeVisitor(IOrganizationService service, Dictionary<string, string> entityTypeAliases)
        {
            this._service = service;
            this._entityTypeAliases = entityTypeAliases;
        }

        protected override Expression VisitConstant(ConstantExpression expression)
        {
            this._value = expression.Value;
            return (Expression)expression;
        }

        protected override Expression VisitMember(MemberExpression expression)
        {
            this._entityTypeAlias = new EntityTypeAlias()
            {
                Type = expression.Member.DeclaringType,
                Alias = expression.GetAliasedEntityName()
            };
            this._attributeLogicalName = expression.GetAttributeLogicalName(this._service, this._entityTypeAliases);
            return (Expression)expression;
        }

        protected override Expression VisitBinary(BinaryExpression expression)
        {
            ExpressionTreeVisitor wherePartVisitor1 = new ExpressionTreeVisitor(this._service, this._entityTypeAliases);
            wherePartVisitor1.Visit(expression.Left);
            ExpressionTreeVisitor wherePartVisitor2 = new ExpressionTreeVisitor(this._service, this._entityTypeAliases);
            wherePartVisitor2.Visit(expression.Right);
            if (expression.NodeType == ExpressionType.AndAlso || expression.NodeType == ExpressionType.OrElse)
            {
                this._filter = new FilterExpression(expression.NodeType == ExpressionType.AndAlso ? LogicalOperator.And : LogicalOperator.Or);
                if (wherePartVisitor1.Condition != null)
                {
                    this._entityTypeAlias = wherePartVisitor1.EntityTypeAlias;
                    this._filter.AddCondition(wherePartVisitor1.Condition);
                }
                else if (wherePartVisitor1.Filter != null)
                {
                    this._entityTypeAlias = wherePartVisitor1.EntityTypeAlias;
                    this._filter.Filters.Add(wherePartVisitor1.Filter);
                }
                if (wherePartVisitor2.Condition != null)
                {
                    this._entityTypeAlias = wherePartVisitor2.EntityTypeAlias;
                    this._filter.AddCondition(wherePartVisitor2.Condition);
                }
                else if (wherePartVisitor2.Filter != null)
                {
                    this._entityTypeAlias = wherePartVisitor2.EntityTypeAlias;
                    this._filter.Filters.Add(wherePartVisitor2.Filter);
                }
            }
            else
            {
                if (!ExpressionTreeVisitor._validEvaluateExpressionTypes.Contains(expression.NodeType))
                    throw new NotImplementedException(string.Format("The expression operator {0} is not supported.", (object)expression.NodeType.ToString()));
                this._condition = new ConditionExpression();
                this.ParsePartExpression(wherePartVisitor1, this._condition);
                this.ParsePartExpression(wherePartVisitor2, this._condition);
                switch (expression.NodeType)
                {
                    case ExpressionType.Equal:
                        if (this._condition.Values.Count<object>() == 0 || this._condition.Values[0] == null)
                        {
                            this._condition.Operator = ConditionOperator.Null;
                            this._condition.Values.Clear();
                            break;
                        }
                        this._condition.Operator = ConditionOperator.Equal;
                        break;
                    case ExpressionType.GreaterThan:
                        this._condition.Operator = ConditionOperator.GreaterThan;
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        this._condition.Operator = ConditionOperator.GreaterEqual;
                        break;
                    case ExpressionType.LessThan:
                        this._condition.Operator = ConditionOperator.LessThan;
                        break;
                    case ExpressionType.LessThanOrEqual:
                        this._condition.Operator = ConditionOperator.LessEqual;
                        break;
                    case ExpressionType.Not:
                        this._condition.Operator = ConditionOperator.NotNull;
                        this._condition.Values.Clear();
                        break;
                    case ExpressionType.NotEqual:
                        if (this._condition.Values.Count<object>() == 0 || this._condition.Values[0] == null)
                        {
                            this._condition.Operator = ConditionOperator.NotNull;
                            this._condition.Values.Clear();
                            break;
                        }
                        this._condition.Operator = ConditionOperator.NotEqual;
                        break;
                }
            }
            return (Expression)expression;
        }

        private void ParsePartExpression(ExpressionTreeVisitor wherePartVisitor, ConditionExpression condition)
        {
            if (!string.IsNullOrEmpty(wherePartVisitor.AttributeLogicalName))
            {
                condition.AttributeName = wherePartVisitor.AttributeLogicalName;
                this._entityTypeAlias = wherePartVisitor.EntityTypeAlias;
            }
            else
            {
                if (wherePartVisitor.Value == null)
                    return;
                condition.Values.Add(wherePartVisitor.Value);
            }
        }

        protected internal override Expression VisitSubQuery(SubQueryExpression expression)
        {
            SubQueryModelVisitor queryModelVisitor = new SubQueryModelVisitor(this._service, this._entityTypeAliases);
            queryModelVisitor.VisitQueryModel(expression.QueryModel);
            if (queryModelVisitor.Condition != null)
            {
                this._condition = queryModelVisitor.Condition;
                this._entityTypeAlias = queryModelVisitor.EntityTypeAlias;
            }
            return (Expression)expression;
        }

        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            // ISSUE: unable to decompile the method.
            return base.VisitMethodCall(expression);
        }

        protected override Expression VisitUnary(UnaryExpression expression)
        {
            // ISSUE: unable to decompile the method.

            return base.VisitUnary(expression);
        }

        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
        {
            throw new NotImplementedException(string.Format("The CRM linq query provider has not implemented the {0} type method.", (object)visitMethod));
        }
    }
}
