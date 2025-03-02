using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.StdSolution
{
    [EntityLogicalName(Schema.LogicalName)]
    public class XrmProduct : XrmEntity<XrmProduct, LibraryConstants.Schema.Product.StateCodes, LibraryConstants.Schema.Product.StatusCodes>
    {
        public new class Schema : LibraryConstants.Schema.Product
        {

        }
        public XrmProduct() : base(Schema.LogicalName)
        {

        }
        [AttributeLogicalName(Schema.ProductId)]
        public System.Nullable<System.Guid> ProductId
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.ProductId);
            }
            set
            {
                this.SetAttributeValue(Schema.ProductId, value);
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



        [AttributeLogicalNameAttribute(Schema.ProductId)]
        public override System.Guid Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                this.ProductId = value;
            }
        }

        [AttributeLogicalNameAttribute(Schema.Name)]
        public string Name { get { return this.GetAttributeValue<string>(Schema.Name); } set { this.SetAttribiuteValue(Schema.Name, value); } }

        [AttributeLogicalNameAttribute(Schema.ProductNumber)]
        public string ProductNumber { get { return this.GetAttributeValue<string>(Schema.ProductNumber); } set { this.SetAttribiuteValue(Schema.ProductNumber, value); } }

        [AttributeLogicalNameAttribute(Schema.DefaultUoMId)]
        public EntityReference DefaultUoMIdRef
        {
            get => this.GetAttributeValue<EntityReference>(Schema.DefaultUoMId);
            set => this.SetAttributeValue(Schema.DefaultUoMId, value);
        }

        [AttributeLogicalNameAttribute(Schema.DefaultUoMId)]
        public Guid? DefaultUoMId
        {
            get => this.DefaultUoMIdRef?.Id;
            set => this.DefaultUoMIdRef = value.HasValue ? new EntityReference(XrmUnitOfMeasure.Schema.LogicalName, value.Value) : null;
        }

        [AttributeLogicalNameAttribute(Schema.DefaultUomScheduleId)]
        public EntityReference DefaultUomScheduleIdRef
        {
            get => this.GetAttributeValue<EntityReference>(Schema.DefaultUomScheduleId);
            set => this.SetAttributeValue(Schema.DefaultUomScheduleId, value);
        }
        [AttributeLogicalNameAttribute(Schema.DefaultUomScheduleId)]
        public Guid? DefaultUomScheduleId
        {
            get => this.DefaultUomScheduleIdRef?.Id;
            set => this.DefaultUomScheduleIdRef = value.HasValue ? new EntityReference(XrmUnitOfMeasurementGroup.Schema.LogicalName, value.Value) : null;
        }

        [AttributeLogicalNameAttribute(Schema.QuantityDecimal)]
        public int? QuantityDecimal
        {
            get => this.GetAttributeValue<int?>(Schema.QuantityDecimal);
            set => this.SetAttributeValue(Schema.QuantityDecimal, value);
        }
    }
}
