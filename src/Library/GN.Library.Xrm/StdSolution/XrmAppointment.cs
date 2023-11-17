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
	public class XrmAppointment : XrmActivity<XrmAppointment, XrmAppointment.Schema.StateCodes, XrmAppointment.Schema.StatusCodes>
	{
		public new class Schema : XrmActivity<XrmEmail, DefaultStateCodes, DefaultStatusCodes>.Schema
		{
			public const string LogicalName = "appointment";

			/// <summary>
			///  
			/// see also: https://docs.microsoft.com/en-us/previous-versions/dynamicscrm-2016/administering-dynamics-365/dn531157(v=crm.8)
			/// </summary>
			public enum StateCodes
			{
				Open = 0,
				Completed = 1,
				Canceled = 2,
				Scheduled = 3

			}
			/// <summary>
			/// Open: Draft, Failed
			/// Completed : Completed,Sent,Recieved,PendingSend,Sending
			/// Canceled: Canceled
			/// </summary>
			public enum StatusCodes
			{
				Free = 1,
				Tentative = 2,
				Completed = 3,
				Canceled = 4,
				OutOfOffice = 6,
				Busy = 5
			}
		}
		public XrmAppointment() : base(Schema.LogicalName)
		{
		}
	}
}
