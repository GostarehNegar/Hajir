using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xrm.Tools.WebAPI.Requests;
using Xrm.Tools.WebAPI.Results;

namespace Xrm.Tools.WebAPI.Metadata
{
	public static class CRMWebAPIMetadataExtensions
	{
		public static async Task<ExpandoObject> GetOptionSetByName(this CRMWebAPI api, string optionSetName)
		{
			CRMGetListOptions options = new CRMGetListOptions() { Select = new[] { "Name" } };

			var queryResult = await api.GetList("GlobalOptionSetDefinitions", options);

			foreach (dynamic optionSet in queryResult.List)
			{

				if ((optionSet != null) && (optionSet.Name == optionSetName))
				{
					var matchingOptionSet = await api.Get("GlobalOptionSetDefinitions", Guid.Parse(optionSet.MetadataId));

					return matchingOptionSet;
				}
			}

			return null;

		}


		public static async Task<List<CRMOptionDisplayValue>> GetOptionSetUserLabels(this CRMWebAPI api, string optionSetName)
		{
			var response = new List<CRMOptionDisplayValue>();

			dynamic optionSet = await api.GetOptionSetByName(optionSetName);

			foreach (dynamic option in optionSet.Options)
			{
				CRMOptionDisplayValue odv = new CRMOptionDisplayValue();
				odv.Value = option.Value;
				odv.Label = option.Label.UserLocalizedLabel.Label;
				response.Add(odv);
			}

			return response;
		}

		public static async Task<List<CRMEntityDisplayName>> GetEntityDisplayNameList(this CRMWebAPI api, string logicalName, int LCID = 0)
		{
			var result = new List<CRMEntityDisplayName>();

			CRMGetListOptions options = new CRMGetListOptions()
			{
				Filter = "IsPrivate eq false",

				//Select = new[] { "MetadataId","EntitySetName","DisplayName",
				//		"DisplayCollectionName","LogicalName","LogicalCollectionName","PrimaryIdAttribute" }
			};

			var queryResults = await api.GetList("EntityDefinitions", options);

			return await Task.FromResult(result).ConfigureAwait(false);

		}

		public static async Task<List<CRMEntityDisplayName>> GetEntityDisplayNameList(this CRMWebAPI api, int LCID = 0)
		{
			var result = new List<CRMEntityDisplayName>();

			CRMGetListOptions options = new CRMGetListOptions()
			{
				Filter = "IsPrivate eq false",

				Select = new[] { "MetadataId","EntitySetName","DisplayName",
						"DisplayCollectionName","LogicalName","LogicalCollectionName","PrimaryIdAttribute" }
			};

			var queryResults = await api.GetList("EntityDefinitions", options);

			foreach (dynamic entity in queryResults.List)
			{
				CRMEntityDisplayName edm = new CRMEntityDisplayName();
				edm.MetadataId = Guid.Parse(entity.MetadataId);
				edm.EntitySetName = entity.EntitySetName;
				edm.LogicalName = entity.LogicalName;
				edm.LogicalCollectionName = entity.LogicalCollectionName;
				edm.PrimaryIdAttribute = entity.PrimaryIdAttribute;
				if ((entity.DisplayName.LocalizedLabels != null) && (entity.DisplayName.LocalizedLabels.Count > 0))
				{
					edm.DisplayName = entity.DisplayName.LocalizedLabels[0].Label;
					if (LCID != 0)
						foreach (dynamic label in entity.DisplayName.LocalizedLabels)
						{ if (label.LanguageCode == LCID) edm.DisplayName = label.Label; }

				}
				else
					edm.DisplayName = edm.LogicalName;
				if ((entity.DisplayCollectionName.LocalizedLabels != null) && (entity.DisplayCollectionName.LocalizedLabels.Count > 0))
				{
					edm.DisplayCollectionName = entity.DisplayCollectionName.LocalizedLabels[0].Label;
					if (LCID != 0)
						foreach (dynamic label in entity.DisplayCollectionName.LocalizedLabels)
						{ if (label.LanguageCode == LCID) edm.DisplayCollectionName = label.Label; }
				}
				else
					edm.DisplayCollectionName = entity.LogicalCollectionName;
				edm.LogicalDisplayName = edm.DisplayName + "(" + edm.LogicalName + ")";
				edm.LogicalDisplayCollectionName = edm.DisplayCollectionName + "(" + edm.LogicalCollectionName + ")";

				result.Add(edm);

			}

			return result;

		}

