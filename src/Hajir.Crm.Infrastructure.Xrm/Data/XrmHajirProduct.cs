using GN.Library.Xrm.StdSolution;
using Hajir.Crm.Entities;
using Hajir.Crm.Features.Products;
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

        [AttributeLogicalName(Schema.ProductType)]
        public OptionSetValue ProductTypeCode { get => this.GetAttributeValue<OptionSetValue>(Schema.ProductType); set => this.SetAttributeValue(Schema.ProductType, value); }

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

        [AttributeLogicalName(Schema.SupportedBatteries)]
        public string SupportedBatteries
        {
            get => this.GetAttributeValue<string>(Schema.SupportedBatteries);
            set => this.SetAttribiuteValue(Schema.SupportedBatteries, value);
        }

        [AttributeLogicalName(Schema.NumberOfFloors)]
        public string NumberOfFloors
        {
            get => this.GetAttributeValue<string>(Schema.NumberOfFloors);
            set => this.SetAttribiuteValue(Schema.NumberOfFloors, value);
        }

        [AttributeLogicalName(Schema.BatteryCurrent)]
        public string BatteryCurrent
        {
            get => this.GetAttributeValue<string>(Schema.BatteryCurrent);
            set => this.SetAttribiuteValue(Schema.BatteryCurrent, value);
        }


        public int GetNumberIfFloors() => int.TryParse(NumberOfFloors, out var _r) ? _r : 0;



    }
}
