using GN.Library.Odoo.Internal.Concrete;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Linq;
using GN.Library.Data;
using GN.Library.Data.Complex;

namespace GN.Library.Odoo
{
	public class OdooEntity
	{
		protected object __CONTEXT__;
		public class Schema
		{
			public const string Write_Date = "write_date";
			public const string DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
			public const string DATE_FORMAT = "yyyy-MM-dd";
		}

		protected RpcRecord record;
		protected IOdooConnection connection;
		public string LogicalName { get; protected set; }
		public OdooEntity(string logicalName)
		{

			this.LogicalName = logicalName;

		}
		protected OdooEntity(RpcRecord record)
		{
			this.record = record;
		}
		protected RpcRecord GetRecord()
		{
			if (this.record == null)
			{
				var connection = this.GetConnection();
				this.record = new RpcRecord(connection.GetRpcConnection(), this.LogicalName, null, connection.GetSchema(this.LogicalName).GetFields());
			}
			return this.record;
		}
		internal T init<T>(IOdooConnection connection, RpcRecord record) where T : OdooEntity
		{
			this.record = record;
			this.connection = connection;
			return (T)this;
		}

		public T ToEntity<T>() where T : OdooEntity, new()
		{
			var result = new T
			{
				connection = this.connection,
				record = this.record
			};
			return result;

		}

		public DateTime? ModifiedOn { get => this.GetAttributeValue<DateTime?>(Schema.Write_Date); }

		public T GetAttributeValue<T>(string name)
		{
			var f = this.GetRecord().GetField(name);
			object result = f?.Value;
			if (result == null)
				return (T)result;
			if (f.Type == "manytomany" && typeof(T) == typeof(object[]) && result as object[] != null)
			{


			}
			if (typeof(T) == typeof(int[]) && result != null && result.GetType() == typeof(object[]) && result as object[] != null)
			{
				result = (result as object[]).Select(x => x != null && int.TryParse(x.ToString(), out var tmp) ? tmp : -1).ToArray();
			}
			if (f.Type == "many2one" && typeof(T) == typeof(int?) && result as object[] != null)
			{
				result = (result as object[]).Length > 0
					? (result as object[])[0]
					: null;
			}
			if (typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTime?))
			{
				if (f != null && f.Type == "datetime")
				{

					if (DateTime.TryParseExact((string)f.Value, Schema.DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out var _date))
					{
						DateTime.SpecifyKind(_date, DateTimeKind.Utc);
						result = _date;
					}
					else if (DateTime.TryParseExact((string)f.Value, Schema.DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out var _date2))
					{
						DateTime.SpecifyKind(_date2, DateTimeKind.Utc);
						result = _date2;

					}
					else
					{
						result = typeof(T) == typeof(DateTime?) ? (DateTime?)null : new DateTime();
					}
				}
				else if (f != null && f.Type == "date")
				{
					if (DateTime.TryParseExact((string)f.Value, Schema.DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out var _date))
					{
						DateTime.SpecifyKind(_date, DateTimeKind.Utc);
						result = _date;
					}
					else if (DateTime.TryParseExact((string)f.Value, Schema.DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out var _date2))
					{
						DateTime.SpecifyKind(_date2, DateTimeKind.Utc);
						result = _date2;
					}
					else
					{
						result = typeof(T) == typeof(DateTime?) ? (DateTime?)null : new DateTime();
					}
				}
				else
				{
					result = typeof(T) == typeof(DateTime?) ? (DateTime?)null : new DateTime();
				}
			}
			if (typeof(T) == typeof(float) || typeof(T) == typeof(float?))
			{
				result = f?.Value != null && float.TryParse(f?.Value?.ToString(), out var tmp) ? tmp : (float?)null;
			}
			return (T)result;
		}
		public void SetAttributeValue(string name, object value)
		{
			var f = this.GetRecord().GetField(name);
			if (f?.Type == "float" && value != null && value.GetType() != typeof(double))
			{
				value = Double.TryParse(value.ToString(), out var d) ? d : (double?)null;
			}
			if (value != null && (value.GetType() == typeof(DateTime) || value.GetType() == typeof(DateTime?)))
			{
				var v = (DateTime)value;
				value = v.ToString(Schema.DATETIME_FORMAT, CultureInfo.InvariantCulture);
			}
			this.GetRecord().SetFieldValue(name, value);
		}
		public int Id { get => this.record?.Id ?? -1; set { if (this.record != null) { this.record.Id = value; } } }

		public IOdooConnection GetConnection()
		{
			this.connection = this.connection ?? AppHost.GetService<IOdooConnection>();
			return this.connection;
		}

		public virtual void Save()
		{
			this.GetRecord().Save();


		}
		public virtual void Delete()
		{
			if (this.Id > 0)
				this.GetConnection().GetRpcConnection().Remove(this.LogicalName, new int[] { this.Id });
		}




