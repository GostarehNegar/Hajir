using Hajir.Crm;
using System.IO;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public class HajirIntegrationOptions
    {
        
        public string LegacyConnectionString { get; set; } = HajirCrmConstants.DefaultLegacyCrmConnectionString;
        public bool LegacyAccountImportEnabled { get; set; } = false;
        public bool LegacyContactImportEnabled { get; set; } = false;
        public bool LegacyImportEnabled { get; set; } = false;

        public HajirIntegrationOptions Validate()
        {
            this.LegacyConnectionString = string.IsNullOrEmpty(this.LegacyConnectionString)? HajirCrmConstants.DefaultLegacyCrmConnectionString:this.LegacyConnectionString;
            return this;
        }
        public string GetQueueStorageConnectionString()
        {
            var result =
                Path.Combine(
                Path.GetDirectoryName( this.GetType().Assembly.Location),
                //Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "Data\\Integration_Queue.dat");

            return $"Filename={result}";
        }
    }
}