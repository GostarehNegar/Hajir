﻿using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace SS.Crm.Linq.Proxies
{
	class OrganizationServiceProxy : IOrganizationService
	{
		public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
		{
			throw new NotImplementedException();
		}

		public Guid Create(Entity entity)
		{
			throw new NotImplementedException();
		}

		public void Delete(string entityName, Guid id)
		{
			throw new NotImplementedException();
		}

		public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
		{
			throw new NotImplementedException();
		}

		public OrganizationResponse Execute(OrganizationRequest request)
		{
			throw new NotImplementedException();
		}

		public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
		{
			throw new NotImplementedException();
		}

		public EntityCollection RetrieveMultiple(QueryBase query)
		{
			throw new NotImplementedException();
		}

		public void Update(Entity entity)
		{
			throw new NotImplementedException();
		}
	}
}
