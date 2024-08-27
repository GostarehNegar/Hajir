using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Integration.SanadPardaz.Models
{
    public class GoodGetModel
    {
        public bool IsSuccess { get; set; }
        public GoodGetDataModel Data { get; set; }
    }
    public class GoodGetDataModel
    {
        public int TotalRecords { get; set; }
        public int CurrentPageNumber { get; set; }
        public int PageSize { get; set; }
        public GoodGetItemModel[] Data { get; set; }

    }
    public class GoodGetItemModel
    {
        public string GoodCode { get; set; }
        public string CatCode { get; set; }
        public string GoodName { get; set; }
        public string CountUnit { get; set; }
        public decimal SaleFee { get; set; }
        public DateTime ActionDate { get; set; }
    }

}
