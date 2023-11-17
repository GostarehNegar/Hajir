using GN.Library.Xrm.StdSolution;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;

namespace GN.Library.Xrm.Query.Internal
{
    interface IHaveTypes
    {
        Type[] Types { get; }
    }
    public class XrmDbContext : DbContext, IHaveTypes, IXrmDbContext
    {
        private readonly IXrmDataServices dataServices;
        private List<Type> ignore_types_list = new List<Type>();

        private List<Action<ModelBuilder>> builderActions = new List<Action<ModelBuilder>>();
        private Dictionary<Type, Action<ModelBuilder>> _types = new Dictionary<Type, Action<ModelBuilder>>();
        internal System.Data.Common.DbConnection DbConnection;

        public Type[] Types => this._types.Keys.ToArray();

        public XrmDbContextOptions Options { get; set; }
        public XrmSettings Settings { get; private set; }
        public string ConnectionString { get; set; }
        public IQueryable<XrmContact> Contacts => this.Set<XrmContact>();
        public XrmDbContext(IXrmDataServices dataServices, XrmDbContextOptions options = null)
        {
            this.Settings = dataServices.Settings;
            this.dataServices = dataServices;
            this.ConnectionString = this.Settings.GetDbContextConnectionString();


            this.Options = options ?? new XrmDbContextOptions();
        }
        public XrmDbContext(IXrmDataServices dataServices, string connectionString)
        {
            this.ConnectionString = connectionString;
            this.dataServices = dataServices;
            //this.AddEntity<TEntity>();

        }
        public XrmDbContext(IXrmDataServices dataServices, System.Data.Common.DbConnection dbConnection, XrmDbContextOptions options=null)
        {
            this.DbConnection = dbConnection;
            this.dataServices = dataServices;
            this.Options = options?? new XrmDbContextOptions();

            //this.AddEntity<TEntity>();

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>();
                if (this.DbConnection == null)
                {
                    optionsBuilder.UseSqlServer(this.ConnectionString);
                }
                else
                {
                    optionsBuilder.UseSqlServer(this.DbConnection);
                }
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ignore_types_list.ForEach(x => modelBuilder.Ignore(x));
            //modelBuilder.ApplyConfiguration(new XrmQueryModelConfiguration<TEntity>(this.dataServices));
            var actions = this._types.Values.ToList();
            this.builderActions = this.builderActions ?? new List<Action<ModelBuilder>>();
            actions.ForEach(x => x(modelBuilder));

        }
        public void _AddEntity(Type type, bool includeRelatedEntities)
        {
            if (!this._types.ContainsKey(type))
            {
                var same = this._types.Keys.FirstOrDefault(x => x.InSameHierarchy(type));
                if (same != null)
                {
                    /// Abondon heirarchies by adding this type
                    /// to ignore list.
                    /// These types will be ignored in OnConfiguring model.
                    /// 
                    ignore_types_list = ignore_types_list ?? new List<Type>();
                    ignore_types_list.Add(same.BaseOfHierarchy(type));
                }
                else
                {
                    this.GetType()
                        .GetMethod(nameof(this.__AddEntity))
                        .MakeGenericMethod(type)
                        .Invoke(this, new object[] { });
                }
                if (includeRelatedEntities || this.Options.IncludeRelatedEntities)
                {

                    type.GetRelatedTypes()
                        .ToList()
                        .ForEach(x => this._AddEntity(x, includeRelatedEntities));
                    if (false)
                    {
                        /// This was meant to be used in Hierarchies
                        /// We currently abondon hierarchies.
                        _AddEntity(type.BaseType, includeRelatedEntities);
                    }


                }

            }


        }
        public IXrmDbContext AddEntity<T>(bool includeRelatedEntities = false) where T : XrmEntity
        {
            Type type = typeof(T);
            _AddEntity(type, includeRelatedEntities);
            //this.GetType()
            //       .GetMethod(nameof(this._AddEntity))
            //       .MakeGenericMethod(type)
            //       .Invoke(this, new object[] { });
            return this;
        }
        public IXrmDbContext __AddEntity<T>() where T : XrmEntity
        {
            this._types = this._types ?? new Dictionary<Type, Action<ModelBuilder>>();
            this._types[typeof(T)] = b => b.ApplyConfiguration(new XrmQueryModelConfiguration<T>(this.dataServices, this.Types));

            //this.types = this.types ?? new List<Type>();
            //this.types.Add(typeof(T));
            //this.builderActions.Add(b => b.ApplyConfiguration(new XrmQueryModelConfiguration<T>(this.dataServices, this.Types)));
            return this;
        }
        //IQueryable<TEntity> IXrmDbContext<TEntity>.Query()
        //{

        //    return this.Set<TEntity>();
        //}
        //public IQueryable<T> CreateQuery<T>() where T:XrmEntity
        //{
        //    return this.Set<T>();
        //}

        IQueryable<T> IXrmDbContext.Query<T>()
        {
            return this.Set<T>();
        }

        public bool Has<T>()
        {
            return this._types.ContainsKey(typeof(T));
        }

    }

}
