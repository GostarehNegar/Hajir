﻿using GN.Library.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Features.Integration
{
    public class IntegrationContact:DynamicEntity
    {
        public string Id { get; set; }
        
        public string Salutation { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Type { get; set; }
        public int? Class { get; set; }

        public string FullName => string.IsNullOrWhiteSpace(LastName) ? Name : $"{FirstName} {LastName}";
        public string MobilePhone { get; set; }
        public string City { get; set; }

        public string ExternalId { get; set; }
        public string JobTitle { get; set; }
        public string Province { get; set; }
        public bool Hadaya { get; set; }
        public string Role { get; set; }
        public string AccontId { get; set; }
        public string Address { get; set; }
        public string BusinessPhone { get; set; }
        public string Email { get; set; }

        public override string ToString()
        {
            return $"{Id} {FullName}";
        }
        public string GetIntegrationDesription()
        {
            return $"";
        }

    }
}
