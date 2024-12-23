using Hajir.Crm;
using System.IO;
using System;

namespace Hajir.Crm.Integration
{
    public class HajirIntegrationOptions
    {

        public string LegacyConnectionString { get; set; } = HajirCrmConstants.DefaultLegacyCrmConnectionString;
        public bool LegacyAccountImportEnabled { get; set; } = false;
        public bool LegacyContactImportEnabled { get; set; } = false;
        public bool LegacyImportEnabled { get; set; } = false;

        public HajirIntegrationOptions Validate()
        {
            LegacyConnectionString = string.IsNullOrEmpty(LegacyConnectionString) ? HajirCrmConstants.DefaultLegacyCrmConnectionString : LegacyConnectionString;
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