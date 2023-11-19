using GN.Library.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Abstractions.Entities
{
    public class HajirProductEntity : DynamicEntity
    {
        public new class Schema : DynamicEntity.Schema
        {
            public const string LogicalName = "product";
            public const string ProductId = LogicalName + "id";
            public const string Name = "name";
            public const string ProductNumber = "productnumber";
        }
    }
}
