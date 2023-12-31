﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace GN.Library.SharePoint.SP2010.WebReferences.Dsp
{
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="StsAdapterSoap", Namespace="http://schemas.microsoft.com/sharepoint/dsp")]
    public partial class StsAdapter : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private Authentication authenticationField;
        
        private DataRoot dataRootField;
        
        private RequestHeader requestField;
        
        private Versions versionsField;
        
        private System.Threading.SendOrPostCallback QueryOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public StsAdapter() {
            this.Url = "http://projects.gnco.ir/_vti_bin/DspSts.asmx";
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public Authentication authentication {
            get {
                return this.authenticationField;
            }
            set {
                this.authenticationField = value;
            }
        }
        
        public DataRoot dataRoot {
            get {
                return this.dataRootField;
            }
            set {
                this.dataRootField = value;
            }
        }
        
        public RequestHeader request {
            get {
                return this.requestField;
            }
            set {
                this.requestField = value;
            }
        }
        
        public Versions versions {
            get {
                return this.versionsField;
            }
            set {
                this.versionsField = value;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event QueryCompletedEventHandler QueryCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapHeaderAttribute("request")]
        [System.Web.Services.Protocols.SoapHeaderAttribute("authentication")]
        [System.Web.Services.Protocols.SoapHeaderAttribute("dataRoot")]
        [System.Web.Services.Protocols.SoapHeaderAttribute("versions", Direction=System.Web.Services.Protocols.SoapHeaderDirection.InOut)]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://schemas.microsoft.com/sharepoint/dsp/queryRequest", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("queryResponse", Namespace="http://schemas.microsoft.com/sharepoint/dsp")]
        public System.Xml.XmlNode Query([System.Xml.Serialization.XmlElementAttribute(Namespace="http://schemas.microsoft.com/sharepoint/dsp", IsNullable=true)] QueryRequest queryRequest) {
            object[] results = this.Invoke("Query", new object[] {
                        queryRequest});
            return ((System.Xml.XmlNode)(results[0]));
        }
        
        /// <remarks/>
        public void QueryAsync(QueryRequest queryRequest) {
            this.QueryAsync(queryRequest, null);
        }
        
        /// <remarks/>
        public void QueryAsync(QueryRequest queryRequest, object userState) {
            if ((this.QueryOperationCompleted == null)) {
                this.QueryOperationCompleted = new System.Threading.SendOrPostCallback(this.OnQueryOperationCompleted);
            }
            this.InvokeAsync("Query", new object[] {
                        queryRequest}, this.QueryOperationCompleted, userState);
        }
        
        private void OnQueryOperationCompleted(object arg) {
            if ((this.QueryCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.QueryCompleted(this, new QueryCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3761.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/sharepoint/dsp")]
    [System.Xml.Serialization.XmlRootAttribute("request", Namespace="http://schemas.microsoft.com/sharepoint/dsp", IsNullable=false)]
    public partial class RequestHeader : System.Web.Services.Protocols.SoapHeader {
        
        private DocumentType documentField;
        
        private MethodType methodField;
        
        private System.Xml.XmlAttribute[] anyAttrField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public DocumentType document {
            get {
                return this.documentField;
            }
            set {
                this.documentField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public MethodType method {
            get {
                return this.methodField;
            }
            set {
                this.methodField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAnyAttributeAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr {
            get {
                return this.anyAttrField;
            }
            set {
                this.anyAttrField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3761.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/sharepoint/dsp")]
    public enum DocumentType {
        
        /// <remarks/>
        content,
        
        /// <remarks/>
        system,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3761.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/sharepoint/dsp")]
    public enum MethodType {
        
        /// <remarks/>
        query,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3761.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/sharepoint/dsp")]
    public partial class PTQuery {
        
        private System.Xml.XmlElement[] anyField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlElement[] Any {
            get {
                return this.anyField;
            }
            set {
                this.anyField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3761.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/sharepoint/dsp")]
    public partial class OrderField {
        
        private string nameField;
        
        private OrderDirection directionField;
        
        public OrderField() {
            this.directionField = OrderDirection.ASC;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(OrderDirection.ASC)]
        public OrderDirection Direction {
            get {
                return this.directionField;
            }
            set {
                this.directionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3761.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/sharepoint/dsp")]
    public enum OrderDirection {
        
        /// <remarks/>
        ASC,
        
        /// <remarks/>
        DESC,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3761.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/sharepoint/dsp")]
    public partial class ServerParameter {
        
        private string nameField;
        
        private bool nullField;
        
        private string valueField;
        
        public ServerParameter() {
            this.nullField = false;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool Null {
            get {
                return this.nullField;
            }
            set {
                this.nullField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3761.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/sharepoint/dsp")]
    public partial class AllFields {
        
        private bool includeHiddenFieldsField;
        
        public AllFields() {
            this.includeHiddenFieldsField = false;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool IncludeHiddenFields {
            get {
                return this.includeHiddenFieldsField;
            }
            set {
                this.includeHiddenFieldsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3761.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/sharepoint/dsp")]
    public partial class Field {
        
        private string nameField;
        
        private string aliasField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Alias {
            get {
                return this.aliasField;
            }
            set {
                this.aliasField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3761.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/sharepoint/dsp")]
    public partial class Fields {
        
        private Field[] fieldField;
        
        private AllFields allFieldsField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Field")]
        public Field[] Field {
            get {
                return this.fieldField;
            }
            set {
                this.fieldField = value;
            }
        }
        
        /// <remarks/>
        public AllFields AllFields {
            get {
                return this.allFieldsField;
            }
            set {
                this.allFieldsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3761.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/sharepoint/dsp")]
    public partial class ServerParameterInfo {
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3761.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/sharepoint/dsp")]
    public partial class DspQuery {
        
        private ServerParameterInfo serverParameterInfoField;
        
        private Fields fieldsField;
        
        private ServerParameter[] serverParametersField;
        
        private System.Xml.XmlNode whereField;
        
        private OrderField[] orderByField;
        
        private long rowLimitField;
        
        public DspQuery() {
            this.rowLimitField = ((long)(-1));
        }
        
        /// <remarks/>
        public ServerParameterInfo ServerParameterInfo {
            get {
                return this.serverParameterInfoField;
            }
            set {
                this.serverParameterInfoField = value;
            }
        }
        
        /// <remarks/>
        public Fields Fields {
            get {
                return this.fieldsField;
            }
            set {
                this.fieldsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public ServerParameter[] ServerParameters {
            get {
                return this.serverParametersField;
            }
            set {
                this.serverParametersField = value;
            }
        }
        
        /// <remarks/>
        public System.Xml.XmlNode Where {
            get {
                return this.whereField;
            }
            set {
                this.whereField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public OrderField[] OrderBy {
            get {
                return this.orderByField;
            }
            set {
                this.orderByField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(typeof(long), "-1")]
        public long RowLimit {
            get {
                return this.rowLimitField;
            }
            set {
                this.rowLimitField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3761.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/sharepoint/dsp")]
    public partial class DSQuery {
        
        private DspQuery queryField;
        
        private string selectField;
        
        private ResultContentType resultContentField;
        
        private ColumnMappingType columnMappingField;
        
        private string resultNamespaceField;
        
        private string resultPrefixField;
        
        private string resultRootField;
        
        private string resultRowField;
        
        private string startPositionField;
        
        private string comparisonLocaleField;
        
        public DSQuery() {
            this.resultContentField = ResultContentType.both;
            this.columnMappingField = ColumnMappingType.element;
        }
        
        /// <remarks/>
        public DspQuery Query {
            get {
                return this.queryField;
            }
            set {
                this.queryField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string select {
            get {
                return this.selectField;
            }
            set {
                this.selectField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(ResultContentType.both)]
        public ResultContentType resultContent {
            get {
                return this.resultContentField;
            }
            set {
                this.resultContentField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(ColumnMappingType.element)]
        public ColumnMappingType columnMapping {
            get {
                return this.columnMappingField;
            }
            set {
                this.columnMappingField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string resultNamespace {
            get {
                return this.resultNamespaceField;
            }
            set {
                this.resultNamespaceField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string resultPrefix {
            get {
                return this.resultPrefixField;
            }
            set {
                this.resultPrefixField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string resultRoot {
            get {
                return this.resultRootField;
            }
            set {
                this.resultRootField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string resultRow {
            get {
                return this.resultRowField;
            }
            set {
                this.resultRowField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string startPosition {
            get {
                return this.startPositionField;
            }
            set {
                this.startPositionField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string comparisonLocale {
            get {
                return this.comparisonLocaleField;
            }
            set {
                this.comparisonLocaleField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3761.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/sharepoint/dsp")]
    public enum ResultContentType {
        
        /// <remarks/>
        both,
        
        /// <remarks/>
        schemaOnly,
        
        /// <remarks/>
        dataOnly,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3761.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/sharepoint/dsp")]
    public enum ColumnMappingType {
        
        /// <remarks/>
        element,
        
        /// <remarks/>
        attribute,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3761.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/sharepoint/dsp")]
    public partial class QueryRequest {
        
        private DSQuery dsQueryField;
        
        private PTQuery ptQueryField;
        
        /// <remarks/>
        public DSQuery dsQuery {
            get {
                return this.dsQueryField;
            }
            set {
                this.dsQueryField = value;
            }
        }
        
        /// <remarks/>
        public PTQuery ptQuery {
            get {
                return this.ptQueryField;
            }
            set {
                this.ptQueryField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3761.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/sharepoint/dsp")]
    [System.Xml.Serialization.XmlRootAttribute("authentication", Namespace="http://schemas.microsoft.com/sharepoint/dsp", IsNullable=false)]
    public partial class Authentication : System.Web.Services.Protocols.SoapHeader {
        
        private System.Xml.XmlElement[] anyField;
        
        private System.Xml.XmlAttribute[] anyAttrField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlElement[] Any {
            get {
                return this.anyField;
            }
            set {
                this.anyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAnyAttributeAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr {
            get {
                return this.anyAttrField;
            }
            set {
                this.anyAttrField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3761.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/sharepoint/dsp")]
    [System.Xml.Serialization.XmlRootAttribute("dataRoot", Namespace="http://schemas.microsoft.com/sharepoint/dsp", IsNullable=false)]
    public partial class DataRoot : System.Web.Services.Protocols.SoapHeader {
        
        private string rootField;
        
        private bool allowRemoteDataAccessField;
        
        private System.Xml.XmlAttribute[] anyAttrField;
        
        public DataRoot() {
            this.allowRemoteDataAccessField = true;
        }
        
        /// <remarks/>
        public string root {
            get {
                return this.rootField;
            }
            set {
                this.rootField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool allowRemoteDataAccess {
            get {
                return this.allowRemoteDataAccessField;
            }
            set {
                this.allowRemoteDataAccessField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAnyAttributeAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr {
            get {
                return this.anyAttrField;
            }
            set {
                this.anyAttrField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3761.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/sharepoint/dsp")]
    [System.Xml.Serialization.XmlRootAttribute("versions", Namespace="http://schemas.microsoft.com/sharepoint/dsp", IsNullable=false)]
    public partial class Versions : System.Web.Services.Protocols.SoapHeader {
        
        private string[] versionField;
        
        private System.Xml.XmlAttribute[] anyAttrField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("version")]
        public string[] version {
            get {
                return this.versionField;
            }
            set {
                this.versionField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAnyAttributeAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr {
            get {
                return this.anyAttrField;
            }
            set {
                this.anyAttrField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    public delegate void QueryCompletedEventHandler(object sender, QueryCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class QueryCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal QueryCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public System.Xml.XmlNode Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((System.Xml.XmlNode)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591