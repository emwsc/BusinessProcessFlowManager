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
                var logError = new Entity("kc_log")
                {
                    ["kc_name"] = "MoveWorkflowAction 1"
                };
                service.Create(logError);
                var entityReference = CrmHelper.GetEntityReferenceFromUrl(service, inDynamicURL.Get(executionContext));
                var logError2 = new Entity("kc_log")
                {
                    ["kc_name"] = "MoveWorkflowAction 2"
                };
                service.Create(logError2);
                BusinessProcessFlowManager.InitForEntity(service, entityReference).Move();
                var logError3 = new Entity("kc_log")
                {
                    ["kc_name"] = "MoveWorkflowAction 3"
                };
                service.Create(logError3);
            }
            catch (Exception e)
            {
                var logError=new Entity("kc_log")
                {
                    ["kc_name"]=e.Message,
                    ["kc_errormessage"] = e.StackTrace + Environment.NewLine + e.InnerException?.Message
                };
                service.Create(logError);
                throw new InvalidPluginExecutionException(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException?.Message);
            }
        }
    }
}
