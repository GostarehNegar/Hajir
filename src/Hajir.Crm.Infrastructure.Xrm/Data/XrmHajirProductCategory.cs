using GN.Library.Xrm;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Infrastructure.Xrm.Data
{
    [EntityLogicalName(Schema.LogicalName)]
    public class XrmHajirProductCategory : XrmEntity<XrmHajirProductCategory, DefaultStateCodes, DefaultStatusCodes>
    {
        public new class Schema : HajirCrmConstants.Schema.ProductCategory
        {

        }
        public XrmHajirProductCategory() : base(Schema.LogicalName)
        {

        }

        [AttributeLogicalName(Schema.ProductCategoryId)]
        public System.Nullable<System.Guid> ProductCategoryId
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.ProductCategoryId);
            }
            set
            {
                this.SetAttributeValue(Schema.ProductCategoryId, value);
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



        [AttributeLogicalNameAttribute(Schema.ProductCategoryId)]
        public override System.Guid Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                this.ProductCategoryId = value;
            }
        }

        [AttributeLogicalNameAttribute(Schema.Name)]
        public string Name { get { return this.GetAttributeValue<string>(Schema.Name); } set { this.SetAttribiuteValue(Schema.Name, value); } }

        [AttributeLogicalNameAttribute(Schema.Code)]
        public int? Code { get { return this.GetAttributeValue<int?>(Schema.Code); } set { this.SetAttribiuteValue(Schema.Code, value); } }

        [AttributeLogicalNameAttribute(Schema.ProductTypeCode)]
        public OptionSetValue ProductTypeCode
        {
            get { return this.GetAttributeValue<OptionSetValue>(Schema.ProductTypeCode); }
            set { this.SetAttribiuteValue(Schema.ProductTypeCode, value); }
        }
        [AttributeLogicalNameAttribute(Schema.ProductTypeCode)]
        public HajirCrmConstants.Schema.Product.ProductTypes? ProductType
        {
            get => (HajirCrmConstants.Schema.Product.ProductTypes?) this.ProductTypeCode?.Value;
            set => this.ProductTypeCode = value.HasValue ? new OptionSetValue((int)value.Value) : null;
        }

    }
}
