using GN.Library.Shared.Entities;
using GN.Library.Xrm;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Hajir.Crm.Infrastructure.Xrm
{
    internal static class HajirXrmExtensions
    {
        public static string GetFormattedValue(this Entity entity, string key)
        {
            return entity.FormattedValues.TryGetValue(key, out var res) ? res : null;
        }

        internal static string RemoveArabic(this string str)
        {
            return str == null ? str : str.Replace("ك", "ک").Replace("ي", "ی");

        }
        public static EntityReference ImportEntityReference(IXrmDataServices target, EntityReference entityReference)
        {
            if (entityReference == null)
                return null;

            Entity target_entity = null;

            try
            {
                target_entity = target.GetXrmOrganizationService()
                    .GetOrganizationService()
                    .Retrieve(entityReference.LogicalName, entityReference.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(false));
            }
            catch (System.Exception ex)
            {
                //Log Error

            }
            if (target_entity == null)
            {
                var id = target.GetXrmOrganizationService()
                    .GetOrganizationService()
                    .Create(new Entity { LogicalName = entityReference.LogicalName, Id = entityReference.Id });
            }

            return new EntityReference(entityReference.LogicalName, entityReference.Id);
        }
        public static bool SetOwner(this IXrmDataServices dataServices, string entityname, Guid id, Guid? owner)
        {
            var result = false;
            var tableName = entityname + "base";
            var idcolumn = entityname + "id";
            if (!owner.HasValue)
            {
                return false;
            }

            dataServices.WithImpersonatedSqlConnection(con => {
                try
                {
                    con.Open();
                    var cmd = con.CreateCommand();
                    cmd.CommandText = $"update {tableName} set " +
                              $" ownerid = '{owner}'  " +
                              $" where {idcolumn}='{id.ToString()}'";
                    result = cmd.ExecuteNonQuery() == 1;
                }
                catch (Exception ex)
                {

                }
            });
            return result;
        }
        public static bool SetModifiedeBy(this IXrmDataServices dataServices, string entityname, Guid id, Guid? createdby, Guid? modifiedby)
        {
            var result = false;
            var tableName = entityname + "base";
            var idcolumn = entityname + "id";
            if (!modifiedby.HasValue || !createdby.HasValue)
            {
                return false;
            }

            dataServices.WithImpersonatedSqlConnection(con => {
                try
                {
                    con.Open();
                    var cmd = con.CreateCommand();
                    cmd.CommandText = $"update {tableName} set " +
                              $" modifiedby = '{modifiedby}'  " +
                              $", createdby =  '{createdby}'  " +
                              $" where {idcolumn}='{id.ToString()}'";
                    result = cmd.ExecuteNonQuery() == 1;
                }
                catch (Exception ex)
                {

                }
            });
            return result;
        }
        internal static void SetStates(this IXrmDataServices dataServices, DynamicEntity source, XrmEntity dest)
        {
            var state_code = source.GetAttributeValue<int?>(XrmEntity.Schema.StateCode);
            var status_code = source.GetAttributeValue<int?>(XrmEntity.Schema.StatusCode);
            if (state_code.HasValue && status_code.HasValue)
            {
                dataServices.GetXrmOrganizationService().SetState(dest, state_code.Value, status_code.Value);
            }
        }
        internal static bool SetDates(this IXrmDataServices dataServices, string entityname, Guid id, DateTime createdOn, DateTime modifiedon)
        {
            var result = false;
            var tableName = entityname + "base";
            var idcolumn = entityname + "id";
            dataServices.WithImpersonatedSqlConnection(con =>
            {
                try
                {
                    con.Open();
                    var cmd = con.CreateCommand();
                    cmd.CommandText = $"update {tableName} set " +
                              $"  modifiedon = CAST('{modifiedon.ToString("yyyy-MM-dd'T'HH:mm:ss")}' AS DATETIME)  " +
                              $", createdon =  CAST('{createdOn.ToString("yyyy-MM-dd'T'HH:mm:ss")}' AS DATETIME) " +
                              $" where {idcolumn}='{id.ToString()}'";
                    result = cmd.ExecuteNonQuery() == 1;
                }
                catch (Exception ex)
                {

                }
            });
            return result;
        }

        internal static string FormatPersianDate(this DateTime? dt, string fmt = null)
        {
            return dt.HasValue ? dt.Value.ToString(fmt ?? "yyyy/MM/dd", new CultureInfo("fa-IR")) : string.Empty;
        }
    }
}
