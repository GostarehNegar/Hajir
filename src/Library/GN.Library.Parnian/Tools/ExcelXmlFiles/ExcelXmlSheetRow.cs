using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GN.Library.Parnian.Tools.ExcelFiles
{
    public class ExcelXmlSheetRow : ExcelXmlBaseNode
    {
        protected ExcelXmlWorksheet m_Parent;
        private List<ExcelXmlSheetCell> m_Cells;
        private int? m_Index = null;
        private ExcelXmlSheetRow m_row;

        internal ExcelXmlSheetRow(XmlNode node, XmlNamespaceManager namespaceManager, ExcelXmlWorksheet parent)
            : base(node, namespaceManager)
        {
            m_Parent = parent;
        }
        internal ExcelXmlSheetRow(ExcelXmlSheetRow row, ExcelXmlWorksheet parent)
            : base(row.XmlNode, row.NamespaceManager)
        {
            m_Parent = parent;
            m_Cells = row.Cells;
            m_row = row;
        }

        public ExcelXmlWorksheet ParentSheet { get { return m_Parent; } }
        public int? RowIndex { get { return GetIndex(); } }
        public int? GetIndex(bool refresh = false)
        {
            if (!m_Index.HasValue || refresh)
                m_Index = this.XmlNode == null
                    ? (int?)null
                    : m_Parent.GetIndex(this);
            return m_Index;
        }
        /// <summary>
        /// Gets a list of 'Cells' in this worksheet row.
        /// </summary>
        public List<ExcelXmlSheetCell> Cells
        {
            get
            {
                return GetCells();
            }
        }
        public List<ExcelXmlSheetCell> GetCells(bool refresh = false)
        {
            if (m_Cells == null || refresh)
            {
                m_Cells = m_Node == null
                    ? new List<ExcelXmlSheetCell>()
                    : m_Node.SelectNodes("./ns:Cell", m_NamespaceManager)
                            .OfType<XmlNode>()
                            .Select(x => new ExcelXmlSheetCell(x, m_NamespaceManager, this))
                            .ToList();
            }
            return m_Cells;
        }
        public void CellValuesChanged(string oldKey)
        {
            if (!m_Parent.ByPassIndexing)
            {
                var newKey = GetKey();
                m_Parent.ValueChange(this, oldKey, newKey);
            }


        }
        public virtual string GetKey()
        {
            return m_Parent.GetKey(this);
        }
        public ExcelXmlSheetCell AppenCell()
        {
            ExcelXmlSheetCell result = null;
            var lastCell = Cells.Count > 0
                ? Cells[Cells.Count - 1]
                : null;
            //var node = m_Node;
            if (lastCell != null && m_Node != null)
            {
                var node = m_Node.AppendChild(lastCell.XmlNode.CloneNode(true));
                /// We are adding a new row that may cause errors if "ExpandedRowCount" is
                /// set on this table, therefore we simple remove it.
                //var ExpandedRowCountAttribute = tableNpde.Attributes.OfType<XmlAttribute>().Where(x => x.Name == "ss:ExpandedRowCount").FirstOrDefault();
                //if (ExpandedRowCountAttribute != null)
                //{
                //    tableNpde.Attributes.Remove(ExpandedRowCountAttribute);
                //}
                result = new ExcelXmlSheetCell(node, m_NamespaceManager, this);
                //m_Rows.Add(result);
                //result.CellValuesChanged("");
                //result = Rows[Rows.Count - 1];
            }
            GetCells(true);
            return result;
        }

        public virtual bool Validate(bool refresh = false)
        {
            return true;
        }
        public bool IsValid
        {
            get { return Validate(); }
        }

        private bool IsPersian(string str)
        {
            if (str != null)
            {
                for (var idx = 0; idx < str.Length; idx++)
                {
                    if (str[idx] >= 'ا' && str[idx] <= 'ی')
                        return true;
                }
            }
            return false;

        }
        public bool IsTranslated
        {
            get
            {
                var idx = m_Parent.GetColumnNumber("1065");
                var cell = idx.HasValue && idx.Value > 0 && idx.Value < Cells.Count
                    ? Cells[idx.Value]
                    : null;
                return cell != null && IsPersian(cell.Value);
            }
        }
    }

}
