using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.StdSolution
{
    /// <summary>
    /// 
    /// Ref: https://docs.microsoft.com/en-us/dynamics365/customer-engagement/web-api/email?view=dynamics-ce-odata-9
    /// Relay: http://quantusdynamics.blogspot.com/2014/01/dynamics-crm-office365-email-relay.html
    /// </summary>
    [EntityLogicalNameAttribute(Schema.LogicalName)]
    public class XrmEmail : XrmActivity<XrmEmail, XrmEmail.Schema.StateCodes, XrmEmail.Schema.StatusCodes>
    {
        public new class Schema : XrmActivity<XrmEmail, DefaultStateCodes, DefaultStatusCodes>.Schema
        {
            public const string LogicalName = "email";

            /// <summary>
            ///  
            /// see also: https://docs.microsoft.com/en-us/previous-versions/dynamicscrm-2016/administering-dynamics-365/dn531157(v=crm.8)
            /// </summary>
            public enum StateCodes
            {
                Open = 0,
                Completed = 1,
                Canceled = 2

            }
            /// <summary>
            /// Open: Draft, Failed
            /// Completed : Completed,Sent,Recieved,PendingSend,Sending
            /// Canceled: Canceled
            /// </summary>
            public enum StatusCodes
            {
                /// <summary>
                /// Draft=1 (State=Open)
                /// </summary>
                Draft = 1,
                /// <summary>
                /// Failed=8 (Valid only for State=Open)
                /// </summary>
                Failed = 8,
                // Completed
                /// <summary>
                /// Completed=2 (State=Completed)
                /// </summary>
                Completed = 2,

                /// <summary>
                /// Send=3 (State=Completed)
                /// </summary>
                Sent = 3,
                /// <summary>
                /// Recieved=4 (State=Completed)
                /// </summary>
                Received = 4,
                /// <summary>
                /// PendingSend =6 (State=Completed)
                /// </summary>

                PendingSend = 6,

                /// <summary>
                /// Sending=7 (State=Completed)
                /// </summary>
                Sending = 7,

                /// <summary>
                /// Canceled =5 (State=Canceled)
                /// </summary>
                Canceled = 5
            }
        }
        public XrmEmail() : base(Schema.LogicalName)
        {
        }
    }
}