		public IOdooSchema GetSchema()
		{
			return this.GetConnection().GetSchema(this.LogicalName);
		}
	}

	public class OdooEntity<T> : OdooEntity, IComplexEntity<int> where T : OdooEntity, IComplexEntity
	{
		private IEntityContext<T> _GetContext()
		{
			return (this as T).GetContext();
		}
		private static IEntityContext<T> DoGetContext(OdooEntity<T> This)
		{
			var entity = This as T;
			if (entity == null)
				throw new Exception("Invalid Entity Type.");
			return entity.GetContext();
		}
		public OdooEntity(string logicalName) : base(logicalName)
		{
			this.LogicalName = logicalName;
			DoGetContext(this).AddHandler(HandleEx);
		}

		public static bool TryGetConvert(object source, string odooFiledType, Type targetType, out object result)
		{
			var success = false;
			result = source;
			var fType = odooFiledType;
			if (result == null)
				return success;
			if (targetType == typeof(int[]) && result != null && result.GetType() == typeof(object[]) && result as object[] != null)
			{
				result = (result as object[]).Select(x => x != null && int.TryParse(x.ToString(), out var tmp) ? tmp : -1).ToArray();
			}
			if (fType == "many2one" && targetType == typeof(int?) && result as object[] != null)
			{
				result = (result as object[]).Length > 0
					? (result as object[])[0]
					: null;
			}
			if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
			{
				if (fType == "datetime")
				{

					if (DateTime.TryParseExact((string)source.ToString(), Schema.DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out var _date))
					{
						DateTime.SpecifyKind(_date, DateTimeKind.Utc);
						result = _date;
					}
					else if (DateTime.TryParseExact((string)source.ToString(), Schema.DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out var _date2))
					{
						DateTime.SpecifyKind(_date2, DateTimeKind.Utc);
						result = _date2;

					}
					else
					{
						result = targetType == typeof(DateTime?) ? (DateTime?)null : new DateTime();
					}
				}
				else if (fType == "date")
				{
					if (DateTime.TryParseExact((string)source.ToString(), Schema.DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out var _date))
					{
						DateTime.SpecifyKind(_date, DateTimeKind.Utc);
						result = _date;
					}
					else if (DateTime.TryParseExact((string)source.ToString(), Schema.DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out var _date2))
					{
						DateTime.SpecifyKind(_date2, DateTimeKind.Utc);
						result = _date2;
					}
					else
					{
						result = targetType == typeof(DateTime?) ? (DateTime?)null : new DateTime();
					}
				}
				else
				{
					result = targetType == typeof(DateTime?) ? (DateTime?)null : new DateTime();
				}
			}
			if (targetType == typeof(float) || targetType == typeof(float?))
			{
				result = source != null && float.TryParse(source.ToString(), out var tmp) ? tmp : (float?)null;
			}
			success = (result != null && targetType.IsAssignableFrom(result.GetType())) ||
					  (result == null && (!targetType.IsValueType || Nullable.GetUnderlyingType(targetType) != null));
			return success;
		}
		public static IComplexEntityMessage HandleEx(IComplexEntity entity, IComplexEntityMessage message)
		{
			var odoo_entity = entity as T;
			if (odoo_entity != null)
			{
				var meta = odoo_entity.GetContext().GetMetaData();
				switch (message)
				{
					case EntityMessages.GetAttributeNames m:
						m.AttributeNames = odoo_entity.GetConnection().GetSchema(odoo_entity.LogicalName)
							.GetFields()
							.Select(x => x.FieldName).ToArray();
						return m.Completed();
					case EntityMessages.GetRawValue m:
						m.Value = odoo_entity.GetAttributeValue<object>(m.AttributeName);
						return m.Completed();
					case EntityMessages.SetRawValue m:
						odoo_entity.SetAttributeValue(m.AttributeName, m.Value);
						return m.Completed();
					case EntityMessages.GetAttributeMetaData m:
						var field = odoo_entity
								.GetSchema()
								.GetFields()
								.FirstOrDefault(x => x.FieldName == m.AttributeName);
						if (field != null)
						{
							m.CustomData = field;
							return m.Completed();
						}
						break;
					case EntityMessages.Convert m:
						switch (m.Operation)
						{
							case EntityMessages.Convert.ConversionOperationType.Get:
								var f = m.SourceValue.GetMetaData().CustomData as OdooField;
								if (f != null && TryGetConvert(m.SourceValue.GetRawValue(), f.Type, m.TargetType, out var _TMP))
								{
									m.ResultValue = _TMP;
									return m.Completed();
								}
								break;
							case EntityMessages.Convert.ConversionOperationType.Set:
								break;
							case EntityMessages.Convert.ConversionOperationType.Serialize:
								break;
							case EntityMessages.Convert.ConversionOperationType.Deserialize:
								break;
							default: break;
						}
						break;

					default:
						break;

				}
			}


			return message;
		}
	}

	public static class OdooEntityExtensions
	{
		public static float? ToFloatValue(this OdooEntity e, object value)
		{
			return value != null && float.TryParse(value.ToString(), out var f)
				? f
				: (float?)null;

		}
	}
}
