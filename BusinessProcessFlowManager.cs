using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace BusinessProcessFlowManager
{
    public class BusinessProcessFlowManager
    {

        public enum MoveState
        {
            Success,
            RequiredFieldIsEmpty
        }

        private IOrganizationService _service;
        private Entity _process;
        private BusinessProcessFlowClientData _businessProcessFlowClientData;

        public string ProcessName
        {
            get { return _process.GetAttributeValue<string>("name"); }
        }

        public Guid ProcessId
        {
            get { return _process.Id; }
        }

        public Dictionary<Guid, Stage> Stages { get; private set; }

        private BusinessProcessFlowManager()
        { 

        }

        public static BusinessProcessFlowManager InitForEntity(IOrganizationService service, EntityReference entityReference)
        {
            return InitForEntity(service, entityReference.Id, entityReference.LogicalName);
        }

        public static BusinessProcessFlowManager InitForEntity(IOrganizationService service, Entity entity)
        {
            return InitForEntity(service, entity.Id, entity.LogicalName);
        }

        public static BusinessProcessFlowManager InitForEntity(IOrganizationService service, Guid entityId, string entityLogicalName)
        {
            var manager = new BusinessProcessFlowManager();
            manager._service = service;
            var entity = service.Retrieve(entityLogicalName, entityId, new ColumnSet("processid"));
            manager._process = manager.RetrieveProcessById(entity.GetAttributeValue<Guid>("processid"));
            manager.Init();
            return manager;
        }

        public static BusinessProcessFlowManager InitByProcessName(IOrganizationService service, string processName)
        {
            var manager = new BusinessProcessFlowManager();
            manager._service = service;
            manager._process = manager.RetrieveProcessByName(processName);
            manager.Init();
            return manager;
        }

        public static BusinessProcessFlowManager InitByProcessId(IOrganizationService service, Guid processId)
        {
            var manager = new BusinessProcessFlowManager();
            manager._service = service;
            manager._process = manager.RetrieveProcessById(processId);
            manager.Init();
            return manager;
        }

        private void Init()
        {
            _businessProcessFlowClientData = ParseBusinessProcessFlow();
            Stages = new Dictionary<Guid, Stage>();
            Convert();
        }

        private void Convert()
        {
            foreach (var item in _businessProcessFlowClientData.steps.list)
            {
                if (!item.steps.list.Any()) continue;
                var stageItem = item.steps.list.Single();
                var stage = string.IsNullOrWhiteSpace(stageItem.nextStageId) ? new Stage(Guid.Parse(stageItem.stageId)) : new Stage(Guid.Parse(stageItem.stageId), Guid.Parse(stageItem.nextStageId));
                foreach (var stepItem in stageItem.steps.list)
                {
                    var step = new Step()
                    {
                        IsRequired = stepItem.isProcessRequired,
                        FieldName = stepItem.steps.list.Single().dataFieldName,
                        StepName = stepItem.steps.list.Single().controlDisplayName
                    };
                    stage.AddStep(step);
                }
                Stages.Add(stage.StageId, stage);
            }
        }

        private BusinessProcessFlowClientData ParseBusinessProcessFlow()
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            var result = jsonSerializer.Deserialize<BusinessProcessFlowClientData>(_process.GetAttributeValue<string>("clientdata"));
            return result;
        }

        private Entity RetrieveProcessById(Guid processId)
        {
            return _service.Retrieve("workflow", processId, new ColumnSet("name", "clientdata"));
        }

        private Entity RetrieveProcessByName(string processName)
        {
            var query = new QueryExpression("workflow") { ColumnSet = new ColumnSet("name", "clientdata") };
            query.Criteria.AddCondition("name", ConditionOperator.Equal, processName);
            var process = _service.RetrieveMultiple(query).Entities.SingleOrDefault();
            if (process == null) throw new Exception("Process not found");
            return process;
        }


        public void NextStage(Entity entity)
        {
            NextStage(entity.Id, entity.LogicalName);
        }

        public void NextStage(EntityReference entityReference)
        {
            NextStage(entityReference.Id, entityReference.LogicalName);
        }


        private bool AllowMove(Stage stage, Entity entity)
        {
            foreach (var step in stage.Steps)
            {
                if (!entity.Contains(step.FieldName) || entity[step.FieldName] == null) return false;
                if (entity.Contains(step.FieldName) && entity[step.FieldName] is bool && !entity.GetAttributeValue<bool>(step.FieldName)) return false;
            }
            return true;
        }

        public MoveState NextStage(Guid entityId, string entityLogicalName)
        {
            var entity = _service.Retrieve(entityLogicalName, entityId, new ColumnSet("stageid", "processid", "traversedpath"));
            var currentPath = entity.GetAttributeValue<string>("traversedpath");
            if (entity.GetAttributeValue<Guid>("processid") != _process.Id) throw new Exception("Wrong process");
            var currentStage = Stages[entity.GetAttributeValue<Guid>("stageid")];
            if (!currentStage.Steps.Any(x => x.IsRequired)) MoveTo(entityId, entityLogicalName, currentStage, entity.GetAttributeValue<string>("traversedpath"));
            entity = _service.Retrieve(entityLogicalName, entityId, new ColumnSet(currentStage.Steps.Where(x => x.IsRequired).Select(x => x.FieldName).ToArray()));
            if (!AllowMove(currentStage, entity)) return MoveState.RequiredFieldIsEmpty;
            MoveTo(entityId, entityLogicalName, currentStage, currentPath);
            return MoveState.Success;
        }

        public void MoveTo(Guid entityId, string entityLogicalName, Stage stage, string currentTraversedPath)
        {
            if (!stage.NextStageId.HasValue) return;
            var entity = new Entity(entityLogicalName) { Id = entityId };
            entity["stageid"] = stage.NextStageId.Value;
            entity["processid"] = _process.Id;
            entity["traversedpath"] = currentTraversedPath + $",{stage.NextStageId.Value}";
            _service.Update(entity);
        }
    }
}
