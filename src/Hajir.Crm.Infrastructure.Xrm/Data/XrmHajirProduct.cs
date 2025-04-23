using GN.Library.Xrm.StdSolution;
using Hajir.Crm.Entities;
using Hajir.Crm.Infrastructure.Xrm.Data;
using Hajir.Crm.Products;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Entities
{
    [EntityLogicalName(Schema.LogicalName)]
    public class XrmHajirProduct : XrmProduct
    {
        public new class Schema : HajirProductEntity.Schema
        {

        }

        [AttributeLogicalName(Schema.CategoryId)]
        public EntityReference Category
        { 
            get => this.GetAttributeValue<EntityReference>(Schema.CategoryId); 
            set => this.SetAttributeValue(Schema.CategoryId, value); }
        
        [AttributeLogicalName(Schema.CategoryId)]
        public Guid? CategoryId
        {
            get => this.Category?.Id;
            set => this.Category = value.HasValue ? new EntityReference(XrmHajirProductCategory.Schema.LogicalName, value.Value) : null;
        }

        [AttributeLogicalName(Schema.ProductTypeCode)]
        public OptionSetValue ProductTypeCode { get => this.GetAttributeValue<OptionSetValue>(Schema.ProductTypeCode); set => this.SetAttributeValue(Schema.ProductTypeCode, value); }

        public Schema.ProductTypes? ProductType
        {
            get => this.ProductTypeCode == null ? (Schema.ProductTypes?)null : (Schema.ProductTypes)this.ProductTypeCode.Value;
            set => this.ProductTypeCode = value.HasValue ? new OptionSetValue((int)value.Value) : null;
        }
        [AttributeLogicalName(Schema.ProductSerie)]
        public OptionSetValue ProductSerieCode { get => this.GetAttributeValue<OptionSetValue>(Schema.ProductSerie); set => this.SetAttributeValue(Schema.ProductSerie, value); }

        [AttributeLogicalName(Schema.ProductSerie)]
        public Schema.ProductSeries? ProductSerie
        {
            get => this.ProductSerieCode == null ? (Schema.ProductSeries?)null : (Schema.ProductSeries)this.ProductSerieCode.Value;
            set => this.ProductSerieCode = value.HasValue ? new OptionSetValue((int)value.Value) : null;
        }

        [AttributeLogicalName(Schema.SpecSupportedBatteries)]
        public string SupportedBatteries
        {
            get => this.GetAttributeValue<string>(Schema.SpecSupportedBatteries);
            set => this.SetAttribiuteValue(Schema.SpecSupportedBatteries, value);
        }

        [AttributeLogicalName(Schema.CabinetNumberOfFloors)]
        public int? NumberOfFloors
        {
            get => this.GetAttributeValue<int?>(Schema.CabinetNumberOfFloors);
            set => this.SetAttribiuteValue(Schema.CabinetNumberOfFloors, value);
        }

        [AttributeLogicalName(Schema.SpecBatteryAmperage)]
        public decimal? BatteryCurrent
        {
            get => this.GetAttributeValue<decimal?>(Schema.SpecBatteryAmperage);
            set => this.SetAttribiuteValue(Schema.SpecBatteryAmperage, value);
        }
        [AttributeLogicalName(Schema.SynchedOn)]
        public DateTime? SynchedOn
        {
            get => this.GetAttributeValue<DateTime?>(Schema.SynchedOn);
            set => this.SetAttribiuteValue(Schema.SynchedOn, value);
        }
        [AttributeLogicalName(Schema.JsonProps)]
        public string JsonProps
        {
            get => this.GetAttributeValue<string>(Schema.JsonProps);
            set => this.SetAttributeValue(Schema.JsonProps, value);
        }
        




    }
}
