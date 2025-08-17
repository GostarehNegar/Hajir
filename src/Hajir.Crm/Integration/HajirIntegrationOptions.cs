using Hajir.Crm;
using System.IO;
using System;

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
    public class HajirIntegrationOptions
    {

        public string LegacyConnectionString { get; set; } = HajirCrmConstants.DefaultLegacyCrmConnectionString;
        public bool LegacyAccountImportEnabled { get; set; } = false;
        public bool LegacyContactImportEnabled { get; set; } = false;
        public bool LegacyImportEnabled { get; set; } = false;
        public ProductIntegrationOptions ProductIntegration { get; set; } = new ProductIntegrationOptions();
        public SanadAccountIntegrationOptions SanadIntegration { get; set; }

        public HajirIntegrationOptions Validate()
        {
            LegacyConnectionString = string.IsNullOrEmpty(LegacyConnectionString) ? HajirCrmConstants.DefaultLegacyCrmConnectionString : LegacyConnectionString;
            ProductIntegration = this.ProductIntegration ?? new ProductIntegrationOptions();
            this.SanadIntegration = (this.SanadIntegration ?? new SanadAccountIntegrationOptions()).Validate();
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