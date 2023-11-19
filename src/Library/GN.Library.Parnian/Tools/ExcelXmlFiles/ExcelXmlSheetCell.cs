using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GN.Library.Parnian.Tools.ExcelFiles
{
    public class ExcelXmlSheetCell : ExcelXmlBaseNode
    {
        private ExcelXmlSheetRow m_parent;
        internal ExcelXmlSheetCell(XmlNode node, XmlNamespaceManager namespaceManager, ExcelXmlSheetRow parent)
            : base(node, namespaceManager)
        {
            m_parent = parent;
        }
        /// <summary>
        /// Gets or sets the cell value.
        /// </summary>
        public string Value
        {
            get
            {
                return m_Node == null || m_Node.SelectSingleNode("./ns:Data", m_NamespaceManager) == null
                    ? null
                    : m_Node.SelectSingleNode("./ns:Data", m_NamespaceManager).InnerText;

            }
            set
            {
                if (m_Node != null && m_Node.SelectSingleNode("./ns:Data", m_NamespaceManager) != null)
                {
                    var oldKey = m_parent.GetKey();
                    m_Node.SelectSingleNode("./ns:Data", m_NamespaceManager).InnerText = value;
                    if (m_parent != null)
                    {
                        m_parent.CellValuesChanged(oldKey);
                    }
                }
            }
        }
        private const string STYLE_ID = "ss:StyleID";
        private const string STYLE = "StyleID";
        public string StyleID
        {
            get
            {
                var result = m_Node == null
                    ? null
                    : m_Node.Attributes[STYLE_ID] == null
                        ? null
                        : m_Node.Attributes[STYLE_ID].Value;
                return result;
            }
            set
            {
                if (m_Node.Attributes[STYLE_ID] == null)
                {
                    var attribute = m_Node.OwnerDocument.CreateAttribute("ss", STYLE);
                    attribute.Value = value;
                    m_Node.Attributes.Append(attribute);
                }
                else
                {
                    m_Node.Attributes[STYLE_ID].Value = value;
                }
            }
        }
    }

}
