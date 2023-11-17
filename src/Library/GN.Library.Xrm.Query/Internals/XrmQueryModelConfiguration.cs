using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using GN.Library.Xrm.StdSolution;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GN.Library.Xrm.Query.Internal
{
    public class XrmQueryModelConfiguration<T> : IEntityTypeConfiguration<T> where T : XrmEntity
    {
        private readonly IXrmDataServices dataServices;
        private IXrmEntitySchema schema;
        private Type[] types;

        public XrmQueryModelConfiguration(IXrmDataServices dataServices, Type[] types)
        {
            this.dataServices = dataServices;
            this.types = types;
        }
        public string LogicalName => (Activator.CreateInstance(typeof(T)) as IXrmEntity)?.LogicalName;

        public IXrmEntitySchema GetSchema(bool refersh = false)
        {
            if (schema == null || refersh)
            {
                this.schema = this.dataServices.GetSchemaService()
                    .GetSchema(this.LogicalName);
            }
            return this.schema;

        }

        public string GetViewName()
        {
            var schema = this.GetSchema();
            return schema.LogicalName;


        }
        public bool ConfigureOwner(EntityTypeBuilder<T> builder, PropertyInfo prop)
        {
            var types = new HashSet<Type>(new Type[] {
                typeof(XrmSystemUser) });
            var ownings = new HashSet<string>(new string[] {
                XrmEntity.Schema.Owner,XrmEntity.Schema.OwningTeam, XrmEntity.Schema.OwningBuisnessUnit, XrmEntity.Schema.OwningUser ,
                XrmEntity.Schema.StateCode, XrmEntity.Schema.StatusCode });
            var is_type = types.Any(x =>  x.IsAssignableFrom(typeof(T)));

            if (is_type && ownings.Contains(prop.GetColumnName()))
            {
                builder.Ignore(prop.Name);
                return true;
            }
            return false;
        }
        public bool ConfigureNavigation(EntityTypeBuilder<T> builder, PropertyInfo prop)
        {
            if (!prop.IsNavigation(this.types))
            {
                return false;
            }
            var fromType = typeof(T);
            var toType = prop.PropertyType;
            var foreignKey = prop.GetForeignKeyName() ?? //   prop.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.ForeignKeyAttribute>()?.Name ??
                           fromType.GetProperties().FirstOrDefault(x => string.Compare(x.Name, prop.Name + "Id", true) == 0)?.Name;
            var foreignProp = fromType.GetProperties().FirstOrDefault(x => string.Compare(x.Name, foreignKey, true) == 0);
            if (foreignProp != null)
            {
                toType.GetReverseProperty<T>(prop.Name);
                var fff =
                    toType.GetCollectionProperties<T>().FirstOrDefault(x => x.GetCustomAttribute<InversePropertyAttribute>()?.Property == prop.Name) ??
                    toType.GetCollectionProperties<T>().FirstOrDefault(x => x.GetCustomAttribute<InversePropertyAttribute>()?.Property == foreignKey) ??
                    toType.GetCollectionProperties<T>().FirstOrDefault(x => x.GetCustomAttribute<InversePropertyExAttribute>()?.Property == prop.Name) ??
                    toType.GetCollectionProperties<T>().FirstOrDefault(x => x.GetCustomAttribute<InversePropertyExAttribute>()?.Property == foreignKey) ??
                    toType.GetCollectionProperties<T>().FirstOrDefault(x => x.PropertyType == typeof(ICollection<T>));
                if (fff == null)
                {
                    var bp = builder.HasOne(toType, prop.Name)
                        .WithMany()
                        .HasForeignKey(foreignKey);
                }
                else
                {
                    var bp = builder.HasOne(toType, prop.Name)
                        .WithMany(fff.Name)
                        .HasForeignKey(foreignKey);

                }
                return true;
            }
            return false;
        }
        public bool ConfigureReverseNavigation(EntityTypeBuilder<T> builder, PropertyInfo prop)
        {
            var _type = prop.GetCollectionType();
            if (_type != null)
            {
                if (this.types.Contains(_type))
                {
                    var navigation_prop = prop.GetCustomAttribute<InversePropertyExAttribute>()?.Property ??
                            _type.GetProperties().FirstOrDefault(x => x.PropertyType == typeof(T))?.Name;
                    if (navigation_prop != null)
                    {
                        builder.HasMany(_type, prop.Name)
                        .WithOne(navigation_prop);

                    }
                }
                else
                {
                    builder.Ignore(prop.Name);
                }
                return true;
            }
            return false;
        }

        public bool ConfigureEntityReference(EntityTypeBuilder<T> builder, PropertyInfo prop)
        {
            var columnName = prop.GetColumnName();
            if (!string.IsNullOrWhiteSpace(columnName) && prop.PropertyType == typeof(EntityReference))
            {
                var converter = new ValueConverter<EntityReference, Guid?>(
                   from => from.Id,
                   to => new EntityReference("", to ?? Guid.Empty)
               );
                builder.Property(prop.Name)
                    .HasColumnName(columnName)
                    .HasConversion(converter);
                return true;
            }
            if (!string.IsNullOrWhiteSpace(columnName) && prop.PropertyType == typeof(EntityReferenceEx))
            {
                var converter = new ValueConverter<EntityReferenceEx, Guid?>(
                    from => from.Id,
                    to => new EntityReferenceEx(to ?? Guid.Empty, ""));
                builder.Property(prop.Name)
                    .HasColumnName(columnName)
                    .HasConversion(converter);
                return true;
            }
            return false;
        }
        public bool ConfigureNotMapped(EntityTypeBuilder<T> builder, PropertyInfo prop)
        {
            var columnName = prop.GetColumnName();
            if (string.IsNullOrWhiteSpace(columnName))
            {
                builder.Ignore(prop.Name);
                return true;
            }
            return false;
        }
        public bool ConfigureId(EntityTypeBuilder<T> builder, PropertyInfo prop)
        {
            if (prop.Name == "Id")
            {
                builder.Ignore(prop.Name);
                return true;
            }
            return false;
        }
        public bool ConfigureMoney(EntityTypeBuilder<T> builder, PropertyInfo prop)
        {
            var columnName = prop.GetColumnName();
            if (!string.IsNullOrEmpty(columnName) && prop.PropertyType == typeof(Money))
            {
                var converter = new ValueConverter<Money, decimal?>(
                   from => from.Value,
                   to => new Money(to ?? 0));
                builder.Property(prop.Name)
                    .HasColumnType("decimal(18,2)")
                    .HasColumnName(columnName)
                    .HasConversion(converter);
                return true;
            }
            return false;
        }
        public bool ConfigureOptionSet(EntityTypeBuilder<T> builder, PropertyInfo prop)
        {
            var columnName = prop.GetColumnName();
            if (!string.IsNullOrEmpty(columnName) && prop.PropertyType == typeof(OptionSetValue))
            {
                var converter = new ValueConverter<OptionSetValue, int?>(
                   from => from.Value,
                   to => to.HasValue ? new OptionSetValue(to.Value) : null);
                builder.Property(prop.Name)
                    .HasColumnType("SMALLINT")
                    .HasColumnName(columnName)
                    .HasConversion(converter);
                return true;
            }
            return false;
        }

        public bool ConfigureColumn(EntityTypeBuilder<T> builder, PropertyInfo prop)
        {
            var known_types = new HashSet<Type>(new Type[] { typeof(Money), typeof(EntityReference), typeof(EntityReferenceEx) });
            var columnName = prop.GetColumnName();
            if (!string.IsNullOrWhiteSpace(columnName)
                && !known_types.Contains(prop.PropertyType))
            {
                builder.Property(prop.Name)
                    .HasColumnName(prop.GetColumnName());

            }
            return false;
        }
        public void Configure_WIthDiscriminators(EntityTypeBuilder<T> builder)
        {
            var schema = this.GetSchema();
            var primaryKey = typeof(T).GetProperties()
                .FirstOrDefault(x => x.Name != "Id" &&
                (x.GetCustomAttribute<AttributeLogicalNameAttribute>()?.LogicalName == schema.PrimaryAttibuteName));
            var type = typeof(T);
            if (1 == 1)
            {
                var f = type.IsDerivedEntity();
                // We dont need object hierarchy 
                // see https://docs.microsoft.com/en-us/ef/core/modeling/inheritance

                //builder.HasBaseType((Type)null);
                if (!f)
                {
                    builder.HasKey(primaryKey.Name);
                    builder.HasBaseType((Type)null);

                    builder.ToView(this.GetViewName());
                    //builder.HasDiscriminator().IsComplete(false);
                    var dis = builder.HasDiscriminator<int?>("StateCode")
                               .HasValue(type, 0);


                    foreach (var t in this.types)
                    {
                        if (t != type)
                        {
                            dis
                                .HasValue(t, 1);

                        }
                    }
                }
                else
                {

                    //.HasValue(type.BaseType, 3);


                }

                //builder.HasNoDiscriminator();
                //builder.HasKey("Id");
            }
            else
            {
                builder.ToTable(this.GetViewName());

                // We dont need object hierarchy 
                // see https://docs.microsoft.com/en-us/ef/core/modeling/inheritance
                builder.HasBaseType((Type)null);
                builder.HasKey(primaryKey.Name);

            }
            var entity = builder;
            var flags = BindingFlags.Public | BindingFlags.Instance;
            if (1 == 1 && type.IsDerivedEntity())
            {
                flags = flags | BindingFlags.DeclaredOnly;
            }
            foreach (var prop in typeof(T).GetProperties(flags))
            {
                var configured =
                    ConfigureNavigation(builder, prop) ||
                    ConfigureReverseNavigation(builder, prop) ||
                    ConfigureNotMapped(builder, prop) ||
                    ConfigureId(builder, prop) ||
                    ConfigureEntityReference(builder, prop) ||
                    ConfigureMoney(builder, prop) ||
                    ConfigureColumn(builder, prop);
            }
        }
        public void Configure(EntityTypeBuilder<T> builder)
        {
            var schema = this.GetSchema();
            if (schema == null)
            {
                throw new Exception(
                    $"Failed to load Schema for this type {typeof(T).Name}");
            }
            var primaryKey = typeof(T).GetProperties()
                .FirstOrDefault(x => x.Name != "Id" &&
                (x.GetCustomAttribute<AttributeLogicalNameAttribute>()?.LogicalName == schema.PrimaryAttibuteName));
            var type = typeof(T);
            builder.ToTable(this.GetViewName());
            // We dont need object hierarchy 
            // see https://docs.microsoft.com/en-us/ef/core/modeling/inheritance
            builder.HasBaseType((Type)null);
            builder.HasKey(primaryKey.Name);


            var entity = builder;
            var flags = BindingFlags.Public | BindingFlags.Instance;
            foreach (var prop in typeof(T).GetProperties(flags))
            {

                var configured =
                    ConfigureOwner(builder, prop) ||
                    ConfigureNavigation(builder, prop) ||
                    ConfigureReverseNavigation(builder, prop) ||
                    ConfigureNotMapped(builder, prop) ||
                    ConfigureId(builder, prop) ||
                    ConfigureEntityReference(builder, prop) ||
                    ConfigureMoney(builder, prop) ||
                    ConfigureOptionSet(builder, prop) ||
                    ConfigureColumn(builder, prop);
            }
        }

        public void Configure_dep(EntityTypeBuilder<T> builder)
        {
            var schema = this.GetSchema();
            var primaryKey = typeof(T).GetProperties()
                .FirstOrDefault(x => x.Name != "Id" &&
                (x.GetCustomAttribute<AttributeLogicalNameAttribute>()?.LogicalName == schema.PrimaryAttibuteName));
            builder.ToTable(this.GetViewName());

            // We dont need object hierarchy 
            // see https://docs.microsoft.com/en-us/ef/core/modeling/inheritance
            builder.HasBaseType((Type)null);
            builder.HasKey(primaryKey.Name);
            //builder.HasKey("Id");
            var entity = builder;
            foreach (var prop in typeof(T).GetProperties())
            {
                var is_navigation_prop = ConfigureNavigation(builder, prop)
                    || ConfigureReverseNavigation(builder, prop);
                var columnName = prop.GetColumnName();
                if (columnName == null)
                {
                    if (!is_navigation_prop)
                        builder.Ignore(prop.Name);
                }
                else if (!is_navigation_prop)
                {
                    if (prop.PropertyType == typeof(EntityReference))
                    {
                        var converter = new ValueConverter<EntityReference, Guid?>(
                           from => from.Id,
                           to => new EntityReference("", to ?? Guid.Empty)
                       );
                        builder.Property(prop.Name)
                            .HasColumnName(columnName)
                            .HasConversion(converter);
                    }
                    else if (prop.PropertyType == typeof(EntityReferenceEx))
                    {
                        var converter = new ValueConverter<EntityReferenceEx, Guid?>(
                            from => from.Id,
                            to => new EntityReferenceEx(to ?? Guid.Empty, "")

                        );
                        builder.Property(prop.Name)
                            .HasColumnName(columnName)
                            .HasConversion(converter);
                    }
                    else if (prop.PropertyType == typeof(Money))
                    {
                        var converter = new ValueConverter<Money, decimal?>(
                           from => from.Value,
                           to => new Money(to ?? 0)
                       );
                        builder.Property(prop.Name)
                            .HasColumnType("decimal(18,2)")
                            .HasColumnName(columnName)
                            .HasConversion(converter);
                    }
                    else if (prop.Name == "Id")
                    {
                        builder.Ignore(prop.Name);
                    }
                    else
                    {
                        builder.Property(prop.Name)
                            .HasColumnName(columnName);


                    }

                }
            }

            if (typeof(XrmContact).IsAssignableFrom(typeof(T)))
            {

            }

        }
    }

}