		public static async Task<List<RelationShipMetadata>> GetManyToOneRelationShips(this CRMWebAPI api,string entityLogicalName)
		{
			var result = new List<RelationShipMetadata>();
			var queryResults = await api.GetList($"EntityDefinitions(LogicalName='{entityLogicalName}')/ManyToOneRelationships");
			if (queryResults.List != null)
            {
				foreach(var item in queryResults.List)
                {
					try
					{
						var data = (IDictionary<String, Object>)item;
						var result_item = new RelationShipMetadata
						{
							ReferencingAttribute = data.TryGetValue("ReferencedAttribute", out var _pop) ? _pop.ToString() : "",
							ReferencingEntityNavigationPropertyName = data.TryGetValue("ReferencingEntityNavigationPropertyName", out var _d) ? _d.ToString() : "",
							MoreData = data

						};
						result.Add(result_item);
					}
                    catch
                    {

                    }
				}
            }

			return result;
        }
		public static async Task<List<LookupAttributeData>> GetLookupFields(this CRMWebAPI api, string entityLogicalName)
		{
			var result = new List<LookupAttributeData>();
			CRMGetListOptions options = new CRMGetListOptions()
			{
				Filter = "((IsValidForRead eq true) and (AttributeOf eq null))",

				Select = new[] { "MetadataId", "DisplayName", "LogicalName", "SchemaName", "AttributeType", "IsPrimaryId", "Targets" }
			};

			var queryResults = await api.GetList($"EntityDefinitions(LogicalName='{entityLogicalName}')/Attributes/Microsoft.Dynamics.CRM.LookupAttributeMetadata", options);
			foreach (dynamic attrib in queryResults.List)
			{
				var item = new LookupAttributeData
				{
					MetadataId = Guid.Parse(attrib.MetadataId),
					LogicalName = attrib.LogicalName,
					Targets = new List<string>()
				};
				foreach (var target in attrib.Targets)
				{
					item.Targets.Add(target as string);
				}
				result.Add(item);
			}

			return result;
		}


		public static async Task<List<CRMAttributeDisplayName>> GetAttributeDisplayNameList(this CRMWebAPI api, string entityLogicalName, int LCID = 0)
		{
			var result = new List<CRMAttributeDisplayName>();

			CRMGetListOptions options = new CRMGetListOptions()
			{
				Filter = "((IsValidForRead eq true) and (AttributeOf eq null))",

				Select = new[] { "MetadataId", "DisplayName", "LogicalName", "SchemaName", "AttributeType", "IsPrimaryId", "IsCustomAttribute" }
			};

			var queryResults = await api.GetList("EntityDefinitions(LogicalName='" + entityLogicalName + "')/Attributes", options);

			foreach (dynamic attrib in queryResults.List)
			{
				CRMAttributeDisplayName edm = new CRMAttributeDisplayName();
				edm.MetadataId = Guid.Parse(attrib.MetadataId);
				edm.LogicalName = attrib.LogicalName;
				edm.SchemaName = attrib.SchemaName;
				edm.IsPrimaryId = attrib.IsPrimaryId;
				edm.AttributeType = attrib.AttributeType;
				edm.IsCustomAttribute = attrib.IsCustomAttribute;
				//edm.EntityLogicalName = attrib.EntityLogicalName;
				if (attrib.AttributeType == "Lookup" || attrib.AttributeType == "Customer" || attrib.AttributeType == "Owner")
					edm.ODataLogicalName = "_" + attrib.LogicalName + "_value";
				else
					edm.ODataLogicalName = attrib.LogicalName;

				if ((attrib.DisplayName.LocalizedLabels != null) && (attrib.DisplayName.LocalizedLabels.Count > 0))
				{
					edm.DisplayName = attrib.DisplayName.LocalizedLabels[0].Label;
					if (LCID != 0)
						foreach (dynamic label in attrib.DisplayName.LocalizedLabels)
						{ if (label.LanguageCode == LCID) edm.DisplayName = label.Label; }
				}
				else
					edm.DisplayName = edm.LogicalName;
				edm.LogicalDisplayName = edm.DisplayName + "(" + edm.LogicalName + ")";
				result.Add(edm);
			}



			return result;

		}
		public static async Task<List<CRMAttributeDisplayName>> GetAttributeDisplayNameList(this CRMWebAPI api, Guid entityID, int LCID = 0)
		{
			var result = new List<CRMAttributeDisplayName>();

			CRMGetListOptions options = new CRMGetListOptions()
			{
				Filter = "((IsValidForRead eq true) and (AttributeOf eq null))",

				Select = new[] { "MetadataId", "DisplayName", "LogicalName", "SchemaName", "AttributeType", "IsPrimaryId" }
			};

			var queryResults = await api.GetList("EntityDefinitions(" + entityID.ToString() + ")/Attributes", options);

			foreach (dynamic attrib in queryResults.List)
			{
				CRMAttributeDisplayName edm = new CRMAttributeDisplayName();
				edm.MetadataId = Guid.Parse(attrib.MetadataId);
				edm.LogicalName = attrib.LogicalName;
				edm.SchemaName = attrib.SchemaName;
				edm.IsPrimaryId = attrib.IsPrimaryId;
				edm.AttributeType = attrib.AttributeType;
				if (attrib.AttributeType == "Lookup" || attrib.AttributeType == "Customer" || attrib.AttributeType == "Owner")
					edm.ODataLogicalName = "_" + attrib.LogicalName + "_value";
				else
					edm.ODataLogicalName = attrib.LogicalName;

				if ((attrib.DisplayName.LocalizedLabels != null) && (attrib.DisplayName.LocalizedLabels.Count > 0))
				{
					edm.DisplayName = attrib.DisplayName.LocalizedLabels[0].Label;
					if (LCID != 0)
						foreach (dynamic label in attrib.DisplayName.LocalizedLabels)
						{ if (label.LanguageCode == LCID) edm.DisplayName = label.Label; }
				}
				else
					edm.DisplayName = edm.LogicalName;
				edm.LogicalDisplayName = edm.DisplayName + "(" + edm.LogicalName + ")";
				result.Add(edm);
			}



			return result;

		}

