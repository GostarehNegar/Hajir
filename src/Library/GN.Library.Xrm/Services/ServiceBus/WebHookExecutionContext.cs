using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GN.Library.Xrm.Services.ServiceBus
{
	public class WebHookExecutionContext
	{
		public class EntityImage
		{
			public Guid Id { get; set; }
			public string LogicalName { get; set; }
			public List<KeyValuePair<string, object>> Attributes { get; set; }
			public List<KeyValuePair<string, string>> FormattedValues { get; set; }

		}
		[JsonIgnore]
		public string JsonRaw { get; set; }
		[JsonIgnore]
		private JObject jobject;
		public Guid BusinessUnitId { get; set; }
		public List<KeyValuePair<string, object>> SharedVariables { get; set; }
		public List<KeyValuePair<string, object>> InputParameters { get; set; }
		public List<KeyValuePair<string, object>> OutputParameters { get; set; }
		public List<KeyValuePair<string, EntityImage>> PreEntityImages { get; set; }
		public List<KeyValuePair<string, EntityImage>> PostEntityImages { get; set; }
		public Guid InitiatingUserId { get; set; }
		public bool IsExecutingOffline { get; set; }
		public bool IsInTransaction { get; set; }
		public bool IsOfflinePlayback { get; set; }
		public int IsolationMode { get; set; }
		public string MessageName { get; set; }
		public int Mode { get; set; }
		public DateTime OperationCreatedOn { get; set; }
		public Guid OperationId { get; set; }
		public Guid OrganizationId { get; set; }
		public string OrganizationName { get; set; }
		public Guid PrimaryEntityId { get; set; }
		public string PrimaryEntityName { get; set; }
		public Guid RequestId { get; set; }

		public EntityImage GetTarget()
		{
			EntityImage result = null;
			if (this.InputParameters != null && this.InputParameters.Any(x => x.Key == "Target"))
			{
				var obj = this.InputParameters.First(x => x.Key == "Target").Value as JObject;
				if (obj != null)
				{
					result = obj.ToObject<EntityImage>();
				}
			}
			return result;
		}
		public WebHookExecutionContext()
		{

		}
		public JObject GetJObject(bool refresh = false)
		{
			if (this.jobject == null || refresh)
				this.jobject = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(this.JsonRaw);
			return this.jobject;
		}

		public static WebHookExecutionContext Deseriallize(string json)
		{
			var str = FormatJson(json);
			var result = Newtonsoft.Json.JsonConvert.DeserializeObject<WebHookExecutionContext>(str);
			result.JsonRaw = str;
			return result;

		}
		public static string FormatJson(string unformattedJson)
		{
			string formattedJson = string.Empty;
			try
			{
				formattedJson = unformattedJson.Trim('"');
				formattedJson = System.Text.RegularExpressions.Regex.Unescape(formattedJson);
			}
			catch (Exception ex)
			{

				throw new Exception(ex.Message);
			}
			return formattedJson;
		}
	}
}
