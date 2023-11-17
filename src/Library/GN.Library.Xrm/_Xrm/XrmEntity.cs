using GN.Library.Data;
using GN.Library.Xrm.Helpers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm
{
	public interface IXrmEntity : IEntity
	{
		string LogicalName { get; }
		Guid Id { get; }
		T GetAttributeValue<T>(string name);
	}

	/// <summary>
	/// Base class for all Xrm entities.
	/// </summary>
	public class XrmEntity : Entity, IXrmEntity
	{
		protected object __CONTEXT__;
		/// <summary>
		/// Static class to acess to entity schema constants.
		/// This is used to centeralize values like field names 
		/// and other schema contants.
		/// Derived classes should provide their own schema.
		/// </summary>
		public class Schema
		{
			public const string StateCode = "statecode";
			public const string StatusCode = "statuscode";
			public const string CreatedOn = "createdon";
			public const string ModifiedOn = "modifiedon";
			public const string CreatedBy = "createdby";
			public const string ModifiedBy = "modifiedby";
			public const string Owner = "ownerid";
			public const string OwningUser = "owninguser";
			public const string OwningTeam = "owningteam";
			public const string OwningBuisnessUnit = "owningbusinessunit";
			public class WhereClauses
			{
			}

		}
		/// <summary>
		/// Entry point to services available for this
		/// entity.
		/// </summary>
		public XrmEntityService<XrmEntity> Services { get; protected set; }

		public XrmEntity(string logicalName) : base(logicalName)
		{
			this.Services = new XrmEntityService<XrmEntity>(this);
		}

		[AttributeLogicalNameAttribute(Schema.StatusCode)]
		public virtual Nullable<int> StatusCode
		{
			get
			{
				var result = this.GetAttributeValue<OptionSetValue>(Schema.StatusCode);
				return result == null ? 0 : result.Value;
			}
			set
			{
				this.SetAttributeValue(Schema.StatusCode,
					value == null ? null : new OptionSetValue(value.Value));
			}
		}

		[AttributeLogicalNameAttribute(Schema.StateCode)]
		public virtual Nullable<int> StateCode
		{
			get
			{
				var result = this.GetAttributeValue<OptionSetValue>(Schema.StateCode);
				return result == null ? 0 : result.Value;
			}
			set
			{
				this.SetAttributeValue(Schema.StateCode,
					value == null ? null : new OptionSetValue(value.Value));
			}
		}

		/// <summary>
		/// Datetime this entity has been created.
		/// </summary>
		[AttributeLogicalNameAttribute(Schema.CreatedOn)]
		public DateTime? CreatedOn
		{
			get
			{
				return this.GetAttributeValue<DateTime?>(Schema.CreatedOn);
			}
			set
			{
				this.SetAttributeValue(Schema.CreatedOn, value);
			}
		}
		/// <summary>
		/// Datetime this entity was last modified.
		/// </summary>
		[AttributeLogicalNameAttribute(Schema.ModifiedOn)]
		public DateTime? ModifiedOn
		{
			get
			{
				return this.GetAttributeValue<DateTime?>(Schema.ModifiedOn);
			}
			set
			{
				this.SetAttributeValue(Schema.ModifiedOn, value);
			}
		}

		[AttributeLogicalNameAttribute(Schema.CreatedBy)]
		public EntityReferenceEx CreatedBy
		{
			get
			{
				return new EntityReferenceEx(this.GetAttributeValue<EntityReference>(Schema.CreatedBy));
			}
			set
			{
				this.SetAttributeValue(Schema.CreatedBy, value?.ToEntityReference());
			}
		}
		//[AttributeLogicalNameAttribute(Schema.CreatedBy)]
		public Guid? CreatedById
		{
			get
			{
				return CreatedBy?.Id;
			}
			set
			{
				this.CreatedBy = value == null ? null : new EntityReferenceEx(value.Value, "systemuser");
			}
		}
		

		[AttributeLogicalNameAttribute(Schema.Owner)]
		public EntityReference Owner
		{
			get
			{
				return this.GetAttributeValue<EntityReference>(Schema.Owner);
			}
			set
			{
				this.SetAttributeValue(Schema.Owner, value);
			}
		}

		[AttributeLogicalNameAttribute(Schema.OwningUser)]
		public EntityReference OwningUser
		{
			get
			{
				return this.GetAttributeValue<EntityReference>(Schema.OwningUser);
			}
			set
			{
				this.SetAttributeValue(Schema.OwningUser, value);
			}
		}
		[AttributeLogicalNameAttribute(Schema.OwningTeam)]
		public EntityReference OwningTeam
		{
			get
			{
				return this.GetAttributeValue<EntityReference>(Schema.OwningTeam);
			}
			set
			{
				this.SetAttributeValue(Schema.OwningTeam, value);
			}
		}
		[AttributeLogicalNameAttribute(Schema.OwningTeam)]
		public EntityReference OwningBusinessUnit
		{
			get
			{
				return this.GetAttributeValue<EntityReference>(Schema.OwningBuisnessUnit);
			}
			set
			{
				this.SetAttributeValue(Schema.OwningBuisnessUnit, value);
			}
		}


		public Guid? OwnerId
		{
			get
			{
				return Owner?.Id;
			}
			//set
			//{
			//    this.Owner = value == null ? null : new EntityReference("systemuser", value.Value);
			//}
		}

		public Guid? OwningUserId
		{
			get
			{
				return this.OwningUser?.Id;
			}
			set
			{
				this.OwningUser = value == null ? null : new EntityReference("systemuser", value.Value);
			}
		}

		public Guid? OwningBusinessUnitId
		{
			get
			{
				return this.OwningBusinessUnit?.Id;
			}
			set
			{
				this.OwningBusinessUnit = value == null ? null : new EntityReference("businessunit", value.Value);
			}
		}
		public Guid? OwningReamId
		{
			get
			{
				return this.OwningTeam?.Id;
			}
			set
			{
				this.OwningTeam = value == null ? null : new EntityReference("team", value.Value);
			}
		}

		public bool IsNew => this.Id == Guid.Empty;

		public void SetAttribiuteValue(string name, object value)
		{
			this.SetAttributeValue(name, value);
		}
		public static string GetLogicalName(Type type)
		{
			var attrbs = type.GetCustomAttributes(typeof(EntityLogicalNameAttribute), true)
				.FirstOrDefault() as EntityLogicalNameAttribute;
			return attrbs?.LogicalName;
		}
		internal static T CreateFromDynamic<T>(ExpandoObject d) where T : XrmEntity
		{
			return CreateFromDynamic(typeof(T), d) as T;
		}
		internal static XrmEntity CreateFromDynamic(Type t, ExpandoObject d)
		{
			XrmEntity result = null;
			if (t == typeof(XrmEntity))
			{
				var logicalName = XrmEntity.GetLogicalName(t);
				if (!string.IsNullOrWhiteSpace(logicalName))
					result = new XrmEntity(logicalName);
			}
			else
			{
				try
				{
					result = Activator.CreateInstance(t) as XrmEntity;
				}
				catch { }
			}
			if (result == null)
			{
				throw new Exception(string.Format("Invalid Entity Type: {0}", t.Name));
			}
			var lst = d.ToList();
			foreach (var p in d)
			{
				var key = p.Key as string;
				var value = p.Value as object;
				if (key == "accountid")
				{

				}
				var isFormattedValue = key != null && key.Contains("@OData.Community.Display.V1.FormattedValue");
				if (isFormattedValue)
				{

				}
				else
				{

					if (!isFormattedValue && value != null && value.GetType() == typeof(Guid))
					{

					}
				}

			}

			return result;
		}


	}
	/// <summary>
	/// Base class for all Xrm entitied with typed Status and State.
	/// </summary>
	/// <typeparam name="TState">enum type for State codes. </typeparam>
	/// <typeparam name="TStatus">enum type for Status codes.</typeparam>
	public class XrmEntity<TState, TStatus> : XrmEntity
		where TState : struct
		where TStatus : struct

	{
		public XrmEntity(string logicalName) : base(logicalName) { }

		/// <summary>
		/// Typed version 'Status' corresponding to StatuCode (int).
		/// Note that you cannnot use it in Linq where clauses.
		/// </summary>
		public Nullable<TStatus> Status
		{
			get
			{
				return this.StatusCode.HasValue
					? (TStatus)Enum.ToObject(typeof(TStatus), this.StatusCode)
					: (Nullable<TStatus>)null;

			}
			set
			{
				this.StatusCode = value == null
					? (int?)null
					: (int?)Convert.ChangeType(value, Enum.GetUnderlyingType(typeof(TStatus)));
			}

		}

		public Nullable<TState> State
		{
			get
			{
				return this.StateCode.HasValue
					? (TState)Enum.ToObject(typeof(TState), this.StateCode)
					: (Nullable<TState>)null;
			}
			set
			{
				this.StateCode = value == null
					? (int?)null
					: (int?)Convert.ChangeType(value, Enum.GetUnderlyingType(typeof(TState)));
			}
		}
	}

	public class XrmEntity<TEntity> : XrmEntity where TEntity : XrmEntity, new()
	{
		[JsonIgnore]
		public new XrmEntityService<TEntity> Services { get; private set; }
		public XrmEntity(string logicalName) : base(logicalName)
		{
			this.Services = new XrmEntityService<TEntity>(this as TEntity);
		}

		public TEntity Clone()
		{
			var result = new TEntity
			{
				Attributes = this.Attributes,
				Id = this.Id
			};// Activator.CreateInstance(typeof(TEntity)) as TEntity;
			return result;
		}
	}

	public class XrmEntity<TEntity, TState, TStatus> : XrmEntity<TEntity>
		where TEntity : XrmEntity, new()
		where TState : struct
		where TStatus : struct
	{
		[JsonIgnore]
		public new XrmEntityService<TEntity> Services { get; private set; }
		public XrmEntity(string logicalName) : base(logicalName)
		{
			this.Services = new XrmEntityService<TEntity>(this as TEntity);
		}

		/// <summary>
		/// Typed version 'Status' corresponding to StatuCode (int).
		/// Note that you cannnot use it in Linq where clauses.
		/// </summary>
		public Nullable<TStatus> Status
		{
			get
			{
				return this.StatusCode.HasValue
					? (TStatus)Enum.ToObject(typeof(TStatus), this.StatusCode)
					: (Nullable<TStatus>)null;

			}
			set
			{
				this.StatusCode = value == null
					? (int?)null
					: (int?)Convert.ChangeType(value, Enum.GetUnderlyingType(typeof(TStatus)));
			}

		}

		public Nullable<TState> State
		{
			get
			{
				return this.StateCode.HasValue
					? (TState)Enum.ToObject(typeof(TState), this.StateCode)
					: (Nullable<TState>)null;
			}
			set
			{
				this.StateCode = value == null
					? (int?)null
					: (int?)Convert.ChangeType(value, Enum.GetUnderlyingType(typeof(TState)));
			}
		}

	}
	public class XrmEntity<TEntity, TService, TStates, TStatus> : XrmEntity<TEntity, TStates, TStatus>
		where TEntity : XrmEntity, new()
		where TService : IXrmEntityService<TEntity>
		where TStates : struct
		where TStatus : struct
	{
		private TService services;
		private TService GetServices(bool refersh = false)
		{
			if (this.services == null || refersh)
			{
				this.services = AppHost.GetService<TService>();// ??
				if (this.services == null)
					this.services = (TService)Activator.CreateInstance(typeof(TService), this);
				this.services.SetEntity(this as TEntity);
			}
			return this.services;
		}
		public new TService Services { get { return this.GetServices(); } }
		public XrmEntity(string logicalName) : base(logicalName)
		{
		}
	}





	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("team")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("CrmSvcUtil", "5.0.9688.32")]
	class Team : Entity
	{

		/// <summary>
		/// Default Constructor.
		/// </summary>
		public Team() :
		base(EntityLogicalName)
		{
		}

		public const string EntityLogicalName = "team";

		public const int EntityTypeCode = 9;

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;

		private void OnPropertyChanged(string propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		private void OnPropertyChanging(string propertyName)
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, new System.ComponentModel.PropertyChangingEventArgs(propertyName));
			}
		}

		/// <summary>
		/// Unique identifier of the user primary responsible for the team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("administratorid")]
		public Microsoft.Xrm.Sdk.EntityReference AdministratorId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("administratorid");
			}
			set
			{
				this.OnPropertyChanging("AdministratorId");
				this.SetAttributeValue("administratorid", value);
				this.OnPropertyChanged("AdministratorId");
			}
		}

		/// <summary>
		/// Unique identifier of the business unit with which the team is associated.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("businessunitid")]
		public Microsoft.Xrm.Sdk.EntityReference BusinessUnitId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("businessunitid");
			}
			set
			{
				this.OnPropertyChanging("BusinessUnitId");
				this.SetAttributeValue("businessunitid", value);
				this.OnPropertyChanged("BusinessUnitId");
			}
		}

		/// <summary>
		/// Unique identifier of the user who created the team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		public Microsoft.Xrm.Sdk.EntityReference CreatedBy
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("createdby");
			}
		}

		///// <summary>
		///// Date and time when the team was created.
		///// </summary>
		//[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdon")]
		//public System.Nullable<System.DateTime> CreatedOn
		//{
		//	get
		//	{
		//		return this.GetAttributeValue<System.Nullable<System.DateTime>>("createdon");
		//	}
		//}

		/// <summary>
		/// Unique identifier of the delegate user who created the team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		public Microsoft.Xrm.Sdk.EntityReference CreatedOnBehalfBy
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("createdonbehalfby");
			}
		}

		/// <summary>
		/// Description of the team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("description")]
		public string Description
		{
			get
			{
				return this.GetAttributeValue<string>("description");
			}
			set
			{
				this.OnPropertyChanging("Description");
				this.SetAttributeValue("description", value);
				this.OnPropertyChanged("Description");
			}
		}

		/// <summary>
		/// E-mail address for the team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("emailaddress")]
		public string EMailAddress
		{
			get
			{
				return this.GetAttributeValue<string>("emailaddress");
			}
			set
			{
				this.OnPropertyChanging("EMailAddress");
				this.SetAttributeValue("emailaddress", value);
				this.OnPropertyChanged("EMailAddress");
			}
		}

		/// <summary>
		/// Exchange rate for the currency associated with the team with respect to the base currency.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("exchangerate")]
		public System.Nullable<decimal> ExchangeRate
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<decimal>>("exchangerate");
			}
		}

		/// <summary>
		/// Unique identifier of the data import or data migration that created this record.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("importsequencenumber")]
		public System.Nullable<int> ImportSequenceNumber
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<int>>("importsequencenumber");
			}
			set
			{
				this.OnPropertyChanging("ImportSequenceNumber");
				this.SetAttributeValue("importsequencenumber", value);
				this.OnPropertyChanged("ImportSequenceNumber");
			}
		}

		/// <summary>
		/// Information about whether the team is a default business unit team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("isdefault")]
		public System.Nullable<bool> IsDefault
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("isdefault");
			}
		}

		/// <summary>
		/// Unique identifier of the user who last modified the team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		public Microsoft.Xrm.Sdk.EntityReference ModifiedBy
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("modifiedby");
			}
		}

		/// <summary>
		/// Date and time when the team was last modified.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedon")]
		public System.Nullable<System.DateTime> ModifiedOn
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("modifiedon");
			}
		}

		/// <summary>
		/// Unique identifier of the delegate user who last modified the team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		public Microsoft.Xrm.Sdk.EntityReference ModifiedOnBehalfBy
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("modifiedonbehalfby");
			}
		}

		/// <summary>
		/// Name of the team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("name")]
		public string Name
		{
			get
			{
				return this.GetAttributeValue<string>("name");
			}
			set
			{
				this.OnPropertyChanging("Name");
				this.SetAttributeValue("name", value);
				this.OnPropertyChanged("Name");
			}
		}

		/// <summary>
		/// Unique identifier of the organization associated with the team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("organizationid")]
		public System.Nullable<System.Guid> OrganizationId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("organizationid");
			}
		}

		/// <summary>
		/// Date and time that the record was migrated.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("overriddencreatedon")]
		public System.Nullable<System.DateTime> OverriddenCreatedOn
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("overriddencreatedon");
			}
			set
			{
				this.OnPropertyChanging("OverriddenCreatedOn");
				this.SetAttributeValue("overriddencreatedon", value);
				this.OnPropertyChanged("OverriddenCreatedOn");
			}
		}

		/// <summary>
		/// Unique identifier of the default queue for the team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("queueid")]
		public Microsoft.Xrm.Sdk.EntityReference QueueId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("queueid");
			}
			set
			{
				this.OnPropertyChanging("QueueId");
				this.SetAttributeValue("queueid", value);
				this.OnPropertyChanged("QueueId");
			}
		}

		/// <summary>
		/// Unique identifier for the team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("teamid")]
		public System.Nullable<System.Guid> TeamId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("teamid");
			}
			set
			{
				this.OnPropertyChanging("TeamId");
				this.SetAttributeValue("teamid", value);
				if (value.HasValue)
				{
					base.Id = value.Value;
				}
				else
				{
					base.Id = System.Guid.Empty;
				}
				this.OnPropertyChanged("TeamId");
			}
		}

		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("teamid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.TeamId = value;
			}
		}

		/// <summary>
		/// Unique identifier of the currency associated with the team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("transactioncurrencyid")]
		public Microsoft.Xrm.Sdk.EntityReference TransactionCurrencyId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("transactioncurrencyid");
			}
			set
			{
				this.OnPropertyChanging("TransactionCurrencyId");
				this.SetAttributeValue("transactioncurrencyid", value);
				this.OnPropertyChanged("TransactionCurrencyId");
			}
		}

		/// <summary>
		/// Version number of the team.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("versionnumber")]
		public System.Nullable<long> VersionNumber
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<long>>("versionnumber");
			}
		}

		/// <summary>
		/// Pronunciation of the full name of the team, written in phonetic hiragana or katakana characters.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("yominame")]
		public string YomiName
		{
			get
			{
				return this.GetAttributeValue<string>("yominame");
			}
			set
			{
				this.OnPropertyChanging("YomiName");
				this.SetAttributeValue("yominame", value);
				this.OnPropertyChanged("YomiName");
			}
		}


	}
}
