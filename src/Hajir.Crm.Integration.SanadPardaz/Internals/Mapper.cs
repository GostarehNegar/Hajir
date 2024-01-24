using Hajir.Crm.Features.Integration;
using Hajir.Crm.Integration.SanadPardaz.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Integration.SanadPardaz.Internals
{
    internal static class Mapper
    {
        public static IntegrationAccount ToAccount(this DetailCode detail)
        {
            return new IntegrationAccount
            {
                Id = detail.DetailAccCode.ToString(),
                Name = detail.DetailAccDesc,
                Type = detail.Typee ?? -1,


            };


        }
        public static IntegrationContact ToContact(this DetailCode detail)
        {
            return new IntegrationContact
            {
                Id = detail.DetailAccCode.ToString(),
                Name = detail.DetailAccDesc,
                Type = detail.Typee ?? -1,
                FirstName = detail.FName,
                LastName = detail.LName,
                Class = detail.DetailClass


            };


        }
        public static IntegrationProduct ToProduct(this Good good)
        {
            return new IntegrationProduct
            {
                Id = good.GoodCode,
                Name = good.GoodName
            };
        }
    }
}
