using GN.Library.Xrm;
using GN.Library.Xrm.Services.Bus;
using GN.Library.Xrm.StdSolution;
using Hajir.Crm.Common;
using Hajir.Crm.Infrastructure.Xrm.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hajir.Crm.Infrastructure.Xrm.Sales.Handlers
{
    public class XrmContactValidationHandler : XrmValidationHandler<XrmContact>
    {
        private readonly ILogger<XrmContactValidationHandler> logger;
        private readonly ICacheService cache;

        public XrmContactValidationHandler(ILogger<XrmContactValidationHandler> logger, ICacheService cache)
        {
            this.logger = logger;
            this.cache = cache;
        }
        public override Task Handle(XrmMessage message)
        {
            var contact = message.Entity.ToEntity<XrmHajirContact>();
            try
            {
                if (contact.City != null)
                {
                    var city = this.cache.Cities.FirstOrDefault(x => x.Id == contact.City.Id.ToString());
                    if (city != null)
                    {
                        message.Change(XrmHajirContact.Schema.Address1_City, city?.Name);
                        var province = this.cache.Provinces.FirstOrDefault(x => x.Id == city.ProvinceId);
                        if (province != null)
                        {
                            message.Change(XrmHajirContact.Schema.address1_stateorprovince, province.Name);
                            if (province.GetId<Guid?>().HasValue)
                            {
                                message.Change(XrmHajirContact.Schema.ProvinceId, new EntityReference(XrmHajirProvince.Schema.LogicalName, province.GetId<Guid?>().Value));
                            }
                            var country = this.cache.Countries.FirstOrDefault(x => x.Id == province.CountryId);
                            if (country != null)
                            {
                                message.Change(XrmHajirContact.Schema.Address1_Country, country?.Name);
                                if (Guid.TryParse(country.Id, out var countryId))
                                {
                                    message.Change(XrmHajirContact.Schema.CountryId, new EntityReference(XrmHajirCountry.Schema.LogicalName, countryId));
                                }
                            }
                        }

                    }
                }
                this.logger.LogInformation(
                    $"Contact Successfully Validated.");
            }
            catch (Exception err)
            {
                this.logger.LogError(
                    $"Am error occured while trying to handle ContactValidationHandler. Err:{err.GetBaseException()}");
                throw;
            }

            return Task.CompletedTask;
            //throw new System.NotImplementedException();
        }
    }
}
