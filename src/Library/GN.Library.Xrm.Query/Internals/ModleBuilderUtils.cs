using Microsoft.EntityFrameworkCore;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Xrm.Sdk.Client;

namespace GN.Library.Xrm.Query.Internal
{
    internal static class ModleBuilderUtils
    {
        public static Type[] GetRelatedTypes(this Type type)
        {
            var result = new List<Type>();
            if (type != null)
            {
                result.AddRange(
                type.GetProperties()
                    .Where(x => typeof(XrmEntity).IsAssignableFrom(x.PropertyType))
                    .Select(x => x.PropertyType));
                result.AddRange(type.GetProperties()
                    .Where(x => x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                    .Where(x => typeof(XrmEntity).IsAssignableFrom(x.PropertyType.GenericTypeArguments[0]))
                    .Select(x => x.PropertyType.GenericTypeArguments[0])
                    .ToList());
                result.AddRange(type.GetProperties()
                    .Where(x => x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    .Where(x => typeof(XrmEntity).IsAssignableFrom(x.PropertyType.GenericTypeArguments[0]))
                    .Select(x => x.PropertyType.GenericTypeArguments[0])
                    .ToList());


            }

            return result.ToArray();
        }
        public static bool IsNavigation(this PropertyInfo prop, IEnumerable<Type> types)
        {
            return prop != null && types.Contains(prop.PropertyType);
        }
        public static Type GetCollectionType(this PropertyInfo prop)
        {
            if (prop.PropertyType.IsGenericType &&
                prop.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
            {
                return prop.PropertyType.GenericTypeArguments[0];
            }
            if (prop.PropertyType.IsGenericType &&
                prop.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return prop.PropertyType.GenericTypeArguments[0];
            }
            return null;
        }
        public static bool IsICollection(this PropertyInfo prop, Type type)
        {
            return prop != null && prop.PropertyType == typeof(ICollection<>).MakeGenericType(type);
        }
        public static bool IsICollection<T>(this PropertyInfo prop)
        {
            return prop.IsICollection(typeof(T));
        }
        public static IEnumerable<PropertyInfo> GetCollectionProperties(this Type type, Type targetType)
        {
            return type.GetProperties().Where(x => x.IsICollection(targetType));
        }
        public static IEnumerable<PropertyInfo> GetCollectionProperties<T>(this Type type)
        {
            return type.GetProperties().Where(x => x.IsICollection<T>());
        }
        public static void GetReverseProperty<T>(this Type type, string foreign)
        {


        }
        public static string LogicalOrTableName(this Type type, bool inherit = true)
        {
            return type.GetCustomAttribute<EntityLogicalNameAttribute>(inherit)?.LogicalName ??
                type.GetCustomAttribute<TableAttribute>(inherit)?.Name;
        }
        public static bool IsXrmEntity(this Type type)
        {
            return typeof(XrmEntity).IsAssignableFrom(type);
        }
        public static bool IsEntityBase(this Type type)
        {
            return IsXrmEntity(type) && !string.IsNullOrWhiteSpace(type.LogicalOrTableName(false));
        }
        public static bool IsDerivedEntity(this Type type)
        {

            if (type.BaseType == null || type.IsEntityBase())
                return false;
            return type.BaseType.IsEntityBase() || type.BaseType.IsDerivedEntity();

        }
        public static bool IsDerivedFrom(this Type type, Type basType)
        {
            if (type.BaseType == null)
                return false;
            if (type.BaseType == basType)
                return true;
            return type.BaseType.IsDerivedFrom(basType);
        }
        public static bool InSameHierarchy(this Type type, Type basType)
        {
            return type.IsDerivedFrom(basType) || basType.IsDerivedFrom(type);
        }
        public static Type BaseOfHierarchy(this Type type, Type basType)
        {
            return type.IsDerivedFrom(basType) ? basType : basType.IsDerivedFrom(type) ? type : null;
        }


        public static string GetColumnName(this PropertyInfo prop)
        {
            return prop.GetCustomAttribute<AttributeLogicalNameAttribute>()?.LogicalName ??
                prop.GetCustomAttribute<ColumnAttribute>()?.Name;

        }
        public static string GetForeignKeyName(this PropertyInfo prop)
        {
            return prop.GetCustomAttribute<ForeignKeyAttribute>()?.Name ??
                prop.GetCustomAttribute<ForeignKeyExAttribute>()?.Name;

        }


    }

}