		public static async Task<Guid> CopyEntityAttribute(this CRMWebAPI api, Guid fromEntityID, Guid toEntityID, Guid fromAttributeID, string attributeType, string logicalName)
		{
			var ec = "EntityDefinitions(" + fromEntityID.ToString() + ")/Attributes(" + fromAttributeID + ")";
			if (attributeType == "Boolean")
				ec += "/Microsoft.Dynamics.CRM.BooleanAttributeMetadata?$expand=OptionSet";
			if (attributeType == "Picklist")
				ec += "/Microsoft.Dynamics.CRM.PicklistAttributeMetadata?$expand=OptionSet,GlobalOptionSet";

			dynamic fromAttrib = await api.Get(ec, Guid.Empty);
			IDictionary<string, object> fromAttribValues = (IDictionary<string, object>)fromAttrib;
			fromAttribValues.Remove("MetadataId");
			fromAttribValues.Remove("EntityLogicalName");
			if (attributeType == "Boolean")
			{
				fromAttribValues["@odata.type"] = "Microsoft.Dynamics.CRM.BooleanAttributeMetadata";
				fromAttribValues.Remove("OptionSet@odata.context");
				if (fromAttrib.OptionSet != null)
				{
					IDictionary<string, object> fromOptionSetValues = (IDictionary<string, object>)fromAttrib.OptionSet;
					fromOptionSetValues.Remove("Name");
					fromOptionSetValues.Remove("MetadataId");
					fromOptionSetValues.Remove("MetadataId");
					fromOptionSetValues["IsCustomOptionSet"] = true;

				}
			}
			if (attributeType == "Picklist")
			{

				fromAttribValues["@odata.type"] = "Microsoft.Dynamics.CRM.PicklistAttributeMetadata";


				if (fromAttrib.OptionSet != null)
				{
					IDictionary<string, object> fromOptionSetValues = (IDictionary<string, object>)fromAttrib.OptionSet;
					fromOptionSetValues.Remove("Name");
					fromOptionSetValues.Remove("MetadataId");
					fromOptionSetValues.Remove("MetadataId");
					fromOptionSetValues["IsCustomOptionSet"] = true;

				}
				else
				{
					fromAttribValues.Remove("OptionSet");
					fromAttribValues["GlobalOptionSet@odata.bind"] = "/GlobalOptionSetDefinitions(" + fromAttrib.GlobalOptionSet.MetadataId + ")";
					fromAttribValues.Remove("OptionSet@odata.context");
					fromAttribValues.Remove("GlobalOptionSet");

				}
			}
			fromAttrib.LogicalName = logicalName;
			fromAttrib.SchemaName = logicalName;


			return await api.Create("EntityDefinitions(" + toEntityID.ToString() + ")/Attributes", fromAttrib);
		}

	}
}
