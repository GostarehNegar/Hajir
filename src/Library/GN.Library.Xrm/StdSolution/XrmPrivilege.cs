using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GN.Library.Xrm.StdSolution
{
    [EntityLogicalName(Schema.LogicalName)]
    public class XrmPrivilege : XrmEntity<XrmPrivilege, DefaultStateCodes, DefaultStatusCodes>
    {
        public new class Schema : XrmEntity.Schema
        {
            public const string LogicalName = "privilege";
            public const string PrivilegeId = LogicalName + "id";
            public const string Name = "name";
            public const string PrevilgeCodes = "prvRead,prvCreate,prvWrite,prvDelete,prvAppendTo,prvAppend,prvAssign,prvShare";

            [Flags]
            public enum Privilelges
            {
                None = 0,
                Read = 1,
                Create = 2,
                Write = 4,
                Delete = 2 ^ 3,
                Append = 2 ^ 4,
                AppendTo = 2 ^ 5,
                Assign = 2 ^ 6,
                Share = 2 ^ 7
            }
        }
        public XrmPrivilege() : base(Schema.LogicalName)
        {

        }

        [AttributeLogicalName(Schema.PrivilegeId)]
        public System.Nullable<System.Guid> PrivilegeId
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.PrivilegeId);
            }
            set
            {
                this.SetAttributeValue(Schema.PrivilegeId, value);
                if (value.HasValue)
                {
                    base.Id = value.Value;
                }
                else
                {
                    base.Id = System.Guid.Empty;
                }
            }
        }



        [AttributeLogicalName(Schema.PrivilegeId)]
        public override System.Guid Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                this.PrivilegeId = value;
            }
        }

        [AttributeLogicalName(Schema.Name)]
        public string Name
        {
            get
            {

                return this.GetAttributeValue<string>(Schema.Name);
            }
            set
            {
                this.SetAttributeValue(Schema.Name, value);
            }
        }

        private Tuple<string, string> Parse(string name)
        {
            var _priviliges = "prvRead,prvCreate,prvWrite,prvDelete,prvAppendTo,prvAppend,prvAssign,prvShare".Split(',');
            if (!string.IsNullOrWhiteSpace(name))
            {
                var prv = _priviliges.FirstOrDefault(x => name.StartsWith(x));
                if (prv != null)
                {
                    return new Tuple<string, string>(prv.Substring(3), name.Substring(prv.Length));
                }
            }
            return new Tuple<string, string>("", "");
        }
        public Schema.Privilelges GetPriviligeCode()
        {
            return Enum.TryParse<Schema.Privilelges>(Parse(this.Name).Item1, out var result)
                ? result
                : Schema.Privilelges.None;
        }
        public string GetRelatedEntityName()
        {
            return this.Parse(this.Name).Item2.ToLowerInvariant();
        }
    }

    public static class PrivilegeExtensions
    {
        public static XrmPrivilege.Schema.Privilelges GetPriviliges(this IEnumerable<XrmPrivilege> values, string entityName)
        {
            var res = XrmPrivilege.Schema.Privilelges.None;
            values.Where(x => x.GetRelatedEntityName() == entityName)
                .Select(x => x.GetPriviligeCode())
                .ToList()
                .ForEach(x => res = res | x);

            var gg = res & XrmPrivilege.Schema.Privilelges.Read;

            return res;


        }

        public static IEnumerable<XrmPrivilege> GetPriviliges(this IXrmRepository<XrmPrivilege> repo, string entityName)
        {

            return repo
                .Queryable
                .Where(x => x.Name.EndsWith(entityName))
                .ToArray()
                .Where(x => x.GetRelatedEntityName() == entityName?.ToLowerInvariant())
                .ToArray();
                //.Select(x => x.GetPriviligeCode())
                //.ToList()
                //.ForEach(x => res = res | x);

        }
    }
}
