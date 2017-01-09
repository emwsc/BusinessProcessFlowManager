using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;

namespace BusinessProcessFlowManager.Helpers
{
    public static class CrmHelper
    {
        public static EntityReference GetEntityReferenceFromUrl(IOrganizationService service, string url)
        {
            var uri = new Uri(url);
            var queryParameters = uri.Query.TrimStart('?').Split('&');
            int? entityTypeCode = null;
            Guid? entityId = null;
            foreach (var queryParameter in queryParameters)
            {
                var split = queryParameter.Split('=');
                if (split == null || split.Length < 2) continue;
                var attrbiute = split[0];
                var value = split[1];
                if (string.Equals(attrbiute, "etc", StringComparison.InvariantCultureIgnoreCase))
                    entityTypeCode = int.Parse(value);
                if (string.Equals(attrbiute, "id", StringComparison.InvariantCultureIgnoreCase))
                    entityId = Guid.Parse(value);
                if (entityTypeCode.HasValue && entityId.HasValue) break;
            }
            if (!entityId.HasValue || !entityTypeCode.HasValue) return null;
            var entityFilter = new MetadataFilterExpression(LogicalOperator.And);
            entityFilter.Conditions.Add(new MetadataConditionExpression("ObjectTypeCode ", MetadataConditionOperator.Equals, entityTypeCode));
            var propertyExpression = new MetadataPropertiesExpression { AllProperties = false };
            propertyExpression.PropertyNames.Add("LogicalName");
            var entityQueryExpression = new EntityQueryExpression()
            {
                Criteria = entityFilter,
                Properties = propertyExpression
            };

            var retrieveMetadataChangesRequest = new RetrieveMetadataChangesRequest()
            {
                Query = entityQueryExpression
            };

            var response = (RetrieveMetadataChangesResponse)service.Execute(retrieveMetadataChangesRequest);
            return response.EntityMetadata.Any() ? new EntityReference(response.EntityMetadata.SingleOrDefault()?.LogicalName, entityId.Value) : null;
        }
    }
}
