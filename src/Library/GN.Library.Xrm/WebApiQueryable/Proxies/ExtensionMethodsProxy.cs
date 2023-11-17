using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SS.Crm.Linq.Proxies
{
    public static class ExtensionMethodsProxy
    {
        private static BindingFlags all = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
        private static MethodInfo M_ToFetchXML;
        static ExtensionMethodsProxy()
        {
            var type = Type.GetType("SS.Crm.Linq.sourceExtensionMethods, SS.Crm.Linq");

            M_ToFetchXML = type.GetMethod("ToFetchXML", all);

        }
        internal static string ToFetchXML(this QueryExpression query)
        {
            return (string)M_ToFetchXML.Invoke(null, new object[] { query });
        }
    }
}
