using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GN.Library.Parnian.Tools.ExcelFiles
{
    public class ExcelXmlWorksheet : ExcelXmlBaseNode
    {
        protected List<ExcelXmlSheetRow> m_Rows;
        private ExcelXmlFile m_Parent;
        private Dictionary<string, List<ExcelXmlSheetRow>> m_Index;
        internal ExcelXmlWorksheet(XmlNode node, XmlNamespaceManager namespaceManager, List<ExcelXmlSheetRow> rows, ExcelXmlFile parent)
            : base(node, namespaceManager)
        {
            m_Rows = rows;
            m_Parent = parent;
            CleanTableAttributes();
        }

        public bool ByPassIndexing = false;
        public void CleanTableAttributes()
        {
            //var ExpandedRowCountAttribute = tableNpde.Attributes.OfType<XmlAttribute>().Where(x => x.Name == "ss:ExpandedRowCount").FirstOrDefault();
            //if (ExpandedRowCountAttribute != null)
            //{
            //    tableNpde.Attributes.Remove(ExpandedRowCountAttribute);
            //}
            var tableNode = GetTableNode();
            var attribute = tableNode.Attributes.OfType<XmlAttribute>().Where(x => x.Name == "ss:ExpandedRowCount").FirstOrDefault();
            if (attribute != null)
            {
                tableNode.Attributes.Remove(attribute);
            }
            attribute = tableNode.Attributes.OfType<XmlAttribute>().Where(x => x.Name == "ss:ExpandedColumnCount").FirstOrDefault();
            if (attribute != null)
            {
                tableNode.Attributes.Remove(attribute);
            }

        }
        public ExcelXmlFile ParentFile { get { return m_Parent; } }

        /// <summary>
        /// Returns the worksheet name.
        /// It assumes that worksheet names are stored as 'Xml Attributes' such as in:
        /// (Worksheet ss:Name='some name')
        /// </summary>
        public string Name
        {
            get
            {
                return m_Node == null ||
                    m_Node.Attributes.OfType<XmlAttribute>().Where(x => x.Name == "ss:Name").FirstOrDefault() == null
                    ? null
                    : m_Node.Attributes.OfType<XmlAttribute>().Where(x => x.Name == "ss:Name").FirstOrDefault().Value;
            }
            set
            {
                var attrib = m_Node == null ||
                    m_Node.Attributes.OfType<XmlAttribute>().Where(x => x.Name == "ss:Name").FirstOrDefault() == null
                    ? null
                    : m_Node.Attributes.OfType<XmlAttribute>().Where(x => x.Name == "ss:Name").FirstOrDefault();
                if (attrib != null)
                {
                    attrib.Value = value;
                }
            }
        }
        /// <summary>
        /// Returns the first 'Table' node of this worksheet.
        /// Note that in case of trasnlation sheets, we always assume that
        /// each sheet contians only a single 'Table'.
        /// </summary>
        /// <returns></returns>
        private XmlNode GetTableNode()
        {
            return m_Node == null
                ? null
                : m_Node.SelectSingleNode("./ns:Table", m_NamespaceManager);

        }

        public int? GetIndex(ExcelXmlSheetRow row)
        {
            int result = 0;
            foreach (var r in GetRows())
            {
                if (row != null && row.XmlNode != null && r.XmlNode != null && object.ReferenceEquals(r.XmlNode, row.XmlNode))
                    return result;
                result++;
            }
            return null;
        }

        public List<ExcelXmlSheetRow> GetRows(bool refresh = true)
        {

            if (m_Rows == null || refresh)
            {
                var tableNode = m_Node == null
                    ? null
                    : m_Node.SelectSingleNode("./ns:Table", m_NamespaceManager);
                m_Rows = tableNode == null
                    ? new List<ExcelXmlSheetRow>()
                    : tableNode.SelectNodes("./ns:Row", m_NamespaceManager)
                                .OfType<XmlNode>()
                                .Select(x => new ExcelXmlSheetRow(x, m_NamespaceManager, this))
                                .ToList();
            }
            return m_Rows;

        }
        protected void Invalidate()
        {
            m_Rows = null;
        }

        /// <summary>
        /// Returns the list of 'Rows' for this worksheet.
        /// </summary>
        public List<ExcelXmlSheetRow> Rows { get { return GetRows(false); } }
        /// <summary>
        /// Appends a new row to the 'Table' node of the worksheet
        /// by cloning the last row.
        /// It may return NULL when the table doesnot contain any row.
        /// </summary>
        /// <returns></returns>
        public ExcelXmlSheetRow AppendRow()
        {
            ExcelXmlSheetRow result = null;
            var lastRow = Rows.Count > 0
                ? Rows[Rows.Count - 1]
                : null;
            var tableNpde = GetTableNode();
            if (lastRow != null && tableNpde != null)
            {
                var node = tableNpde.AppendChild(lastRow.XmlNode.CloneNode(true));
                /// We are adding a new row that may cause errors if "ExpandedRowCount" is
                /// set on this table, therefore we simple remove it.
                var ExpandedRowCountAttribute = tableNpde.Attributes.OfType<XmlAttribute>().Where(x => x.Name == "ss:ExpandedRowCount").FirstOrDefault();
                if (ExpandedRowCountAttribute != null)
                {
                    tableNpde.Attributes.Remove(ExpandedRowCountAttribute);
                }
                result = new ExcelXmlSheetRow(node, m_NamespaceManager, this);
                m_Rows.Add(result);
                result.CellValuesChanged("");
                //result = Rows[Rows.Count - 1];
            }
            return result;
        }
        public ExcelXmlSheetRow InsertRow(ExcelXmlSheetRow prevRow)
        {
            ExcelXmlSheetRow result = null;
            //var lastRow = Rows.Count > 0
            //    ? Rows[Rows.Count - 1]
            //    : null;
            var tableNpde = GetTableNode();
            if (prevRow != null && tableNpde != null)
            {
                var node = tableNpde.InsertAfter(prevRow.XmlNode.CloneNode(true), prevRow.XmlNode);
                /// We are adding a new row that may cause errors if "ExpandedRowCount" is
                /// set on this table, therefore we simple remove it.
                var ExpandedRowCountAttribute = tableNpde.Attributes.OfType<XmlAttribute>().Where(x => x.Name == "ss:ExpandedRowCount").FirstOrDefault();
                if (ExpandedRowCountAttribute != null)
                {
                    tableNpde.Attributes.Remove(ExpandedRowCountAttribute);
                }
                result = new ExcelXmlSheetRow(node, m_NamespaceManager, this);
                m_Rows.Add(result);
                result.CellValuesChanged("");
                //result = Rows[Rows.Count - 1];
            }
            return result;

        }

        protected int HeaderRow = 0;

        /// <summary>
        /// Returns a column number based on a 'header text' in column headings.
        /// This assumes that the first row of the sheet contains column names and 
        /// returns the index of such matching column.
        /// </summary>
        /// <param name="headerText"></param>
        /// <returns></returns>
        public int? GetColumnNumber(string headerText)
        {
            var rows = Rows;
            if (rows.Count > HeaderRow)
            {
                var cells = rows[HeaderRow].Cells;
                for (int idx = 0; idx < cells.Count; idx++)
                {
                    if (cells[idx].Value == headerText)
                        return idx;
                }
            }
            return null;
        }
        public virtual void OnBeforeSave()
        {

        }

        public ExcelXmlWorksheet Copy(ExcelXmlFile destination, string newName)
        {
            if (string.IsNullOrEmpty(newName))
                newName = Name;
            destination.DeleteWorksheet(newName);
            var importedNode = destination.XmlDocument.ImportNode(m_Node.CloneNode(true), true);
            var cloned = destination.GetWorkbookXmlNode().AppendChild(importedNode) as XmlElement;
            cloned.Attributes["ss:Name"].Value = newName;
            m_Parent.CopyStyles(destination);
            return destination.GetByName(newName);
        }
        public virtual string GetKey(ExcelXmlSheetRow row)
        {
            return Guid.NewGuid().ToString();
        }
        protected void ReIndex(bool refresh = true)
        {
            if (m_Index == null || refresh)
            {
                m_Index = new Dictionary<string, List<ExcelXmlSheetRow>>();
                foreach (var row in Rows)
                {
                    var key = GetKey(row);
                    if (m_Index.ContainsKey(key))
                    {
                        m_Index[key].Add(row);
                    }
                    else
                    {
                        var lst = new List<ExcelXmlSheetRow>();
                        lst.Add(row);
                        m_Index.Add(key, lst);
                    }
                }
            }
        }
        protected List<ExcelXmlSheetRow> _GetByKey(string key)
        {
            if (m_Index == null)
                ReIndex();
            List<ExcelXmlSheetRow> result = m_Index == null
                ? null
                : m_Index.TryGetValue(key, out result)
                    ? result
                    : null;
            return result;

        }
        public virtual void ValueChange(ExcelXmlSheetRow row, string oldKey, string newKey)
        {
            if (ByPassIndexing)
                return;
            var oldList = _GetByKey(oldKey);
            if (m_Index != null)
            {
                if (m_Index.ContainsKey(newKey))
                {
                    m_Index[newKey].Add(row);
                }
                else
                {
                    var lst = new List<ExcelXmlSheetRow>();
                    lst.Add(row);
                    m_Index.Add(newKey, lst);
                }
            }
        }

        public bool AddLanguageColumn(string langId)
        {
            if (!GetColumnNumber(langId).HasValue)
            {
                var rows = Rows;
                foreach (var row in rows)
                {
                    row.AppenCell().Value = "";
                }
                rows[0].GetCells(true)[rows[0].Cells.Count - 1].Value = langId;
            }

            return GetColumnNumber(langId).HasValue;


        }
        public bool DeleteRow(ExcelXmlSheetRow row)
        {
            try
            {
                var tableNpde = GetTableNode();
                if (tableNpde != null)
                {
                    tableNpde.RemoveChild(row.XmlNode);
                    m_Rows = null;
                }
                return true;
            }
            catch { }
            return false;


        }
    }

    public class ExcelXmlWorksheetBase : ExcelXmlBaseNode
    {
        internal ExcelXmlWorksheetBase(XmlNode node, XmlNamespaceManager namespaceManager)//, List<ExcelXmlSheetRow> rows, ExcelXmlFile parent)
            : base(node, namespaceManager)
        {
            // m_Rows = rows;
            // m_Parent = parent;
        }

    }
    public class ExcelXmlWorksheet<T> : ExcelXmlWorksheetBase
        where T : ExcelXmlSheetRow
    {
        protected List<T> m_Rows;
        private ExcelXmlFile m_Parent;
        private Dictionary<string, List<ExcelXmlSheetRow>> m_Index;
        internal ExcelXmlWorksheet(XmlNode node, XmlNamespaceManager namespaceManager, List<ExcelXmlSheetRow> rows, ExcelXmlFile parent)
            : base(node, namespaceManager)
        {
            m_Rows = rows == null
                ? null
                : rows
                    .Select(x => ConvertRow(x))
                    .ToList();
            m_Parent = parent;
        }

        public virtual T ConvertRow(ExcelXmlSheetRow row)
        {
            return row as T;
        }
        public ExcelXmlFile ParentFile { get { return m_Parent; } }

        /// <summary>
        /// Returns the worksheet name.
        /// It assumes that worksheet names are stored as 'Xml Attributes' such as in:
        /// (Worksheet ss:Name='some name')
        /// </summary>
        public string Name
        {
            get
            {
                return m_Node == null ||
                    m_Node.Attributes.OfType<XmlAttribute>().Where(x => x.Name == "ss:Name").FirstOrDefault() == null
                    ? null
                    : m_Node.Attributes.OfType<XmlAttribute>().Where(x => x.Name == "ss:Name").FirstOrDefault().Value;
            }
        }
        /// <summary>
        /// Returns the first 'Table' node of this worksheet.
        /// Note that in case of trasnlation sheets, we always assume that
        /// each sheet contians only a single 'Table'.
        /// </summary>
        /// <returns></returns>
        private XmlNode GetTableNode()
        {
            return m_Node == null
                ? null
                : m_Node.SelectSingleNode("./ns:Table", m_NamespaceManager);

        }


        public virtual T Factory(XmlNode node, XmlNamespaceManager namespaceManager, ExcelXmlWorksheetBase parent)
        {
            return null;
        }
        public List<T> GetRows(bool refresh = false)
        {

            if (m_Rows == null || refresh)
            {
                var tableNode = m_Node == null
                    ? null
                    : m_Node.SelectSingleNode("./ns:Table", m_NamespaceManager);
                m_Rows = tableNode == null
                    ? new List<T>()
                    : tableNode.SelectNodes("./ns:Row", m_NamespaceManager)
                                .OfType<XmlNode>()
                                .Select(x => Factory(x, m_NamespaceManager, this))
                                .ToList();
            }
            return m_Rows;

        }
        protected void Invalidate()
        {
            m_Rows = null;
        }

        /// <summary>
        /// Returns the list of 'Rows' for this worksheet.
        /// </summary>
        public List<T> Rows { get { return GetRows(); } }
        /// <summary>
        /// Appends a new row to the 'Table' node of the worksheet
        /// by cloning the last row.
        /// It may return NULL when the table doesnot contain any row.
        /// </summary>
        /// <returns></returns>
        public T AppendRow()
        {
            T result = null;
            var lastRow = Rows.Count > 0
                ? Rows[Rows.Count - 1]
                : null;
            var tableNpde = GetTableNode();
            if (lastRow != null && tableNpde != null)
            {
                var node = tableNpde.AppendChild(lastRow.XmlNode.CloneNode(true));
                /// We are adding a new row that may cause errors if "ExpandedRowCount" is
                /// set on this table, therefore we simple remove it.
                var ExpandedRowCountAttribute = tableNpde.Attributes.OfType<XmlAttribute>().Where(x => x.Name == "ss:ExpandedRowCount").FirstOrDefault();
                if (ExpandedRowCountAttribute != null)
                {
                    tableNpde.Attributes.Remove(ExpandedRowCountAttribute);
                }
                result = Factory(node, m_NamespaceManager, this);
                m_Rows.Add(result);
                result.CellValuesChanged("");
                //result = Rows[Rows.Count - 1];
            }
            return result;
        }

        /// <summary>
        /// Returns a column number based on a 'header text' in column headings.
        /// This assumes that the first row of the sheet contains column names and 
        /// returns the index of such matching column.
        /// </summary>
        /// <param name="headerText"></param>
        /// <returns></returns>
        public int? GetColumnNumber(string headerText)
        {
            var rows = Rows;
            if (rows.Count > 0)
            {
                var cells = rows[0].Cells;
                for (int idx = 0; idx < cells.Count; idx++)
                {
                    if (cells[idx].Value == headerText)
                        return idx;
                }
            }
            return null;
        }

        public ExcelXmlWorksheet Copy(ExcelXmlFile destination, string newName)
        {
            if (string.IsNullOrEmpty(newName))
                newName = Name;
            destination.DeleteWorksheet(newName);
            var importedNode = destination.XmlDocument.ImportNode(m_Node.CloneNode(true), true);
            var cloned = destination.GetWorkbookXmlNode().AppendChild(importedNode) as XmlElement;
            cloned.Attributes["ss:Name"].Value = newName;
            m_Parent.CopyStyles(destination);
            return destination.GetByName(newName);
        }
        public virtual string GetKey(ExcelXmlSheetRow row)
        {
            return Guid.NewGuid().ToString();
        }
        protected void ReIndex()
        {
            m_Index = new Dictionary<string, List<ExcelXmlSheetRow>>();
            foreach (var row in Rows)
            {
                var key = GetKey(row);
                if (m_Index.ContainsKey(key))
                {
                    m_Index[key].Add(row);
                }
                else
                {
                    var lst = new List<ExcelXmlSheetRow>();
                    lst.Add(row);
                    m_Index.Add(key, lst);
                }
            }
        }
        protected List<ExcelXmlSheetRow> _GetByKey(string key)
        {
            if (m_Index == null)
                ReIndex();
            List<ExcelXmlSheetRow> result = m_Index == null
                ? null
                : m_Index.TryGetValue(key, out result)
                    ? result
                    : null;
            return result;

        }
        public virtual void ValueChange(ExcelXmlSheetRow row, string oldKey, string newKey)
        {
            var oldList = _GetByKey(oldKey);
            if (m_Index != null)
            {
                if (m_Index.ContainsKey(newKey))
                {
                    m_Index[newKey].Add(row);
                }
                else
                {
                    var lst = new List<ExcelXmlSheetRow>();
                    lst.Add(row);
                    m_Index.Add(newKey, lst);
                }
            }
        }

        public bool DeleteRow(ExcelXmlSheetRow row)
        {
            try
            {
                var tableNpde = GetTableNode();
                if (tableNpde != null)
                {
                    tableNpde.RemoveChild(row.XmlNode);
                    m_Rows = null;
                }
                return true;
            }
            catch { }
            return false;


        }

    }
}
