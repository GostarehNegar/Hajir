﻿using GN.Library.Odoo.Internal.Concrete;
using System.Xml.Linq;

namespace GN.Library.Odoo.Internal.Entensions
{
    public static class RpcContextEntesionXml
    {
        public static XElement ToXml(this RpcContext context)
        {
            var root = new XElement("records");

            foreach (var record in context.GetRecords())
            {
                var element = new XElement("Record");
                foreach (var field in record.GetFields())
                {
                    element.Add(new XAttribute(field.FieldName,  field.Value));
                }
                root.Add(element);
            }
            return root;
        }
    }
}
