using System;

namespace Hajir.Crm.Integration.SanadPardaz.Models
{
    /// <summary>
    /// "DetailAccCode": 19,
        //"DetailAccDesc": "مهدي پناهي فرد پيله رود",
        //"DetailClass": 90,
        //"Parent_DetailAccCode": null,
        //"Parent_2_DetailAccCode": null,
        //"NationalId": "0069752109",
        //"Typee": 0,
        //"ZipCode": "",
        //"EmailAddress": "",
        //"MobileNo": "09101862104",
        //"TellNo": "77798947",
        //"FaxNo": null,
        //"Addr": "تهران-تهرانپارس-خيابان 190 غربي-کوچه مرادي-پلاک 111-واحد 3",
        //"City": null,
        //"ProvinceID": null,
        //"RegNo": "8109",
        //"EconomicCode": null,
        //"BuyerType": null,
        //"CityCode": null,
        //"PreCodeCity": null,
        //"NationId": 1,
        //"Parent_3_DetailAccCode": null,
        //"Parent_4_DetailAccCode": null,
        //"ActionDate": null
    /// </summary>
    public class DetailModel
    {
        public int DetailAccCode { get; set; }
        public string DetailAccDesc { get; set; }
        public int DetailClass { get; set; }
        //"detailStatus": null,
        public int ACode { get; set; }
        //"creator": "se",
        //"hostName": "TOLID-PC6",
        //"okFlag": false,
        //"groupId": null,
        public DateTime insertDate { get; set; }
        //"map_DetailAccDesc": "",
        public bool ActiveFlag { get; set; }

        
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
        public string RegNo { get; set; }
        public int? CityCode { get; set; }
        public string City { get; set; }
        public int Typee { get; set; }
        public string EconomicCode { get; set; }

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
}
