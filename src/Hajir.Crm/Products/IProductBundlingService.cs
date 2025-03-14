﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Products
{
    public interface IProductBundlingService
    {
        string ValidateBundle(ProductBundle bundle);
        IEnumerable<Product> GetAllUpses();
        IEnumerable<Product> GetAllBatteries();
        IEnumerable<Product> GetAllCabinets();
        IEnumerable<Product> Products { get; }
        CabinetSet[] Design(Product UPS, int power, int numberOfBatteries, CabinetVendors vendor, IEnumerable<Product> cabinets = null);
        CabinetSet[] Design(Product UPS, int power, int numberOfBatteries, IEnumerable<Product> cabinets = null);


    }
}
