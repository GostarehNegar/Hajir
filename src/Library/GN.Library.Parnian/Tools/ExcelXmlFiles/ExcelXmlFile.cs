using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;



namespace GN.Library.Parnian.Tools.ExcelFiles
{
    /// <summary>
    /// A class to work with 'Crm Translation Excel Xnl' files.
    /// These files are generated using the 'Export Translation' functionality of MS-CRM. 
    /// The generated file normally inculdes three sheets: 'Information', 'Display Strings' and 'Localized Labels' sheets.
    /// Use this class to access these files, edit strings and save them to new files.
    /// 
    /// </summary>
    public class ExcelXmlFile
    {
        XmlDocument m_XmlDoc;
        XmlNamespaceManager _nameSpace;
        private string m_FileName;
        private static ExcelXmlFile m_Template;
        private List<ExcelXmlStyle> m_Styles;
        private List<ExcelXmlWorksheet> m_Worksheets;

        //private string LocaliedLabelsWorksheeName = "Localized Labels";
        //private string DisplayStringsWorksheeName = "Display Strings";
        //private string InformationWorksheeName = "Information";
        //private const string StringsWorksheeName = "Strings.dll";
        //private const string WebResourcesWorksheetName = "WebResources";

        private string LoadTemplate()
        {
            var result = "";
            var asm = this.GetType().Assembly;
            var name = asm.GetManifestResourceNames()
                .Where(x => x != null && x.ToLowerInvariant().Contains("template"))
                .FirstOrDefault();
            if (!string.IsNullOrEmpty(name))
            {
                var stream = asm.GetManifestResourceStream(name);
                var reader = new System.IO.StreamReader(stream);
                result = reader.ReadToEnd();
            }
            return result;

        }

        public XmlDocument XmlDocument { get { return m_XmlDoc; } }
        public static ExcelXmlFile Template
        {
            get
            {
                if (m_Template == null)
                {
                    m_Template = new ExcelXmlFile();
                    m_Template.loadFromTemplate();


                }
                return m_Template;
            }
        }
        private void loadFromTemplate()
        {
            var txt = LoadTemplate();
            m_XmlDoc = new XmlDocument();
            m_XmlDoc.LoadXml(txt);
            Init();
            //_nameSpace = new XmlNamespaceManager(m_XmlDoc.NameTable);
            //_nameSpace.AddNamespace("ns", "urn:schemas-microsoft-com:office:spreadsheet");
            //_nameSpace.AddNamespace("o", "urn:schemas-microsoft-com:office:office");
            //_nameSpace.AddNamespace("x", "urn:schemas-microsoft-com:office:excel");
            //_nameSpace.AddNamespace("ss", "urn:schemas-microsoft-com:office:spreadsheet");
        }
        public ExcelXmlFile Clone()
        {
            var result = new ExcelXmlFile();
            result.m_XmlDoc = m_XmlDoc;
            result.m_FileName = m_FileName;
            result._nameSpace = _nameSpace;
            return result;
        }

        protected virtual void Init()
        {
            _nameSpace = new XmlNamespaceManager(m_XmlDoc.NameTable);
            _nameSpace.AddNamespace("ns", "urn:schemas-microsoft-com:office:spreadsheet");
            _nameSpace.AddNamespace("o", "urn:schemas-microsoft-com:office:office");
            _nameSpace.AddNamespace("x", "urn:schemas-microsoft-com:office:excel");
            _nameSpace.AddNamespace("ss", "urn:schemas-microsoft-com:office:spreadsheet");
            GetWorksheets(true);
        }


