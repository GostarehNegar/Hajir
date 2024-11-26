using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xrm.Tools.WebAPI.Results
{
    public class CRMAttributeDisplayName
    {
        public Guid MetadataId { get; set; }
        public string LogicalName { get; set; }
        public string SchemaName { get; set; }
        public bool IsPrimaryId { get; set; }
        public string AttributeType { get; set; }
        public string ODataLogicalName { get; set; }
        public string DisplayName { get; set; }
        public MyLabel DisplayNameEx { get; set; }
        public MyLabel Description { get; set; }
        public string LogicalDisplayName { get; set; }
        public bool IsCustomAttribute { get; set; }
        public List<string> Targets { get; set; }
        public List<RelationShipMetadata> Relationships {get;set;}
        public MyOptionSetMetadata Options { get; set; }
        
        public AttributeRequiredLevel RequiredLevel { get; set; }


    }
    public class MyRequiredLevel
    {
        public string Value { get; set; }

    }
    public class MyLocalizedLabel
    {
        public string Label { get; set; }
        public long LangCode { get; set; }
    }
    public class MyLabel
    {
        public MyLocalizedLabel[] LocalizedLabels { get; set; } = new MyLocalizedLabel[] { };
        public MyLocalizedLabel UserLocalizedLabel { get; set; }
        public string GetLabel(int langcode)
        {
            return (this.LocalizedLabels ?? Array.Empty<MyLocalizedLabel>())
                .FirstOrDefault(x => x.LangCode == langcode)?.Label ?? UserLocalizedLabel?.Label;
                
        }
        public void AddLabel(MyLocalizedLabel lbl)
        {
            if (lbl != null)
            {
                this.LocalizedLabels = (this.LocalizedLabels ?? new MyLocalizedLabel[] { })
                    .Append(lbl)
                    .ToArray();
            }
        }
    }
    public class MyOptionSetMetadata
    {
        public class OptionSetClass
        {
            public class LocalizedLabelClass
            {
                public string Label { get; set; }
                public int LanguageCode { get; set; }
            }
            public class LabelClass
            {
                public LocalizedLabelClass[] LocalizedLabels { get; set; }
                public LocalizedLabelClass UserLocalizedLabel { get; set; }

            }
            public class OptionsClass
            {
                public int Value { get; set; }
                public LabelClass Label { get; set; }
                public LabelClass Description { get; set; }
            }
            public OptionsClass[] Options { get; set; }
            public LabelClass DisplayName { get; set; }
            public LabelClass Description { get; set; }
        }

        public string LogicalName { get; set; }
        public OptionSetClass OptionSet { get; set; }
    }
    public class LookupAttributeData
	{
		public string LogicalName { get; set; }
		public Guid MetadataId { get; set; }
		public List<string> Targets { get; set; }
	}
    public class RelationShipMetadata
    {
        public string ReferencingAttribute { get; set; }
        public string ReferencingEntityNavigationPropertyName { get; set; }

        public IDictionary<string,object> MoreData { get; set; }

    }
}
