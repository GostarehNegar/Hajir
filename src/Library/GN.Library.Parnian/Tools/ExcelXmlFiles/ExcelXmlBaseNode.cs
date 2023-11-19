using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;


namespace GN.Library.Parnian.Tools.ExcelFiles
{
    public class ExcelXmlBaseNode
    {
        protected XmlNode m_Node;
        protected XmlNamespaceManager m_NamespaceManager;
        internal ExcelXmlBaseNode(XmlNode node, XmlNamespaceManager namespaceManager)
        {
            m_Node = node;
            m_NamespaceManager = namespaceManager;
        }

        public XmlNode XmlNode { get { return m_Node; } }
        public virtual bool Validate(bool Throw) { return true; }
        public XmlNamespaceManager NamespaceManager { get { return m_NamespaceManager; } }

    }

}
