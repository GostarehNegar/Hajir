using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Integration.SanadPardaz.Models
{
    public class DetailGetModel
    {
        public bool IsSuccess { get; set; }
        public string StatusCode { get; set; }
        public string Message { get; set; }
        public DetailGetDataModel Data { get; set; }
    }
    public class DetailGetDataModel
    {
        public int TotalRecords { get; set; }
        public int CurrentPageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public DetailGetItemModel[] Data { get; set; }

    }
    public class DetailGetItemModel
    {
        public int DetailAccCode { get; set; }
        public string DetailAccDesc { get; set; }
        public int DetailClass { get;set; }
        //"detailStatus": null,
        public int ACode { get; set; }
        //"creator": "se",
        //"hostName": "TOLID-PC6",
        //"okFlag": false,
        //"groupId": null,
        public DateTime insertDate { get; set; }
        //"map_DetailAccDesc": "",
        public bool ActiveFlag { get; set; }

        //"parent_DetailAccCode": null,
        //"treeNodeId": null,
        //"parent_2_DetailAccCode": null,
        //"idDetailAccIdent": 0,
        //"countUnit": null,
        //"archDocId": 584,
        //"activityTypeId3": null,
        public string FName { get; set; }
        public string LName { get; set; }
        public string NationalId { get; set; }
        //"typee": null,
        public string ZipCode { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNo { get; set; }
        public string TellNo { get; set; }
        public string FaxNo { get; set; }
        public string Addr { get; set; }
        //"birthDate": null,
        //"city": null,
        //"provinceID": null,
        //"regNo": null,
        //"economicCode": null,
        //"sex": null,
        //"abC_HasCostDriver": false,
        //"abC_ClassCode": 1,
        //"abC_PricingClass": 1,
        //"buyerType": null,
        //"cityCode": null,
        //"preCodeCity": null,
        //"agreementNo": null,
        //"geo_X": null,
        //"geo_Y": null,
        //"latinName": null,
        //"nationId": null,
        //"userCode": null,
        //"sysCode": null,
        public string ActionDate { get; set; }
    }

    public class DetailGetByCodeModel
    {
        public bool IsSuccess { get; set; }
        public string StatusCode { get; set; }
        public string Message { get; set; }
        public DetailGetItemModel Data { get; set; }
    }
}
