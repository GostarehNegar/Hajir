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
