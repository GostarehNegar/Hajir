using Hajir.Crm;

namespace Microsoft.Extensions.DependencyInjection
{
    public class HajirIntegrationOptions
    {
        public string LegacyConnectionString { get; set; } = HajirCrmConstants.DefaultLegacyCrmConnectionString;

        public HajirIntegrationOptions Validate()
        {
            this.LegacyConnectionString = string.IsNullOrEmpty(this.LegacyConnectionString)? HajirCrmConstants.DefaultLegacyCrmConnectionString:this.LegacyConnectionString;
            return this;
        }
    }
}