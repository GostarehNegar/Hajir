using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace GN.Library.Xrm.StdSolution
{
    /// <summary>
    /// 
    /// https://learn.microsoft.com/en-us/dynamics365/customerengagement/on-premises/developer/entities/post?view=op-9-1
    /// </summary>
    [EntityLogicalName(Schema.LogicalName)]
    public class XrmPost : XrmEntity<XrmPost, DefaultStateCodes, DefaultStatusCodes>
    {
        public new class Schema : XrmEntity.Schema
        {
            public const string LogicalName = "post";
            public const string PostId = LogicalName + "id";
            public const string Text = "text";
            public const string RegardingObjectId = "regardingobjectid";
            public const string Post_Comments = "Post_Comments";
        }
        public XrmPost() : base(Schema.LogicalName)
        {

        }
        [AttributeLogicalName(Schema.PostId)]
        public System.Nullable<System.Guid> PostId
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.PostId);
            }
            set
            {
                this.SetAttributeValue(Schema.PostId, value);
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
        [AttributeLogicalName(Schema.PostId)]
        public override System.Guid Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                this.PostId = value;
            }
        }


        [AttributeLogicalName(Schema.Text)]
        public string Text
        {
            get => this.GetAttributeValue<string>(Schema.Text);
            set => this.SetAttribiuteValue(Schema.Text, value);
        }

        [AttributeLogicalName(Schema.RegardingObjectId)]
        public EntityReference RegardingObjectId
        {
            get => this.GetAttributeValue<EntityReference>(Schema.RegardingObjectId);
            set => this.SetAttribiuteValue(Schema.RegardingObjectId, value);
        }
        [RelationshipSchemaName("Post_Comments")]
        public IEnumerable<XrmPostComment> PostComments
        {
            get
            {
                return this.GetRelatedEntities<XrmPostComment>("Post_Comments", null);
            }

        }

    }
    public static class XrmPostExtensions
    {
        public static IEnumerable<T> GetMyPosts<T>(this IXrmRepository<T> repo, Guid userId, DateTime since, int take = 1000, int skip = 0) where T : XrmPost
        {
            return repo
                .Queryable
                .Where(x => x.RegardingObjectId.Id == userId && x.ModifiedOn > since)
                .Skip(skip)
                .Take(take)
                .ToArray();
        }
        public static EntityReference[] ExtractEntitiesFromPostText(this XrmPost post)
        {
            return ExtractEntitiesFromPostText(post.Text ?? "");
        }
        public static EntityReference[] ExtractEntitiesFromPostText(string text)
        {
            try
            {
                return new Regex(@"@\[(.*?)\]")
                    .Matches(text)
                    .OfType<Match>()
                    .Where(x => x.Success)
                    .Select(x => x.Value.Replace("@[", "").Replace("]", ""))
                    .Select(x => x.Split(','))
                    .Where(x => x.Length == 3)
                    .Select(x => new EntityReference
                    {
                        LogicalName = int.TryParse(x[0], out var _i) ? Entities.GetNameByCode(_i) : null,
                        Id = Guid.TryParse(x[1], out var _id) ? _id : Guid.Empty,
                        Name = x[2].Replace("\"", "")
                    })
                    .Where(x => !string.IsNullOrWhiteSpace(x.LogicalName) && x.Id != Guid.Empty)
                    .ToArray();

            }
            catch { }
            return new EntityReference[] { };

        }
    }
}
