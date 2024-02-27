﻿using GN.Library.Xrm;
using Hajir.Crm.Entities;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Infrastructure.Xrm.Data
{
    [EntityLogicalName(Schema.LogicalName)]
    public class XrmHajirCity : XrmEntity<XrmHajirCity, DefaultStateCodes, DefaultStatusCodes>
    {
        public new class Schema : XrmEntity.Schema
        {
            public const string SolutionPerfix = HajirCrmConstants.RahsamSolutionPerfix;
            public const string LogicalName = SolutionPerfix + "city";
            public const string CityId = LogicalName + "id";
            public const string Name = SolutionPerfix + "name";
        }

        public XrmHajirCity() : base(Schema.LogicalName) { }

        [AttributeLogicalName(Schema.CityId)]
        public System.Nullable<System.Guid> CityId
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.CityId);
            }
            set
            {
                this.SetAttributeValue(Schema.CityId, value);
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



        [AttributeLogicalNameAttribute(Schema.CityId)]
        public override System.Guid Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                this.CityId = value;
            }
        }

        [AttributeLogicalNameAttribute(Schema.Name)]
        public string Name { get { return this.GetAttributeValue<string>(Schema.Name); } set { this.SetAttribiuteValue(Schema.Name, value); } }

    }
}
