// Decompiled with JetBrains decompiler
// Type: SS.Crm.Linq.sourceExtensionMethods
// Assembly: SS.Crm.Linq, Version=1.0.6939.26200, Culture=neutral, PublicKeyToken=1eea6d0e8f401bee
// MVID: CA16C0DC-D52F-484E-A539-505517B5F7DC
// Assembly location: C:\Users\babak\Desktop\SS.Crm.Linq.dll

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SS.Crm.Linq
{
  internal static class sourceExtensionMethods
  {
    private static Dictionary<string, EntityMetadata> _entityMetadata = new Dictionary<string, EntityMetadata>();

    internal static void CopyTo(this Entity source, Entity target)
    {
      if (target == null || target == source)
        return;
      target.RelatedEntities.Clear();
      target.Attributes.Clear();
      target.FormattedValues.Clear();
      target.LogicalName = source.LogicalName;
      target.RelatedEntities.AddRange((IEnumerable<KeyValuePair<Relationship, EntityCollection>>) source.RelatedEntities);
      target.Attributes.AddRange((IEnumerable<KeyValuePair<string, object>>) source.Attributes);
      target.FormattedValues.AddRange((IEnumerable<KeyValuePair<string, string>>) source.FormattedValues);
      target.ExtensionData = source.ExtensionData;
      target.EntityState = source.EntityState;
      target.Id = source.Id;
    }

    internal static string GetAttributeLogicalName(this MemberInfo member, IOrganizationService service, string entityLogicalName)
    {
      AttributeLogicalNameAttribute logicalNameAttribute = ((IEnumerable<object>) member.GetCustomAttributes(typeof (AttributeLogicalNameAttribute), false)).FirstOrDefault<object>() as AttributeLogicalNameAttribute;
      if (logicalNameAttribute != null)
        return logicalNameAttribute.LogicalName;
      if (!typeof (Entity).IsAssignableFrom(member.DeclaringType) || !(member.Name == "Id") || string.IsNullOrEmpty(entityLogicalName))
        return (string) null;
      if (!sourceExtensionMethods._entityMetadata.ContainsKey(entityLogicalName))
      {
        RetrieveEntityRequest retrieveEntityRequest = new RetrieveEntityRequest()
        {
          LogicalName = entityLogicalName,
          EntityFilters = EntityFilters.Entity,
          RetrieveAsIfPublished = true
        };
        RetrieveEntityResponse retrieveEntityResponse = service.Execute((OrganizationRequest) retrieveEntityRequest) as RetrieveEntityResponse;
        sourceExtensionMethods._entityMetadata.Add(entityLogicalName, retrieveEntityResponse.EntityMetadata);
      }
      if (sourceExtensionMethods._entityMetadata.ContainsKey(entityLogicalName))
        return sourceExtensionMethods._entityMetadata[entityLogicalName].PrimaryIdAttribute;
      return "Id";
    }

    internal static string GetAttributeLogicalName(this Expression expression, IOrganizationService service, Dictionary<string, string> entityTypeAliases)
    {
      string str = string.Empty;
      if (expression is MemberExpression)
      {
        MemberExpression member = expression as MemberExpression;
        string entityLogicalName = member.GetEntityLogicalName(entityTypeAliases);
        str = member.Member.GetAttributeLogicalName(service, entityLogicalName);
        if (string.IsNullOrEmpty(str))
          str = member.Expression.GetAttributeLogicalName(service, entityTypeAliases);
      }
      else if (expression is UnaryExpression)
        str = (expression as UnaryExpression).Operand.GetAttributeLogicalName(service, entityTypeAliases);
      else if (expression is MethodCallExpression && ((MethodCallExpression) expression).Object != null && ((MethodCallExpression) expression).Object.Type == typeof (AttributeCollection))
        str = (string) ((ConstantExpression) (expression as MethodCallExpression).Arguments[0]).Value;
      return str;
    }

    internal static string GetEntityLogicalName(this Type type)
    {
      EntityLogicalNameAttribute logicalNameAttribute = ((IEnumerable<object>) type.GetCustomAttributes(typeof (EntityLogicalNameAttribute), false)).FirstOrDefault<object>() as EntityLogicalNameAttribute;
      if (logicalNameAttribute != null)
        return logicalNameAttribute.LogicalName;
      return (string) null;
    }

    internal static string GetEntityLogicalName(this MemberExpression member, Dictionary<string, string> entityTypeAliases)
    {
      string str = member.Member.DeclaringType.GetEntityLogicalName();
      if (string.IsNullOrEmpty(str))
      {
        if (member.Expression is MemberExpression && member.Expression.Type == typeof (EntityReference))
          str = ((MemberExpression) member.Expression).Member.DeclaringType.GetEntityLogicalName();
        else if (member.Expression is QuerySourceReferenceExpression)
        {
          QuerySourceReferenceExpression expression = member.Expression as QuerySourceReferenceExpression;
          if (entityTypeAliases.ContainsKey(expression.ReferencedQuerySource.ItemName))
            str = entityTypeAliases[expression.ReferencedQuerySource.ItemName];
        }
      }
      return str;
    }

    internal static Type GetEntityType(this MemberExpression member)
    {
      Type declaringType = member.Member.DeclaringType;
      if (string.IsNullOrEmpty(declaringType.GetEntityLogicalName()) && member.Expression is MemberExpression && (member.Expression.Type == typeof (EntityReference) && !string.IsNullOrEmpty(((MemberExpression) member.Expression).Member.DeclaringType.GetEntityLogicalName())))
        declaringType = ((MemberExpression) member.Expression).Member.DeclaringType;
      return declaringType;
    }

    internal static string GetAliasedEntityName(this Expression expression)
    {
      if (expression is MethodCallExpression)
      {
        MethodCallExpression methodCallExpression = expression as MethodCallExpression;
        if (methodCallExpression.Object != null)
          return methodCallExpression.Object.GetAliasedEntityName();
        return methodCallExpression.Arguments[0].GetAliasedEntityName();
      }
      if (expression is MemberExpression)
        return (expression as MemberExpression).Expression.GetAliasedEntityName();
      if (expression is QuerySourceReferenceExpression)
        return (expression as QuerySourceReferenceExpression).ReferencedQuerySource.ItemName;
      if (expression is UnaryExpression)
        return (expression as UnaryExpression).Operand.GetAliasedEntityName();
      if (expression is SubQueryExpression)
        return (expression as SubQueryExpression).QueryModel.MainFromClause.ItemName;
      if (expression is ConstantExpression)
        return string.Empty;
      if (expression is PartialEvaluationExceptionExpression)
        return (expression as PartialEvaluationExceptionExpression).EvaluatedExpression.GetAliasedEntityName();
      throw new Exception("Please update the 'GetAliasedEntityName' method to handle the expression type of '" + expression.GetType().ToString() + "'");
    }

    internal static string GetEntityTypeName(this Expression expression)
    {
      if (expression is SubQueryExpression)
        return (expression as SubQueryExpression).QueryModel.MainFromClause.FromExpression.GetEntityTypeName();
      if (!(expression is ConstantExpression))
        throw new NotImplementedException(string.Format("Please update the 'GetEntityTypeName' method handle the expression type of '{0}'", (object) expression.GetType().ToString()));
      ConstantExpression constantExpression = expression as ConstantExpression;
      if (!(constantExpression.Value is IQueryable))
        throw new NotImplementedException(string.Format("Please update the 'GetEntityTypeName' method handle the expression type of '{0}'", (object) expression.GetType().ToString()));
      IQueryable queryable = constantExpression.Value as IQueryable;
      if (queryable is ICrmQueryable)
        return ((ICrmQueryable) queryable).EntityLogicalName;
      return queryable.ElementType.GetEntityLogicalName();
    }

    internal static string ToFetchXML(this QueryExpression query)
    {
      string str1 = "<fetch mapping=\"logical\">" + string.Format("<entity name=\"{0}\">", (object) query.EntityName) + query.ColumnSet.ToFetchXMLPart();
      foreach (OrderExpression order in (Collection<OrderExpression>) query.Orders)
        str1 += order.ToFetchXMLPart();
      string str2 = str1 + query.Criteria.ToFetchXMLPart();
      foreach (LinkEntity linkEntity in (Collection<LinkEntity>) query.LinkEntities)
        str2 += linkEntity.ToFetchXMLPart();
      return str2 + "</entity>" + "</fetch>";
    }

    internal static string ToFetchXMLPart(this FilterExpression filter)
    {
      string str = string.Format("<filter type=\"{0}\">", (object) filter.FilterOperator.ToString().ToLowerInvariant());
      foreach (ConditionExpression condition in (Collection<ConditionExpression>) filter.Conditions)
        str += condition.ToFetchXMLPart();
      foreach (FilterExpression filter1 in (Collection<FilterExpression>) filter.Filters)
        str += filter1.ToFetchXMLPart();
      return str + "</filter>";
    }

    internal static string ToFetchXMLPart(this ConditionExpression condition)
    {
      string operatorString = condition.Operator.GetOperatorString();
      string str1 = "";
      if (condition.Values == null || condition.Values.Count == 0)
        str1 = string.Format("<condition attribute=\"{0}\" operator=\"{1}\" />", (object) condition.AttributeName, (object) operatorString);
      else if (condition.Values.Count > 1 || condition.Operator == ConditionOperator.In || condition.Operator == ConditionOperator.NotIn)
      {
        string str2 = string.Format("<condition attribute=\"{0}\" operator=\"{1}\">", (object) condition.AttributeName, (object) operatorString);
        foreach (object obj in (Collection<object>) condition.Values)
          str2 = !(obj is EntityReference) ? str2 + string.Format("<value>{0}</value>", (object) obj.ToString()) : str2 + string.Format("<value uitype=\"{0}\">{1}</value>", (object) ((EntityReference) obj).LogicalName, (object) ((EntityReference) obj).Id.ToString());
        str1 = str2 + "</condition>";
      }
      else if (condition.Values.Count == 1)
        str1 = string.Format("<condition attribute=\"{0}\" operator=\"{1}\" value=\"{2}\" />", (object) condition.AttributeName, (object) operatorString, (object) condition.Values[0].ToString());
      return str1;
    }

    internal static string ToFetchXMLPart(this ColumnSet columns)
    {
      string str = "";
      if (columns.AllColumns)
      {
        str = "<all-attributes />";
      }
      else
      {
        foreach (string column in (Collection<string>) columns.Columns)
          str += string.Format("<attribute name=\"{0}\" />", (object) column);
      }
      return str;
    }

    internal static string ToFetchXMLPart(this LinkEntity linkEntity)
    {
      return string.Format("<link-entity name=\"{0}\" from=\"{1}\" to=\"{2}\" alias=\"{3}\">", (object) linkEntity.LinkToEntityName, (object) linkEntity.LinkFromAttributeName, (object) linkEntity.LinkToAttributeName, (object) linkEntity.EntityAlias) + linkEntity.Columns.ToFetchXMLPart() + linkEntity.LinkCriteria.ToFetchXMLPart() + "</link-entity>";
    }

    internal static string ToFetchXMLPart(this OrderExpression order)
    {
      return string.Format("<order attribute=\"{0}\" descending=\"{1}\" />", (object) order.AttributeName, order.OrderType == OrderType.Ascending ? (object) "false" : (object) "true");
    }

    internal static string GetOperatorString(this ConditionOperator op)
    {
      switch (op)
      {
        case ConditionOperator.Equal:
          return "eq";
        case ConditionOperator.NotEqual:
        case ConditionOperator.NotOn:
          return "ne";
        case ConditionOperator.GreaterThan:
          return "gt";
        case ConditionOperator.LessThan:
          return "lt";
        case ConditionOperator.GreaterEqual:
          return "ge";
        case ConditionOperator.LessEqual:
          return "le";
        case ConditionOperator.Like:
        case ConditionOperator.Contains:
        case ConditionOperator.BeginsWith:
          return "like";
        case ConditionOperator.NotLike:
          return "not-like";
        case ConditionOperator.In:
          return "in";
        case ConditionOperator.NotIn:
          return "not-in";
        case ConditionOperator.Between:
          return "between";
        case ConditionOperator.NotBetween:
          return "not-between";
        case ConditionOperator.Null:
          return "null";
        case ConditionOperator.NotNull:
          return "not-null";
        case ConditionOperator.Yesterday:
          return "yesterday";
        case ConditionOperator.Today:
          return "today";
        case ConditionOperator.Tomorrow:
          return "tomorrow";
        case ConditionOperator.Last7Days:
          return "last-seven-days";
        case ConditionOperator.Next7Days:
          return "next-seven-days";
        case ConditionOperator.LastWeek:
          return "last-week";
        case ConditionOperator.ThisWeek:
          return "this-week";
        case ConditionOperator.NextWeek:
          return "next-week";
        case ConditionOperator.LastMonth:
          return "last-month";
        case ConditionOperator.ThisMonth:
          return "this-month";
        case ConditionOperator.NextMonth:
          return "next-month";
        case ConditionOperator.On:
          return "on";
        case ConditionOperator.OnOrBefore:
          return "on-or-before";
        case ConditionOperator.OnOrAfter:
          return "on-or-after";
        case ConditionOperator.LastYear:
          return "last-year";
        case ConditionOperator.ThisYear:
          return "this-year";
        case ConditionOperator.NextYear:
          return "next-year";
        case ConditionOperator.LastXHours:
          return "last-x-hours";
        case ConditionOperator.NextXHours:
          return "next-x-hours";
        case ConditionOperator.LastXDays:
          return "last-x-days";
        case ConditionOperator.NextXDays:
          return "next-x-days";
        case ConditionOperator.LastXWeeks:
          return "last-x-weeks";
        case ConditionOperator.NextXWeeks:
          return "next-x-weeks";
        case ConditionOperator.LastXMonths:
          return "last-x-months";
        case ConditionOperator.NextXMonths:
          return "next-x-months";
        case ConditionOperator.LastXYears:
          return "last-x-years";
        case ConditionOperator.NextXYears:
          return "next-x-years";
        case ConditionOperator.EqualUserId:
          return "eq-userid";
        case ConditionOperator.NotEqualUserId:
          return "ne-userid";
        case ConditionOperator.EqualBusinessId:
          return "eq-businessid";
        case ConditionOperator.NotEqualBusinessId:
          return "ne-businessid";
        case ConditionOperator.DoesNotContain:
        case ConditionOperator.DoesNotBeginWith:
        case ConditionOperator.DoesNotEndWith:
          return "not-like";
        case ConditionOperator.OlderThanXMonths:
          return "olderthan-x-months";
        case ConditionOperator.ThisFiscalYear:
          return "this-fiscal-year";
        case ConditionOperator.ThisFiscalPeriod:
          return "this-fiscal-period";
        case ConditionOperator.NextFiscalYear:
          return "next-fiscal-year";
        case ConditionOperator.NextFiscalPeriod:
          return "next-fiscal-period";
        case ConditionOperator.LastFiscalYear:
          return "last-fiscal-year";
        case ConditionOperator.LastFiscalPeriod:
          return "last-fiscal-period";
        case ConditionOperator.LastXFiscalYears:
          return "last-x-fiscal-years";
        case ConditionOperator.LastXFiscalPeriods:
          return "last-x-fiscal-periods";
        case ConditionOperator.NextXFiscalYears:
          return "next-x-fiscal-years";
        case ConditionOperator.NextXFiscalPeriods:
          return "next-x-fiscal-periods";
        case ConditionOperator.InFiscalYear:
          return "in-fiscal-year";
        case ConditionOperator.InFiscalPeriod:
          return "in-fiscal-period";
        case ConditionOperator.InFiscalPeriodAndYear:
          return "in-fiscal-period-and-year";
        case ConditionOperator.InOrBeforeFiscalPeriodAndYear:
          return "in-or-before-fiscal-period-and-year";
        case ConditionOperator.InOrAfterFiscalPeriodAndYear:
          return "in-or-after-fiscal-period-and-year";
        default:
          throw new NotSupportedException("Please update the 'GetOperatorString' method to handle the '" + op.ToString() + "' operator.");
      }
    }

    internal static string GetAggregateOperatorString(this ResultOperatorBase aggregate)
    {
      if (aggregate is SumResultOperator)
        return "sum";
      if (aggregate is AverageResultOperator)
        return "avg";
      if (aggregate is MaxResultOperator)
        return "max";
      if (aggregate is MinResultOperator)
        return "min";
      if (aggregate is CountResultOperator)
        return "countcolumn";
      throw new NotSupportedException("The '" + aggregate.GetType().ToString() + "' aggregate type is not supported.");
    }
  }
}
