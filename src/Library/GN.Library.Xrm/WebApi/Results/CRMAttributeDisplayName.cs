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
        public string LogicalDisplayName { get; set; }
        public bool IsCustomAttribute { get; set; }
        public List<string> Targets { get; set; }
        public List<RelationShipMetadata> Relationships {get;set;}

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
