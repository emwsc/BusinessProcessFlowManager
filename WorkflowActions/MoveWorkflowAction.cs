using System;
using System.Activities;
using BusinessProcessFlowManager.Helpers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace BusinessProcessFlowManager
{
    public class MoveWorkflowAction : CodeActivity
    {
        [RequiredArgument]
        [Input("Dynamic URL")]
        public InArgument<string> inDynamicURL { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracer = executionContext.GetExtension<ITracingService>();
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                var entityReference = CrmHelper.GetEntityReferenceFromUrl(service, inDynamicURL.Get(executionContext));
                BusinessProcessFlowManager.InitForEntity(service, entityReference).Move();
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException?.Message);
            }
        }
    }
}