        /// <summary>
        /// Opens a 'Crm Translation Xml Excel' file.
        /// It just opens the file as a normal 'Xml Document', one may use Validate
        /// to ensure that the file is valid.
        /// </summary>
        /// <param name="fileName"></param>
        public void Open(string fileName)
        {
            m_XmlDoc = new XmlDocument();
            m_XmlDoc.Load(fileName);
            m_FileName = fileName;
            /// Befrore using the Excel Xml file,
            /// we need to setup the 'Name Space Manager'
            /// (reference: http://stackoverflow.com/questions/4171451/xmldocument-selectsinglenode-and-xmlnamespace-issue).
            /// We have following important namespaces extreacted from 'CrmTranslation.Xml'.
            //<Workbook 
            //    xmlns="urn:schemas-microsoft-com:office:spreadsheet" 
            //    xmlns:o="urn:schemas-microsoft-com:office:office" 
            //    xmlns:x="urn:schemas-microsoft-com:office:excel" 
            //    xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet" 
            //    xmlns:html="http://www.w3.org/TR/REC-html40">
            //<DocumentProperties xmlns="urn:schemas-microsoft-com:office:office">
            //</DocumentProperties><ExcelWorkbook xmlns="urn:schemas-microsoft-com:office:excel">
            Init();
            //_nameSpace = new XmlNamespaceManager(m_XmlDoc.NameTable);
            //_nameSpace.AddNamespace("ns", "urn:schemas-microsoft-com:office:spreadsheet");
            //_nameSpace.AddNamespace("o", "urn:schemas-microsoft-com:office:office");
            //_nameSpace.AddNamespace("x", "urn:schemas-microsoft-com:office:excel");
            //_nameSpace.AddNamespace("ss", "urn:schemas-microsoft-com:office:spreadsheet");
        }
        public void Load(string content)
        {
            m_XmlDoc = new XmlDocument();
            m_XmlDoc.LoadXml(content);
            m_FileName = "";
            /// Befrore using the Excel Xml file,
            /// we need to setup the 'Name Space Manager'
            /// (reference: http://stackoverflow.com/questions/4171451/xmldocument-selectsinglenode-and-xmlnamespace-issue).
            /// We have following important namespaces extreacted from 'CrmTranslation.Xml'.
            //<Workbook 
            //    xmlns="urn:schemas-microsoft-com:office:spreadsheet" 
            //    xmlns:o="urn:schemas-microsoft-com:office:office" 
            //    xmlns:x="urn:schemas-microsoft-com:office:excel" 
            //    xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet" 
            //    xmlns:html="http://www.w3.org/TR/REC-html40">
            //<DocumentProperties xmlns="urn:schemas-microsoft-com:office:office">
            //</DocumentProperties><ExcelWorkbook xmlns="urn:schemas-microsoft-com:office:excel">
            Init();
            //_nameSpace = new XmlNamespaceManager(m_XmlDoc.NameTable);
            //_nameSpace.AddNamespace("ns", "urn:schemas-microsoft-com:office:spreadsheet");
            //_nameSpace.AddNamespace("o", "urn:schemas-microsoft-com:office:office");
            //_nameSpace.AddNamespace("x", "urn:schemas-microsoft-com:office:excel");
            //_nameSpace.AddNamespace("ss", "urn:schemas-microsoft-com:office:spreadsheet");

        }

        public void Create()
        {
            var txt = LoadTemplate();
            m_XmlDoc = new XmlDocument();
            m_XmlDoc.LoadXml(txt);
            _nameSpace = new XmlNamespaceManager(m_XmlDoc.NameTable);
            _nameSpace.AddNamespace("ns", "urn:schemas-microsoft-com:office:spreadsheet");
            _nameSpace.AddNamespace("o", "urn:schemas-microsoft-com:office:office");
            _nameSpace.AddNamespace("x", "urn:schemas-microsoft-com:office:excel");
            _nameSpace.AddNamespace("ss", "urn:schemas-microsoft-com:office:spreadsheet");
        }
        public XmlNode GetWorkbookXmlNode()
        {
            return m_XmlDoc == null
                ? null
                : m_XmlDoc.SelectSingleNode("//ns:Workbook", _nameSpace);
        }

