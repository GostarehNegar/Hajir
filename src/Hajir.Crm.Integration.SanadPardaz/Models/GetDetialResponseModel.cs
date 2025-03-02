using Microsoft.Crm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Integration.SanadPardaz.Models
{
    public class GetDetailRequestModel
    {
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public string orderBy { get; set; }
        public string orderDirection { get; set; }
    }

    public class GetDetialResponseModel : SanadApiResponseModel<DetailModel>
    {



    }
   

}
