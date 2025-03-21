﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Sales
{
    public class UnitOfMeasurements
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string UnitId { get; set; }
    }
    public class UnitOfMeasurmentGroup
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public UnitOfMeasurements[] Unites { get; set; }
    }
}