        /// <summary>
        /// Saves the file with the same name used to open it.
        /// </summary>
        public void Save()
        {
            Save(m_FileName);
        }
        /// <summary>
        /// Saves the 'Translation Xml' with the 'file name' supplied.
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName)
        {
            foreach (var sheet in GetWorksheets())
            {
                sheet.OnBeforeSave();
            }
            m_XmlDoc.Save(fileName);
            m_FileName = fileName;
        }

        private int GetWorksheetsCount()
        {
            return m_XmlDoc == null
                ? 0
                : m_XmlDoc.SelectNodes("//ns:Worksheet", _nameSpace).OfType<XmlNode>().Count();
        }

        /// <summary>
        /// Gets a list of all worksheets.
        /// </summary>
        /// <returns></returns>
        private List<ExcelXmlWorksheet> GetWorksheets(bool refrseh = false)
        {
            if (m_Worksheets == null || m_Worksheets.Count != GetWorksheetsCount() || refrseh)
            {
                m_Worksheets = m_XmlDoc == null
                    ? new List<ExcelXmlWorksheet>()
                    : m_XmlDoc
                        .SelectNodes("//ns:Worksheet", _nameSpace).OfType<XmlNode>()
                        .Select(x => new ExcelXmlWorksheet(x, _nameSpace, null, this))
                        .ToList();
            }
            return m_Worksheets;
        }

        /// <summary>
        /// Gets a list of worksheets in this workbook.
        /// </summary>
        public List<ExcelXmlWorksheet> Worksheets { get { return GetWorksheets(); } }

        public void CopyWorksheet(ExcelXmlFile destination, string worksheetName)
        {
            var wk = GetByName(worksheetName).XmlNode;
            var cloned = destination.GetWorkbookXmlNode().AppendChild(wk.CloneNode(true)) as XmlElement;
            cloned.Attributes["ss:Name"].Value = "Name1";
            //cloned.Attributes["ss:Id"].Value = "Name1";




        }
        public void Test()
        {

            return;

        }

        /// <summary>
        /// Returns the file name used to open this 'Crm Translation Xml Excel' file.
        /// </summary>
        public string FileName { get { return m_FileName; } }

        /// <summary>
        /// Gets a worksheet using the worksheet name.
        /// </summary>
        /// <param name="worksheetName"></param>
        /// <returns></returns>
        public ExcelXmlWorksheet GetByName(string worksheetName)
        {
            return GetWorksheets().Where(x => x.Name == worksheetName).FirstOrDefault();
        }
        public T GetByNameAs<T>(string worksheetName, Func<ExcelXmlWorksheet, T> factory, bool createIfNotFound = false)
            where T : ExcelXmlWorksheet
        {
            var result = GetWorksheets().Where(x => x.Name == worksheetName).FirstOrDefault();
            if (result == null && createIfNotFound && Template != null)
            {
                var src = Template.GetByName(worksheetName);
                if (src != null)
                {
                    src.Copy(this, null);
                    result = GetWorksheets().Where(x => x.Name == worksheetName).FirstOrDefault();
                }

            }
            if (result != null && (result as T) == null)
            {
                m_Worksheets.Remove(result);
                result = factory(result);
                m_Worksheets.Add(result);
            }

            return result as T;


        }










