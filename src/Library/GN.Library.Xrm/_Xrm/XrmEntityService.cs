using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm
{
	public interface IXrmEntityService
	{
		XrmEntity This { get; }

	}
	public interface IXrmEntityService<TEntity> : IXrmEntityService where TEntity : XrmEntity
	{
		TEntity This { get; set; }
		void SetEntity(TEntity entity);
		T GetService<T>();
		TEntity Update();
		IXrmRepository<TEntity> GetRepository();
		IXrmEntitySchema GetSchema();
	}

	public class XrmEntityService<TEntity> : IXrmEntityService<TEntity> where TEntity : XrmEntity
	{
		public TEntity This { get; set; }
		XrmEntity IXrmEntityService.This => this.This;
		public ConcurrentDictionary<string, object> Properties { get; private set; }
		public XrmEntityService() : this(null)
		{

		}
		public XrmEntityService(TEntity entity)
		{
			this.This = entity;
			this.Properties = new ConcurrentDictionary<string, object>();
		}
		public T ToEnity<T>() where T : Entity
		{
			return This?.ToEntity<T>();
		}
		public T GetAttributeValue<T>(string name)
		{
			return This == null ? default(T) : This.GetAttributeValue<T>(name);
		}
		public void SetAttributeValue(string name, object value)
		{
			This.Attributes[name] = value;
		}
		public T GetService<T>()
		{
			return AppHost.Context.AppServices.GetService<T>();
		}
		public IXrmRepository<TEntity> GetRepository()
		{
			return GetService<IXrmRepository<TEntity>>();
			//return GetOrAddProperty<IXrmRepository<TEntity>>(() => GetService<IXrmRepository<TEntity>>());
		}
		public IXrmEntitySchema GetSchema()
		{
			var service = GetService<IXrmSchemaService>();
			return This == null || !service.EntityExists(This.LogicalName) ? null : service.GetSchema(This?.LogicalName);
		}

		public void SetEntity(TEntity entity)
		{
			this.This = entity;
		}

		public TEntity Update()
		{
			if (This != null)
				GetRepository().Update(This);
			return This;
		}
		public TEntity Upsert()
		{
			if (This != null)
				GetRepository().Upsert(This);
			return This;
		}
		public void Delete()
		{
			if (This != null)
				GetRepository().Delete(This);

		}
		public TEntity Refresh(Guid? id = null)
		{
			if (This != null && ((id ?? This.Id) != Guid.Empty))
			{
				This = GetRepository().Retrieve(id ?? This.Id, This.LogicalName);
			}
			return This;
		}

		public T GetOrAddProperty<T>(Func<T> factory, string key = null)
		{
			key = key ?? "" + typeof(T).AssemblyQualifiedName;
			return (T)this.Properties.GetOrAdd(key, k =>
			{
				return factory == null ? default(T) : factory();
			});
		}

	}
}
