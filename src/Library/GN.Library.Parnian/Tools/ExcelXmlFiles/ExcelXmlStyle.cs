using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace GN.Library.Parnian.Tools.ExcelFiles
{
    public class ExcelXmlStyle : ExcelXmlBaseNode
    {
        private ExcelXmlWorksheet m_Parent;
        public ExcelXmlStyle(XmlNode node, XmlNamespaceManager nameSpaceManager)
            : base(node, nameSpaceManager)
        {
            //m_Parent = parent;
        }

        public string Id
        {
            get
            {
                return m_Node.Attributes["ss:ID"] == null
                    ? null
                    : m_Node.Attributes["ss:ID"].Value;
            }
        }
    }

}
