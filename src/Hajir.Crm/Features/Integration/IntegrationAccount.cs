using GN.Library.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Features.Integration
{
    public class IntegrationAccount:DynamicEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }

        public string MainPhone { get; set; }
        public string Fax { get; set; }
        public string gn_shenasemeli { get; set; }
        public string gn_sabt { get; set; }
        public string gn_eco_code { get; set; }
        public string address1_name { get; set; }
        public string gn_hesab_no { get; set; }
        public string City { get; set; }
        public string address1_postalcode { get; set; }
        public string Daraje_Ahamiat { get; set; }
        public string Nahve_Ahnaei { get; set; }
        public string Category { get; set; }
        public string Industry { get; set; }
        public string RelationShipType { get; set; }

        public override string ToString()
        {
            return $"{Id} {Name}";
        }

        public string GetImportantIntegrationValuesAsText()
        {
            var result = new StringBuilder();
            void add(string ley, string value)
            {
                result.AppendLine($"{ley}:{value}");
            }
            add("Name", Name);
            add("gn_eco_code", gn_eco_code);
            add("gn_hesab_no", gn_hesab_no);
            add("gn_shenasemeli", gn_shenasemeli);
            add("gn_sabt", gn_sabt);

            add("Category", Category);
            add("درجه اهمیت", Daraje_Ahamiat);
            add("نحوه آشنایی", Nahve_Ahnaei);
            add("صنعت", Industry);
            add("RelationShipType", RelationShipType);
            return result.ToString();
            
                
        }
    }
}
