using GN.Library.Shared.Entities;
using GN.Library.Xrm;
using GN.Library.Xrm.Services.Bus;
using Hajir.Crm.Features.Common;
using Hajir.Crm.Infrastructure.Xrm.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Infrastructure.Xrm.Sales.Handlers
{
    public class XrmAccountValidationHandler : XrmValidationHandler<XrmHajirAccount>
    {
        private readonly ILogger<XrmAccountValidationHandler> logger;
        private readonly ICacheService cache;

        public XrmAccountValidationHandler(ILogger<XrmAccountValidationHandler> logger, ICacheService cache)
        {
            this.logger = logger;
            this.cache = cache;
        }
        public override async Task Handle(XrmMessage message)
        {
            await Task.CompletedTask;
            try
            {
                var account = message.Entity.ToEntity<XrmHajirAccount>();
                var city = account.GetAttributeValue<EntityReference>(XrmHajirAccount.Schema.CityId);
                if (city != null)
                {
                    var _city = this.cache.Cities.FirstOrDefault(x => x.Id == city.Id.ToString());
                    if (_city != null)
                    {
                        message.Change(XrmHajirAccount.Schema.Address1_City, _city.Name);
                        var province = this.cache.Provinces.FirstOrDefault(x => x.Id == _city.ProvinceId);
                        if (province!=null)
                        {
                            message.Change(XrmHajirAccount.Schema.address1_stateorprovince, province.Name);
                            var country = this.cache.Countries.FirstOrDefault(x => x.Id == province.CountryId);
                            if (country != null)
                            {
                                message.Change(XrmHajirAccount.Schema.address1_country, country.Name);
                            }
                        }
                    }
                }

                this.logger.LogInformation(
                    $"Account Validation Successfully Handled.");
            }
            catch (Exception err)
            {
                this.logger.LogError(
                    $"An error occured while trying to handle 'AccountValidation'. Err:{err.GetBaseException()}");
                throw;
            }
        }
    }
}