        ///// <summary>
        ///// Returns the 'Display Strings' worksheet.
        ///// Returns Null, if such worksheet does not exist in this workbook.
        ///// </summary>
        //public DisplayStringsXmlWorksheet DisplayStrings
        //{
        //    get
        //    {
        //        return GetByNameAs<DisplayStringsXmlWorksheet>(DisplayStringsWorksheeName, x =>
        //        {
        //            return new DisplayStringsXmlWorksheet(x);
        //        });
        //        //return GetByName(DisplayStringsWorksheeName) == null
        //        //    ? null
        //        //    : new DisplayStringsXmlWorksheet(GetByName(DisplayStringsWorksheeName));//.XmlNode, _nameSpace, this);
        //    }
        //}
        //public ResourceWorksheet GetResourceWorksheet(string tableName, bool createIfNotFound = false)
        //{
        //    var result = GetByNameAs<ResourceWorksheet>(tableName, x =>
        //    {
        //        return new ResourceWorksheet(x);
        //    }, createIfNotFound);
        //    if (result == null && createIfNotFound)
        //    {
        //        var tempate = Template;
        //        if (tempate != null)
        //        {
        //            var src = tempate.GetResourceWorksheet(KnownDllFiles.GetByType(KnownDllType.Strings).ExcelTableName);
        //            if (src != null)
        //            {
        //                src.Copy(this, tableName);
        //                result = GetResourceWorksheet(tableName, false);

        //            }
        //        }

        //    }
        //    return result;
        //}
        public void DeleteWorksheet(string worksheetName)
        {
            var node = GetWorkbookXmlNode();
            if (GetByName(worksheetName) != null)
                node.RemoveChild(GetByName(worksheetName).XmlNode);

        }

        public virtual bool Validate(bool Throw)
        {
            bool result = true;
            //result = result && LocalizedLabels != null && LocalizedLabels.XmlNode != null;

            //if (!result && Throw)
            //{
            //    throw new InvalidProgramException(
            //        string.Format("Invalid File. Valid 'Excel Translation Xml' files should have a 'Localized Labels' worksheet. We found no worksheet with that name" +
            //        " in this file. FileName:{0}", m_FileName
            //        ));
            //}
            //result = result && LocalizedLabels.Validate(Throw);

            //result = result && Information != null && Information.XmlNode != null;
            //if (!result && Throw)
            //{
            //    throw new InvalidProgramException(
            //        string.Format("Invalid File. Valid 'Excel Translation Xml' files should have a 'Information' worksheet. We found no worksheet with that name" +
            //        " in this file. FileName:{0}", m_FileName
            //        ));
            //}
            //result = result && DisplayStrings != null && DisplayStrings.XmlNode != null;
            //if (!result && Throw)
            //{
            //    throw new InvalidProgramException(
            //        string.Format("Invalid File. Valid 'Excel Translation Xml' files should have a 'Display Strings' worksheet. We found no worksheet with that name" +
            //        " in this file. FileName:{0}", m_FileName
            //        ));
            //}
            //result = result && DisplayStrings != null && DisplayStrings.Validate(Throw);
            return result;
        }

        private int GetStylesCount()
        {
            return m_XmlDoc == null
                ? 0
                : m_XmlDoc.SelectNodes("//ns:Style", _nameSpace).OfType<XmlNode>().Count();
        }
        public List<ExcelXmlStyle> GetStyles()
        {
            if (m_Styles == null || m_Styles.Count != GetStylesCount())
            {
                m_Styles = m_XmlDoc == null
                ? new List<ExcelXmlStyle>()
                : m_XmlDoc
                    .SelectNodes("//ns:Style", _nameSpace).OfType<XmlNode>()
                    .Select(x => new ExcelXmlStyle(x, _nameSpace))
                    .ToList();
            }
            return m_Styles;
        }
        public void CopyStyles(ExcelXmlFile destination)
        {
            var styles = this.GetStyles();
            var destStyles = destination.GetStyles();
            var destStylesNode = destination.m_XmlDoc.SelectSingleNode("//ns:Styles", _nameSpace);
            foreach (var style in styles)
            {
                var dest = destStyles.Where(x => x.Id == style.Id).FirstOrDefault();
                if (dest == null)
                {
                    var importedNode = destination.m_XmlDoc.ImportNode(style.XmlNode.CloneNode(true), true);
                    destStylesNode.AppendChild(importedNode);
                }
            }
        }




        /// <summary>
        /// A class representing a 'Row' in an 'Excel Xml' sheet.
        /// Use 'Cells' property to access row cells.
        /// </summary>




    }
}
