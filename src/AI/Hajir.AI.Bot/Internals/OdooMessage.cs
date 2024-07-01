using GN.Library.Odoo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.AI.Bot.Internals
{
    [OdooModel(Schema.LogicalName)]
    public class OdooMessage : OdooEntity
    {
        public new class Schema:OdooEntity.Schema
        {
            public const string LogicalName = "mail.message";
            public const string Subject = "subject";
            public const string AUthor_Id = "author_id";
            public const string Body = "body";
            public const string MessageType = "message_type";
            public const string PartnerIds = "partner_ids";
        }
        public OdooMessage():base(Schema.LogicalName) { }
        
        [OdooColumn(Schema.Subject)]
        public string Subject { get => this.GetAttributeValue<string>(Schema.Subject); set => this.SetAttributeValue(Schema.Subject, value); }

        [OdooColumn(Schema.AUthor_Id)]
        public int? AuthorId { get => this.GetAttributeValue<int?>(Schema.AUthor_Id); set => this.SetAttributeValue(Schema.AUthor_Id, value); }

        [OdooColumn(Schema.PartnerIds)]
        public int[] PartnerIds { get => this.GetAttributeValue<int[]>(Schema.PartnerIds); set => this.SetAttributeValue(Schema.PartnerIds, value); }

        [OdooColumn(Schema.Body)]
        public string Body { get => this.GetAttributeValue<string>(Schema.Body); set => this.SetAttributeValue(Schema.Body, value); }

        [OdooColumn(Schema.MessageType)]
        public string MessageType { get => this.GetAttributeValue<string>(Schema.MessageType); set => this.SetAttributeValue(Schema.MessageType, value); }

    }


    [OdooModel(Schema.LogicalName)]
    public class OdooMail : OdooEntity
    {
        public new class Schema : OdooEntity.Schema
        {
            public const string LogicalName = "mail.mail";
            public const string Subject = "subject";
            public const string AUthor_Id = "author_ids";
            public const string Body = "body_html";
            public const string MessageType = "message_type";
            public const string PartnerIds = "partner_ids";
            public const string Email_To = "email_to";
            public const string Receipients = "recipient_ids";
        }
        public OdooMail() : base(Schema.LogicalName) { }

        [OdooColumn(Schema.Subject)]
        public string Subject { get => this.GetAttributeValue<string>(Schema.Subject); set => this.SetAttributeValue(Schema.Subject, value); }

        [OdooColumn(Schema.AUthor_Id)]
        public int? AuthorId { get => this.GetAttributeValue<int?>(Schema.AUthor_Id); set => this.SetAttributeValue(Schema.AUthor_Id, value); }

        [OdooColumn(Schema.Receipients)]
        public int[] Receipients { get => this.GetAttributeValue<int[]>(Schema.Receipients); set => this.SetAttributeValue(Schema.Receipients, value); }

        [OdooColumn(Schema.Body)]
        public string Body { get => this.GetAttributeValue<string>(Schema.Body); set => this.SetAttributeValue(Schema.Body, value); }

        [OdooColumn(Schema.MessageType)]
        public string MessageType { get => this.GetAttributeValue<string>(Schema.MessageType); set => this.SetAttributeValue(Schema.MessageType, value); }

    }
}
