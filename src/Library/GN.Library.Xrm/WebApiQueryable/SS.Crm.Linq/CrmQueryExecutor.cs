// Decompiled with JetBrains decompiler
// Type: SS.Crm.Linq.CrmQueryExecutor
// Assembly: SS.Crm.Linq, Version=1.0.6939.26200, Culture=neutral, PublicKeyToken=1eea6d0e8f401bee
// MVID: CA16C0DC-D52F-484E-A539-505517B5F7DC
// Assembly location: C:\Users\babak\Desktop\SS.Crm.Linq.dll

using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using SS.Crm.Linq.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace SS.Crm.Linq
{
  internal class CrmQueryExecutor : IQueryExecutor
  {
    private List<Type> _supportedAggregates = new List<Type>()
    {
      typeof (SumResultOperator),
      typeof (AverageResultOperator),
      typeof (MinResultOperator),
      typeof (MaxResultOperator),
      typeof (CountResultOperator)
    };
    private IOrganizationService _service;
    private string _entityLogicalName;
    private ColumnSet _columns;
    private bool _retrieveAllRecords;

    public CrmQueryExecutor(IOrganizationService service, string entityLogicalName = null, ColumnSet columns = null, bool retrieveAllRecords = false)
    {
      this._service = service;
      this._entityLogicalName = entityLogicalName;
      this._columns = columns;
      this._retrieveAllRecords = retrieveAllRecords;
    }

    public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
    {
      return this.ExecuteQuery<T>(queryModel);
    }

    public T ExecuteScalar<T>(QueryModel queryModel)
    {
      return this.ExecuteQuery<T>(queryModel).FirstOrDefault<T>();
    }

    public IEnumerable<T> ExecuteQuery<T>(QueryModel queryModel)
    {
      CrmQueryModelVisitor modelVisitor = new CrmQueryModelVisitor(this._service, this._entityLogicalName, this._columns);
      modelVisitor.VisitQueryModel(queryModel);
      ObservableCollection<ResultOperatorBase> resultOperators = queryModel.ResultOperators;
      Func<ResultOperatorBase, bool> func = (Func<ResultOperatorBase, bool>) (resultOperator =>
      {
        if (!(resultOperator is SkipResultOperator))
          return resultOperator is TakeResultOperator;
        return true;
      });
            ///
            ///Func<ResultOperatorBase, bool> predicate
            Func<ResultOperatorBase, bool> predicate = x => { return true; };

      if (resultOperators.Where<ResultOperatorBase>(predicate).Count<ResultOperatorBase>() > 0)
      {
        SkipResultOperator skipResultOperator = queryModel.ResultOperators.Where<ResultOperatorBase>((Func<ResultOperatorBase, bool>) (resultOperator => resultOperator is SkipResultOperator)).FirstOrDefault<ResultOperatorBase>() as SkipResultOperator ?? new SkipResultOperator((Expression) Expression.Constant((object) 0));
        TakeResultOperator takeResultOperator = queryModel.ResultOperators.Where<ResultOperatorBase>((Func<ResultOperatorBase, bool>) (resultOperator => resultOperator is TakeResultOperator)).FirstOrDefault<ResultOperatorBase>() as TakeResultOperator ?? new TakeResultOperator((Expression) Expression.Constant((object) 0));
        queryModel.ResultOperators.Remove((ResultOperatorBase) skipResultOperator);
        queryModel.ResultOperators.Remove((ResultOperatorBase) takeResultOperator);
        if (!(takeResultOperator.Count is ConstantExpression) || !(skipResultOperator.Count is ConstantExpression))
          throw new NotSupportedException("The 'Skip' and 'Take' values must be a constant expression.");
        int num1 = (int) ((ConstantExpression) takeResultOperator.Count).Value;
        int num2 = (int) ((ConstantExpression) skipResultOperator.Count).Value;
        if (num2 != 0 && num1 > num2)
          throw new NotSupportedException("The 'Take' count must be <= the 'Skip' count.");
        if (num2 != 0 && (double) num2 / (double) num1 - Math.Truncate((double) num2 / (double) num1) != 0.0)
          throw new NotSupportedException("The 'Skip' count must be a multiple of the 'Take' count.");
        modelVisitor.Query.PageInfo = new PagingInfo()
        {
          PageNumber = Math.Max(num2 / num1 + 1, 1),
          Count = num1
        };
      }
      if (queryModel.ResultOperators.Count == 1 && this._supportedAggregates.Contains(queryModel.ResultOperators.FirstOrDefault<ResultOperatorBase>().GetType()) || queryModel.ResultOperators.Count == 2 && queryModel.ResultOperators[0] is DistinctResultOperator && this._supportedAggregates.Contains(queryModel.ResultOperators.LastOrDefault<ResultOperatorBase>().GetType()))
      {
        int num = queryModel.ResultOperators.Count > 1 ? 1 : 0;
        ResultOperatorBase aggregate = queryModel.ResultOperators.LastOrDefault<ResultOperatorBase>();
        XDocument xdocument = XDocument.Parse(modelVisitor.Query.ToFetchXML());
        string aggregateOperatorString = aggregate.GetAggregateOperatorString();
        xdocument.Descendants((XName) "fetch").FirstOrDefault<XElement>().Add((object) new XAttribute((XName) "aggregate", (object) "true"));
        XElement xelement = xdocument.Descendants((XName) "attribute").FirstOrDefault<XElement>();
        string str = string.Format("{0}.{1}", (object) xelement.Attribute((XName) "name").Value, (object) aggregateOperatorString);
        xelement.Add((object) new XAttribute((XName) "aggregate", (object) aggregateOperatorString));
        xelement.Add((object) new XAttribute((XName) "alias", (object) str));
        if (num != 0)
          xelement.Add((object) new XAttribute((XName) "distinct", (object) "true"));
        return (IEnumerable<T>) new List<T>()
        {
          (T) Convert.ChangeType((object) XDocument.Parse((this._service.Execute((OrganizationRequest) new ExecuteFetchRequest()
          {
            FetchXml = xdocument.ToString()
          }) as ExecuteFetchResponse).FetchXmlResult).Descendants((XName) str).FirstOrDefault<XElement>().Value, typeof (T))
        };
      }
      if (modelVisitor.GroupByOperators != null && modelVisitor.GroupByOperators.Count > 0)
      {
        GroupResultOperator groupByOperator = modelVisitor.GroupByOperators[0] as GroupResultOperator;
        string groupByAttribute = groupByOperator.KeySelector.GetAttributeLogicalName(this._service, modelVisitor.EntityTypeAliases);
        string aggregateAttribute = groupByOperator.ElementSelector.GetAttributeLogicalName(this._service, modelVisitor.EntityTypeAliases);
        List<string> stringList = new List<string>();
        for (int index = 1; index < modelVisitor.ReturnBindings.Count; ++index)
        {
          ResultOperatorBase resultOperator = (modelVisitor.ReturnBindings[index].SourceExpression as SubQueryExpression).QueryModel.ResultOperators[0];
          stringList.Add(resultOperator.GetAggregateOperatorString());
        }
        XDocument xdocument = XDocument.Parse(modelVisitor.Query.ToFetchXML());
        xdocument.Descendants((XName) "fetch").FirstOrDefault<XElement>().Add((object) new XAttribute((XName) "aggregate", (object) "true"));
        XElement xelement1 = xdocument.Descendants((XName) "attribute").Where<XElement>((Func<XElement, bool>) (element => element.Attribute((XName) "name").Value == groupByAttribute)).FirstOrDefault<XElement>();
        XAttribute xattribute1 = new XAttribute((XName) "groupby", (object) "true");
        xelement1.Add((object) xattribute1);
        XAttribute xattribute2 = new XAttribute((XName) "alias", (object) groupByAttribute);
        xelement1.Add((object) xattribute2);
        XElement xelement2 = xdocument.Descendants((XName) "attribute").Where<XElement>((Func<XElement, bool>) (element => element.Attribute((XName) "name").Value == aggregateAttribute)).FirstOrDefault<XElement>();
        string str1 = string.Format("{0}.{1}", (object) xelement2.Attribute((XName) "name").Value, (object) stringList[0]);
        xelement2.Add((object) new XAttribute((XName) "aggregate", (object) stringList[0]));
        xelement2.Add((object) new XAttribute((XName) "alias", (object) str1));
        for (int index = 1; index < stringList.Count; ++index)
        {
          XElement xelement3 = new XElement((XName) "attribute");
          string str2 = string.Format("{0}.{1}", (object) xelement2.Attribute((XName) "name").Value, (object) stringList[index]);
          xelement3.Add((object) new XAttribute((XName) "name", (object) aggregateAttribute));
          xelement3.Add((object) new XAttribute((XName) "aggregate", (object) stringList[index]));
          xelement3.Add((object) new XAttribute((XName) "alias", (object) str2));
          xelement2.AddAfterSelf((object) xelement3);
          xelement2 = xelement3;
        }
        List<XElement> list = XDocument.Parse((this._service.Execute((OrganizationRequest) new ExecuteFetchRequest()
        {
          FetchXml = xdocument.ToString()
        }) as ExecuteFetchResponse).FetchXmlResult).Descendants((XName) "result").ToList<XElement>();
        List<Entity> entities = new List<Entity>();
        foreach (XElement xelement3 in list)
        {
          Entity entity = new Entity();
          entity.Attributes[groupByAttribute] = Convert.ChangeType((object) xelement3.Element((XName) groupByAttribute).Value, groupByOperator.KeySelector.Type);
          for (int index1 = 0; index1 < stringList.Count; ++index1)
          {
            string index2 = string.Format("{0}.{1}", (object) xelement2.Attribute((XName) "name").Value, (object) stringList[index1]);
            entity.Attributes[index2] = Convert.ChangeType((object) xelement3.Element((XName) index2).Value, groupByOperator.ElementSelector.Type);
          }
          entities.Add(entity);
        }
        return this.ParseEntities<T>(modelVisitor, entities);
      }
      EntityCollection entityCollection = RetrieveHelper.RetrieveEntities(this._service, modelVisitor.Query, this._retrieveAllRecords);
      return this.ParseEntities<T>(modelVisitor, entityCollection.Entities.ToList<Entity>());
    }

    private IEnumerable<T> ParseEntities<T>(CrmQueryModelVisitor modelVisitor, List<Entity> entities)
    {
      Type returnType = modelVisitor.ReturnType;
      if (returnType == typeof (Entity))
        return entities.Cast<T>().AsEnumerable<T>();
      ConstructorInfo constructor = (ConstructorInfo) null;
      try
      {
        constructor = returnType.GetConstructor(new Type[1]
        {
          typeof (Entity)
        });
      }
      catch (Exception ex)
      {
      }
      if (constructor != (ConstructorInfo) null)
        return entities.Select<Entity, T>((Func<Entity, T>) (entity => (T) constructor.Invoke(new object[1]
        {
          (object) entity
        })));
      if (typeof (Entity).IsAssignableFrom(returnType))
      {
        constructor = returnType.GetConstructor(new Type[0]);
        if (!(constructor != (ConstructorInfo) null))
          throw new Exception("The type does not have a constructor that has not arguments");
        List<Entity> source = new List<Entity>();
        foreach (Entity entity in entities)
        {
          Entity target = (Entity) constructor.Invoke(new object[0]);
          entity.CopyTo(target);
          source.Add(target);
        }
        return source.Cast<T>();
      }
      if (CrmQueryExecutor.CheckIfAnonymousType(returnType))
      {
        constructor = ((IEnumerable<ConstructorInfo>) returnType.GetConstructors()).FirstOrDefault<ConstructorInfo>();
        ParameterInfo[] parameters = constructor.GetParameters();
        List<T> objList = new List<T>();
        foreach (Entity entity in entities)
        {
          List<object> objectList = new List<object>();
          if (modelVisitor.GroupByOperators != null)
          {
            string attributeLogicalName = ((GroupResultOperator) modelVisitor.GroupByOperators[0]).KeySelector.GetAttributeLogicalName(this._service, modelVisitor.EntityTypeAliases);
            objectList.Add(entity[attributeLogicalName]);
            for (int index = 1; index < modelVisitor.ReturnBindings.Count; ++index)
            {
              string aggregateOperatorString = ((SubQueryExpression) modelVisitor.ReturnBindings[index].SourceExpression).QueryModel.ResultOperators[0].GetAggregateOperatorString();
              string attributeName = string.Format("{0}.{1}", (object) ((GroupResultOperator) modelVisitor.GroupByOperators[0]).ElementSelector.GetAttributeLogicalName(this._service, modelVisitor.EntityTypeAliases), (object) aggregateOperatorString);
              if (entity.Contains(attributeName))
                objectList.Add(entity[attributeName]);
              else
                objectList.Add((object) null);
            }
          }
          else
          {
            Dictionary<string, ConstructorInfo> entityConstructors = new Dictionary<string, ConstructorInfo>();
            foreach (ParameterInfo parameterInfo in parameters)
            {
              ParameterInfo parameter = parameterInfo;
              AnonymousBinding anonymousBinding = modelVisitor.ReturnBindings.Where<AnonymousBinding>((Func<AnonymousBinding, bool>) (binding => binding.TargetMember.Name == parameter.Name)).FirstOrDefault<AnonymousBinding>();
              if (anonymousBinding != null)
                objectList.Add(this.GetExpressionValue(modelVisitor, anonymousBinding.SourceExpression, entity, entityConstructors));
              else
                objectList.Add((object) null);
            }
          }
          object obj = constructor.Invoke(objectList.ToArray());
          objList.Add((T) obj);
        }
        return (IEnumerable<T>) objList;
      }
      constructor = typeof (T).GetConstructor(new Type[0]);
      if (!(constructor != (ConstructorInfo) null))
        throw new Exception(string.Format("Unable to convert to the type: {0}", (object) returnType.Name));
      List<T> objList1 = new List<T>();
      PropertyInfo[] properties = typeof (T).GetProperties();
      Dictionary<string, ConstructorInfo> entityConstructors1 = new Dictionary<string, ConstructorInfo>();
      foreach (Entity entity in entities)
      {
        T obj = (T) constructor.Invoke(new object[0]);
        foreach (PropertyInfo propertyInfo1 in properties)
        {
          PropertyInfo propertyInfo = propertyInfo1;
          AnonymousBinding anonymousBinding = modelVisitor.ReturnBindings.Where<AnonymousBinding>((Func<AnonymousBinding, bool>) (binding => binding.TargetMember.Name == propertyInfo.Name)).FirstOrDefault<AnonymousBinding>();
          if (anonymousBinding != null)
            propertyInfo.SetValue((object) obj, this.GetExpressionValue(modelVisitor, anonymousBinding.SourceExpression, entity, entityConstructors1));
        }
        objList1.Add(obj);
      }
      return (IEnumerable<T>) objList1;
    }

    private object GetExpressionValue(CrmQueryModelVisitor modelVisitor, Expression expression, Entity entity, Dictionary<string, ConstructorInfo> entityConstructors)
    {
      if (expression is MemberExpression)
      {
        object obj = (object) null;
        MemberExpression expression1 = expression as MemberExpression;
        AttributeLogicalNameAttribute logicalNameAttribute = ((IEnumerable<object>) expression1.Member.GetCustomAttributes(typeof (AttributeLogicalNameAttribute), false)).FirstOrDefault<object>() as AttributeLogicalNameAttribute;
        if (logicalNameAttribute != null)
        {
          string aliasedEntityName = expression1.GetAliasedEntityName();
          string attributeName = logicalNameAttribute.LogicalName;
          if (aliasedEntityName != modelVisitor.MainEntityTypeAlias)
            attributeName = string.Format("{0}.{1}", (object) aliasedEntityName, (object) attributeName);
          obj = attributeName.Contains(".") ? (entity.Contains(attributeName) ? ((AliasedValue) entity[attributeName]).Value : (object) null) : (entity.Contains(attributeName) ? entity[attributeName] : (object) null);
        }
        else if (expression1.Member.Name == "Id")
          return (object) entity.Id;
        return obj;
      }
      if (expression is ConstantExpression)
        return ((ConstantExpression) expression).Value;
      if (expression is UnaryExpression)
      {
        UnaryExpression expression1 = expression as UnaryExpression;
        string aliasedEntityName = expression1.GetAliasedEntityName();
        string attributeName = expression1.Operand.GetAttributeLogicalName(this._service, modelVisitor.EntityTypeAliases);
        if (aliasedEntityName != modelVisitor.MainEntityTypeAlias)
          attributeName = string.Format("{0}.{1}", (object) aliasedEntityName, (object) attributeName);
        if (!attributeName.Contains("."))
        {
          if (!entity.Contains(attributeName))
            return (object) null;
          return entity[attributeName];
        }
        if (!entity.Contains(attributeName))
          return (object) null;
        return ((AliasedValue) entity[attributeName]).Value;
      }
      if (expression is MethodCallExpression)
      {
        MethodCallExpression methodCallExpression = expression as MethodCallExpression;
        if (methodCallExpression.Object == null)
          return methodCallExpression.Method.Invoke((object) methodCallExpression.Object, new object[0]);
        object expressionValue = this.GetExpressionValue(modelVisitor, methodCallExpression.Object, entity, entityConstructors);
        return methodCallExpression.Method.Invoke(expressionValue, new object[0]);
      }
      if (!(expression is QuerySourceReferenceExpression))
        throw new NotSupportedException(string.Format("The expression type {0} is not supported.", (object) expression.GetType().Name));
      QuerySourceReferenceExpression referenceExpression = expression as QuerySourceReferenceExpression;
      if (referenceExpression.ReferencedQuerySource is MainFromClause)
      {
        MainFromClause referencedQuerySource = referenceExpression.ReferencedQuerySource as MainFromClause;
        if (!entityConstructors.ContainsKey(referencedQuerySource.ItemName))
          entityConstructors[referencedQuerySource.ItemName] = referencedQuerySource.ItemType.GetConstructor(new Type[0]);
        Entity entity1 = entityConstructors[referencedQuerySource.ItemName].Invoke(new object[0]) as Entity;
        foreach (KeyValuePair<string, object> attribute in (DataCollection<string, object>) entity.Attributes)
        {
          if (!attribute.Key.Contains("."))
            entity1[attribute.Key] = attribute.Value;
        }
        return (object) entity1;
      }
      if (!(referenceExpression.ReferencedQuerySource is JoinClause))
        throw new NotSupportedException(string.Format("The expression type {0} is not supported.", (object) expression.GetType().Name));
      JoinClause referencedQuerySource1 = referenceExpression.ReferencedQuerySource as JoinClause;
      if (!entityConstructors.ContainsKey(referencedQuerySource1.ItemName))
        entityConstructors[referencedQuerySource1.ItemName] = referencedQuerySource1.ItemType.GetConstructor(new Type[0]);
      Entity entity2 = entityConstructors[referencedQuerySource1.ItemName].Invoke(new object[0]) as Entity;
      foreach (KeyValuePair<string, object> attribute in (DataCollection<string, object>) entity.Attributes)
      {
        if (attribute.Key.Contains(referencedQuerySource1.ItemName + ".") && attribute.Value is AliasedValue)
          entity2[((AliasedValue) attribute.Value).AttributeLogicalName] = ((AliasedValue) attribute.Value).Value;
      }
      return (object) entity2;
    }

    public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
    {
      return this.ExecuteCollection<T>(queryModel).FirstOrDefault<T>();
    }

    private static bool CheckIfAnonymousType(Type type)
    {
      if (type == (Type) null)
        throw new ArgumentNullException(nameof (type));
      if (Attribute.IsDefined((MemberInfo) type, typeof (CompilerGeneratedAttribute), false) && type.IsGenericType && type.Name.Contains("AnonymousType") && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$")))
        return (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
      return false;
    }
  }
}
