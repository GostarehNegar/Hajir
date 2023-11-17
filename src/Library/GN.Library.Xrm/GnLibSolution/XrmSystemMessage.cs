using GN.Library.Contracts_Deprecated;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.GnLibSolution
{
	[EntityLogicalName(Schema.LogicalName)]
	public class XrmSystemMessage : XrmEntity<XrmSystemMessage, DefaultStateCodes, XrmSystemMessage.Schema.StatusCodes>
	{
		public new class Schema
		{
			public const string SolutionPerfix = Constants.SolutionPerfix;
			public const int SoltionOptionBase = Constants.SolutionOptionBase;
			public const string LogicalName = SolutionPerfix + "systemmessage";
			public const string SystemMessageId = LogicalName + "id";
			public const string Topic = SolutionPerfix + "topic";
			public const string Int1 = SolutionPerfix + "int1";
			public const string Int2 = SolutionPerfix + "int2";
			public const string IntResult = SolutionPerfix + "intresult";
			public const string Response = SolutionPerfix + "response";
			public const string Str1 = SolutionPerfix + "str1";
			public const string Str2 = SolutionPerfix + "str2";
			public const string StrResult = SolutionPerfix + "strresult";
			public const string Url1 = SolutionPerfix + "url1";
			public const string Url2 = SolutionPerfix + "url2";
			public const string Decimal1 = SolutionPerfix + "decimal1";
			public const string Decimal2 = SolutionPerfix + "decimal2";
			public const string DecimalResult = SolutionPerfix + "decimalresult";
			public const string Float1 = SolutionPerfix + "float1";
			public const string Float2 = SolutionPerfix + "float2";
			public const string FloatResult = SolutionPerfix + "floatresult";
			public const string Date1 = SolutionPerfix + "date1";
			public const string Date2 = SolutionPerfix + "date2";
			public const string DateResult = SolutionPerfix + "dateresult";
			public const string DateTime1 = SolutionPerfix + "datetime1";
			public const string DateTime2 = SolutionPerfix + "datetime2";
			public const string DateTimeResult = SolutionPerfix + "datetimeresult";
			public const string MessageType = SolutionPerfix + "messagetype";
			public const string Message = SolutionPerfix + "message";
			public const string Error = SolutionPerfix + "error";
			public const string Log = SolutionPerfix + "log";
			public const string Mode = SolutionPerfix + "mode";
			public const string Timeout = SolutionPerfix + "timeout";

			public enum StatusCodes
			{
				Active_Ready = 1,
				Active_Draft = SoltionOptionBase,
				Active_InProgress = SoltionOptionBase + 1,
				Active_Processed = SoltionOptionBase + 2,
				Active_Processed_With_Error = SoltionOptionBase + 3,
				InActive_Completed = 2,
				InActive_Failed = SoltionOptionBase + 4,
				InActive_Canceled = SoltionOptionBase + 5,
				InActive_Timeout = SoltionOptionBase + 6,
			}
			public enum MessageModes
			{
				Event = SoltionOptionBase,
				Command = SoltionOptionBase + 1,
				AsyncCommand = SoltionOptionBase + 2
			}
		}
		public XrmSystemMessage() : base(Schema.LogicalName)
		{

		}
		[AttributeLogicalName(Schema.SystemMessageId)]
		public System.Nullable<System.Guid> SystemMessageId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.SystemMessageId);
			}
			set
			{
				this.SetAttributeValue(Schema.SystemMessageId, value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					this.Attributes.Remove(Schema.SystemMessageId);
					base.Id = System.Guid.Empty;
				}
			}
		}

		[AttributeLogicalNameAttribute(Schema.SystemMessageId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				if (value != Guid.Empty)
					this.SystemMessageId = value;
			}
		}


		[AttributeLogicalNameAttribute(Schema.Topic)]
		public string Topic
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.Topic);
			}
			set
			{
				this.SetAttributeValue(Schema.Topic, value);
			}
		}

		[AttributeLogicalName(Schema.Mode)]
		public int? ModeCode
		{
			get
			{
				var _ = this.GetAttributeValue<OptionSetValue>(Schema.Mode);
				return _ == null
					? (int?)null
					: _.Value;
			}
			set
			{
				if (value.HasValue)
					this.SetAttribiuteValue(Schema.Mode, new OptionSetValue(value.Value));
				else
					this.SetAttribiuteValue(Schema.Mode, null);
			}
		}

		public Schema.MessageModes? Mode
		{
			get
			{
				return this.ModeCode.HasValue
					? (Schema.MessageModes?)Enum.ToObject(typeof(Schema.MessageModes), this.ModeCode)
					: (Schema.MessageModes?)null;

			}
			set
			{
				this.ModeCode = value == null
					? (int?)null
					: (int?)Convert.ChangeType(value, Enum.GetUnderlyingType(typeof(Schema.MessageModes)));
			}

		}

		[AttributeLogicalName(Schema.Timeout)]
		public int? Timeout
		{
			get
			{
				var value = this.GetAttributeValue<object>(Schema.Timeout);
				return value == null
					? (int?)null
					: Convert.ToInt32(value);
			}
			set { this.SetAttribiuteValue(Schema.Timeout, value); }
		}


		[AttributeLogicalName(Schema.Str1)]
		public string Str1
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.Str1);
			}
			set
			{
				this.SetAttributeValue(Schema.Str1, value);
			}
		}

		[AttributeLogicalName(Schema.Response)]
		public string Response
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.Response);
			}
			set
			{
				this.SetAttributeValue(Schema.Response, value);
			}
		}

		[AttributeLogicalName(Schema.Str2)]
		public string Str2
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.Str2);
			}
			set
			{
				this.SetAttributeValue(Schema.Str2, value);
			}
		}

		[AttributeLogicalName(Schema.StrResult)]
		public string StrResult
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.StrResult);
			}
			set
			{
				this.SetAttributeValue(Schema.StrResult, value);
			}
		}

		[AttributeLogicalName(Schema.Error)]
		public string Error
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.Error);
			}
			set
			{
				this.SetAttributeValue(Schema.Error, value);
			}
		}

		[AttributeLogicalName(Schema.Log)]
		public string Log
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.Log);
			}
			set
			{
				this.SetAttributeValue(Schema.Log, value);
			}
		}


		[AttributeLogicalName(Schema.Url1)]
		public string Url1
		{
			get { return this.GetAttributeValue<string>(Schema.Url1); }
			set { this.SetAttributeValue(Schema.Url1, value); }
		}

		[AttributeLogicalName(Schema.Url2)]
		public string Url2
		{
			get { return this.GetAttributeValue<string>(Schema.Url2); }
			set { this.SetAttributeValue(Schema.Url2, value); }
		}


		[AttributeLogicalName(Schema.MessageType)]
		public string MessageType
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.MessageType);
			}
			set
			{
				this.SetAttributeValue(Schema.MessageType, value);
			}
		}

		[AttributeLogicalName(Schema.Message)]
		public string Message
		{
			get { return this.GetAttributeValue<string>(Schema.Message); }
			set { this.SetAttributeValue(Schema.Message, value); }
		}

		[AttributeLogicalName(Schema.Int1)]
		public int? Int1
		{
			get
			{
				var value = this.GetAttributeValue<object>(Schema.Int1);
				return value == null
					? (int?)null
					: Convert.ToInt32(value);
			}
			set { this.SetAttributeValue(Schema.Int1, value); }
		}

		[AttributeLogicalName(Schema.Int2)]
		public int? Int2
		{
			get
			{
				var value = this.GetAttributeValue<object>(Schema.Int2);
				return value == null
					? (int?)null
					: Convert.ToInt32(value);
			}
			set
			{
				if (value.HasValue)
					this.SetAttributeValue(Schema.Int2, Convert.ToInt32(value));
				else
					this.SetAttributeValue(Schema.Int2, null);
			}
		}

		[AttributeLogicalName(Schema.IntResult)]
		public int? IntResult
		{
			get
			{
				var value = this.GetAttributeValue<object>(Schema.IntResult);
				return value == null
					? (int?)null
					: Convert.ToInt32(value);
			}
			set
			{
				if (value.HasValue)
					this.SetAttributeValue(Schema.IntResult, Convert.ToInt32(value));
				else
					this.SetAttributeValue(Schema.IntResult, null);
			}
		}

		[AttributeLogicalName(Schema.Decimal1)]
		public decimal? Decimal1
		{
			get
			{
				var o = this.GetAttributeValue<object>(Schema.Decimal1);
				return o == null
					? (decimal?)null
					: Convert.ToDecimal(o);
				//return this.GetAttributeValue<decimal?>(Schema.Decimal1);
			}
			set { this.SetAttributeValue(Schema.Decimal1, value); }
		}

		[AttributeLogicalName(Schema.Decimal2)]
		public decimal? Decimal2
		{
			get
			{
				var o = this.GetAttributeValue<object>(Schema.Decimal2);
				return o == null
					? (decimal?)null
					: Convert.ToDecimal(o);
			}
			set { this.SetAttributeValue(Schema.Decimal2, value); }
		}

		[AttributeLogicalName(Schema.DecimalResult)]
		public decimal? DecimalResult
		{
			get
			{
				var o = this.GetAttributeValue<object>(Schema.DecimalResult);
				return o == null
					? (decimal?)null
					: Convert.ToDecimal(o);
			}
			set { this.SetAttributeValue(Schema.DecimalResult, value); }
		}

		[AttributeLogicalName(Schema.Date1)]
		public DateTime? Date1
		{
			get { return this.GetAttributeValue<DateTime?>(Schema.Date1); }
			set { this.SetAttributeValue(Schema.Date1, value); }
		}

		[AttributeLogicalName(Schema.Date2)]
		public DateTime? Date2
		{
			get { return this.GetAttributeValue<DateTime?>(Schema.Date2); }
			set { this.SetAttributeValue(Schema.Date2, value); }
		}

		[AttributeLogicalName(Schema.DateResult)]
		public DateTime? DateResult
		{
			get { return this.GetAttributeValue<DateTime?>(Schema.DateResult); }
			set { this.SetAttributeValue(Schema.DateResult, value); }
		}


		[AttributeLogicalName(Schema.DateTime1)]
		public DateTime? DateTime1
		{
			get { return this.GetAttributeValue<DateTime?>(Schema.DateTime1); }
			set { this.SetAttributeValue(Schema.DateTime1, value); }
		}

		[AttributeLogicalName(Schema.DateTime2)]
		public DateTime? DateTime2
		{
			get { return this.GetAttributeValue<DateTime?>(Schema.DateTime2); }
			set { this.SetAttributeValue(Schema.DateTime2, value); }
		}

		[AttributeLogicalName(Schema.DateTimeResult)]
		public DateTime? DateTimeResult
		{
			get { return this.GetAttributeValue<DateTime?>(Schema.DateTimeResult); }
			set { this.SetAttributeValue(Schema.DateTimeResult, value); }
		}


		[AttributeLogicalName(Schema.Float1)]
		public double? Float1
		{
			get
			{
				var obj = this.GetAttributeValue<object>(Schema.Float1);

				return obj == null
					? (double?)null
					: (double?)Convert.ToDouble(obj);
			}
			set { this.SetAttributeValue(Schema.Float1, value); }
		}

		[AttributeLogicalName(Schema.Float2)]
		public double? Float2
		{
			get
			{
				var obj = this.GetAttributeValue<object>(Schema.Float2);

				return obj == null
					? (double?)null
					: (double?)Convert.ToDouble(obj);
			}
			set { this.SetAttributeValue(Schema.Float2, value); }
		}

		[AttributeLogicalName(Schema.FloatResult)]
		public double? FloatResult
		{
			get
			{
				var obj = this.GetAttributeValue<object>(Schema.FloatResult);

				return obj == null
					? (double?)null
					: (double?)Convert.ToDouble(obj);
			}
			set
			{
				this.SetAttributeValue(Schema.FloatResult, value);
			}
		}

		public override string ToString()
		{
			return string.Format("SystemMessage topic:'{0}', Id:'{1}', Status:'{2}'", this.Topic, this.Id, this.Status);
		}

		public XrmSystemMessageModel ToXrmMessageModel()
		{
			return new XrmSystemMessageModel
			{
				SystemEventId = this.SystemMessageId ?? this.Id,
				Mode = this.Mode?.ToString(),
				Float1 = this.Float1,
				Float2 = this.Float2,
				FloatResult = this.FloatResult,
				Date1 = this.Date1,
				Date2 = this.Date2,
				DateResult = this.DateResult,
				Int1 = (int?)this.Int1,
				Int2 = (int?)this.Int2,
				IntResult = this.IntResult,
				DateTime1 = this.DateTime1,
				DateTime2 = this.DateTime2,
				DateTimeResult = this.DateTimeResult,
				Str1 = this.Str1,
				Str2 = this.Str2,
				StrResult = this.StrResult,
				Url2 = this.Url2,
				Url1 = this.Url1,
				Topic = this.Topic,
				Decimal1 = this.Decimal1,
				Decimal2 = this.Decimal2,
				DecimalResult = this.DecimalResult,
				Message = this.Message,
				MessageType = this.MessageType,
				Log = this.Log,
				Error = this.Error,
				Response = this.Response,
				IsReady = this.Status == Schema.StatusCodes.Active_Ready,
				Failed = this.Status == Schema.StatusCodes.InActive_Failed,
				Completed = this.Status == Schema.StatusCodes.InActive_Completed,

			};

		}

		public bool SetFailed(IXrmRepository<XrmSystemMessage> repo)
		{
			try
			{
				repo.SetState(this, (int)DefaultStateCodes.InActive, (int)Schema.StatusCodes.InActive_Failed);
			}
			catch { }
			return false;
		}
		public bool SetCompleted(IXrmRepository<XrmSystemMessage> repo)
		{
			try
			{
				repo.SetState(this, (int)DefaultStateCodes.InActive, (int)Schema.StatusCodes.InActive_Completed);
			}
			catch { }
			return false;
		}

		public bool IsCommand()
		{
			return this.Mode == Schema.MessageModes.Command || this.Mode == Schema.MessageModes.AsyncCommand;
		}
		public bool IsAsyncCommand()
		{
			return this.Mode == Schema.MessageModes.AsyncCommand;
		}

		public bool IsSyncCommand()
		{
			return this.Mode == Schema.MessageModes.Command;
		}

		public bool IsEvent()
		{
			return this.Mode == Schema.MessageModes.Event;
		}
		public XrmSystemMessage UpdateRely(XrmSystemMessageReplyModel model)
		{
			this.Str1 = model.Str1;
			this.Str2 = model.Str2;
			this.StrResult = model.StrResult;
			this.Log = model.Log;
			this.Date1 = model.Date1;
			this.Date2 = model.Date2;
			this.DateTime1 = model.DateTime1;
			this.DateTime2 = model.DateTime2;
			this.DateTimeResult = model.DateTimeResult;
			this.DateResult = model.DateResult;
			this.Error = model.Error;
			this.Float1 = model.Float1;
			this.Float2 = model.Float2;
			this.FloatResult = model.FloatResult;
			this.Int1 = model.Int1;
			this.Int2 = model.Int2;
			this.IntResult = model.IntResult;
			this.Message = model.Message;
			this.MessageType = model.MessageType;
			this.Topic = model.Topic;
			this.Url1 = model.Url1;
			this.Url2 = model.Url2;
			this.Decimal1 = model.Decimal1;
			this.Decimal2 = model.Decimal2;
			this.DecimalResult = model.DecimalResult;
			this.Response = model.Response;

			return this;
		}

	}
}
