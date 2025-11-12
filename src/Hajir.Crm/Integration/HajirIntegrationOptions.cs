using Hajir.Crm;
using System.IO;
using System;
using System.Linq;
using Hajir.Crm.Integration.PriceList;

namespace Hajir.Crm.Integration
{
    public class ProductIntegrationOptions
    {
        public bool Disabled { get; set; } = false;
        public bool CSVDatasheetIntegration { get; set; } = false;
    }
    public class SanadAccountIntegrationOptions
    {
        public bool Disabled { get; set;} = false;
        public int CacheDurationSeconds { get; set; } = 60;
        public SanadAccountIntegrationOptions Validate()
        {
            this.CacheDurationSeconds = this.CacheDurationSeconds > 10 ? this.CacheDurationSeconds : 60;
            return this;
        }
    }
    public class ProductDbIntegrationOptions
    {
        public bool Disabled { get; set; } = true;
        public short[] Categories { get; set; } =Array.Empty<short>();
        public int WaitSeconds { get; set;} = 60;
        public ProductDbIntegrationOptions Validate()
        {
            Categories = Categories?? Array.Empty<short>();
            if (Categories.Length == 0)
            {
                Categories = HajirCrmConstants.Schema.Product.GetProductCategories();
            }
            this.WaitSeconds = this.WaitSeconds < 60 ? 60 : 60;
            return this;
        }
    }
    public class HajirIntegrationOptions
    {

        public string LegacyConnectionString { get; set; } = HajirCrmConstants.DefaultLegacyCrmConnectionString;
        public bool LegacyAccountImportEnabled { get; set; } = false;
        public bool LegacyContactImportEnabled { get; set; } = false;
        public bool LegacyImportEnabled { get; set; } = false;
        public ProductIntegrationOptions ProductIntegration { get; set; } = new ProductIntegrationOptions();
        public SanadAccountIntegrationOptions SanadIntegration { get; set; }
        public ProductDbIntegrationOptions ProductSanadDbIntegrationOptions { get; set; } = new ProductDbIntegrationOptions { };
      
        public PriceListIntegrationOptions PriceLists { get; set; }
        public HajirIntegrationOptions Validate()
        {
            LegacyConnectionString = string.IsNullOrEmpty(LegacyConnectionString) ? HajirCrmConstants.DefaultLegacyCrmConnectionString : LegacyConnectionString;
            ProductIntegration = this.ProductIntegration ?? new ProductIntegrationOptions();
            this.SanadIntegration = (this.SanadIntegration ?? new SanadAccountIntegrationOptions()).Validate();
            this.ProductSanadDbIntegrationOptions = (this.ProductSanadDbIntegrationOptions ?? new ProductDbIntegrationOptions()).Validate();
            this.PriceLists = (this.PriceLists ?? new PriceListIntegrationOptions()).Vaidate();
            return this;
        }
        public string GetQueueStorageConnectionString()
        {
            var result =
                Path.Combine(
                Path.GetDirectoryName(GetType().Assembly.Location),
                //Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "Data\\Integration_Queue.dat");

            return $"Filename={result}";
        }
    }
}