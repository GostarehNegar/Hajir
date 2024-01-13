using GN.Library.Xrm;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Infrastructure.Xrm.Data
{
    [EntityLogicalName(Schema.LogicalName)]
    public class XrmHajirProductSeries : XrmEntity<XrmHajirProductSeries, DefaultStateCodes, DefaultStatusCodes>
    {
        public new class Schema : XrmEntity.Schema
        {
            public const string SolutionPerfix = HajirCrmConstants.RahsamSolutionPerfix;
            public const string LogicalName = SolutionPerfix + "productseries";
            public const string ProductSeriesId = LogicalName + "id";
            public const string Name = SolutionPerfix + "name";

        }
        public XrmHajirProductSeries() : base(Schema.LogicalName) { }

        [AttributeLogicalName(Schema.ProductSeriesId)]
        public System.Nullable<System.Guid> ProductSeriesId
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.ProductSeriesId);
            }
            set
            {
                this.SetAttributeValue(Schema.ProductSeriesId, value);
                if (value.HasValue)
                {
                    base.Id = value.Value;
                }
                else
                {
                    base.Id = System.Guid.Empty;
                }
            }
        }

        [AttributeLogicalNameAttribute(Schema.ProductSeriesId)]
        public override System.Guid Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                this.ProductSeriesId = value;
            }
        }

        [AttributeLogicalNameAttribute(Schema.Name)]
        public string Name { get { return this.GetAttributeValue<string>(Schema.Name); } set { this.SetAttribiuteValue(Schema.Name, value); } }

    }
}
