using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using BusinessProcessFlowManager.Manager;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace BusinessProcessFlowManager
{
    public class BusinessProcessFlowManager
    {

        public enum MoveState
        {
            None,
            Success,
            LastStage,
            RequiredFieldIsEmpty
        }

        private IOrganizationService _service;
        private Entity _process;
        private BusinessProcessFlowClientData _businessProcessFlowClientData;
        private Entity _targetEntity;

        public string ProcessName => _process.GetAttributeValue<string>("name");

        public Guid ProcessId => _process.Id;

        public Dictionary<Guid, Stage> Stages { get; private set; }

        private BusinessProcessFlowManager()
        {
            Stages = new Dictionary<Guid, Stage>();
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
            var manager = new BusinessProcessFlowManager { _service = service };
            var entity = service.Retrieve(entityLogicalName, entityId, new ColumnSet("processid"));
            manager._process = manager.RetrieveProcessById(entity.GetAttributeValue<Guid>("processid"));
            manager.Init(entityLogicalName, entityId);
            return manager;
        }

        public static BusinessProcessFlowManager InitByProcessName(IOrganizationService service, string processName)
        {
            var manager = new BusinessProcessFlowManager { _service = service };
            manager._process = manager.RetrieveProcessByName(processName);
            manager.Init();
            return manager;
        }

        public static BusinessProcessFlowManager InitByProcessId(IOrganizationService service, Guid processId)
        {
            var manager = new BusinessProcessFlowManager { _service = service };
            manager._process = manager.RetrieveProcessById(processId);
            manager.Init();
            return manager;
        }

        private void Init(string entityLogicalName, Guid entityId)
        {
            _targetEntity = new Entity(entityLogicalName) { Id = entityId };
            Init();
        }

        private void Init()
        {
            _businessProcessFlowClientData = ParseBusinessProcessFlow();
            Convert();
        }

        private void Convert()
        {
            foreach (var item in _businessProcessFlowClientData.steps.list)
            {
                if (!item.steps.list.Any()) continue;
                var stageItem = item.steps.list.Single();
                var stage = string.IsNullOrWhiteSpace(stageItem.nextStageId) ? new Stage(stageItem.description, Guid.Parse(stageItem.stageId)) : new Stage(stageItem.description, Guid.Parse(stageItem.stageId), Guid.Parse(stageItem.nextStageId));
                foreach (var stepItem in stageItem.steps.list)
                {
                    if (StepClasses.IsJSStep(stepItem.__class)) continue;
                    if (StepClasses.IsConditionBranch(stepItem.__class))
                    {
                        for (var i = 0; i < stepItem.steps.list.Length; i++)
                        {
                            var stepWithCondition = stepItem.steps.list[i];
                            if (stepWithCondition.conditionExpression == null)
                            {
                                stage.NextStageId = Guid.Parse(stepWithCondition.steps.list.Single().stageId);
                                continue;
                            }
                            var conditions = stepWithCondition.conditionExpression.right;
                            foreach (var condition in conditions)
                            {
                                stage.AddCondtion(new Condition
                                {
                                    LogicalFieldName = stepWithCondition.conditionExpression.left.attributeName,
                                    ConditionValue = condition.staticValue.primitiveValue,
                                    NextStageId = Guid.Parse(stepWithCondition.steps.list.Single().stageId)
                                });
                            }
                        }
                    }
                    else
                    {
                        var step = new Step()
                        {
                            IsRequired = stepItem.isProcessRequired,
                            FieldName = stepItem.steps.list.Single().dataFieldName,
                            StepName = stepItem.steps.list.Single().controlDisplayName
                        };
                        stage.AddStep(step);
                    }
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

        /// <summary>
        /// Move record to next stage. This functions will check required fields on current stage before move to next. Works only if manager enabled from entity.
        /// </summary>
        /// <returns>Move state enum</returns>
        public MoveState NextStage()
        {
            if (_targetEntity == null) throw new Exception("Target entity is undefined");
            return NextStage(_targetEntity);
        }

        /// <summary>
        /// Move record to next stage. This functions will check required fields on current stage before move to next.
        /// </summary>
        /// <param name="entity">Entity record</param>
        /// <returns>Move state enum</returns>
        public MoveState NextStage(Entity entity)
        {
            return NextStage(entity.Id, entity.LogicalName);
        }

        /// <summary>
        /// Move record to next stage. This functions will check required fields on current stage before move to next.
        /// </summary>
        /// <param name="entityReference">Entity reference</param>
        /// <returns>Move state enum</returns>
        public MoveState NextStage(EntityReference entityReference)
        {
            return NextStage(entityReference.Id, entityReference.LogicalName);
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

        /// <summary>
        /// Move record to next stage. This functions will check required fields on current stage before move to next.
        /// </summary>
        /// <param name="entityId">Record id</param>
        /// <param name="entityLogicalName">Entity logical name</param>
        /// <returns>Move state enum</returns>
        public MoveState NextStage(Guid entityId, string entityLogicalName)
        {
            var entity = _service.Retrieve(entityLogicalName, entityId, new ColumnSet("stageid", "processid", "traversedpath"));
            var currentPath = entity.GetAttributeValue<string>("traversedpath");
            if (entity.GetAttributeValue<Guid>("processid") != _process.Id) throw new Exception("Wrong process");
            var currentStage = Stages[entity.GetAttributeValue<Guid>("stageid")];
            if (currentStage.IsLastStage()) return MoveState.LastStage;
            if (!currentStage.HasRequiredFields())
            {
                NextStage(entity, currentStage, entity.GetAttributeValue<string>("traversedpath"));
                return MoveState.Success;
            }
            entity = _service.Retrieve(entityLogicalName, entityId, new ColumnSet(currentStage.Steps.Where(x => x.IsRequired).Select(x => x.FieldName).ToArray()));
            if (!AllowMove(currentStage, entity)) return MoveState.RequiredFieldIsEmpty;
            NextStage(entity, currentStage, currentPath);
            return MoveState.Success;
        }

        private void NextStage(Entity currentEntity, Stage stage, string currentTraversedPath)
        {
            Guid? nextStageId = null;
            nextStageId = stage.HasConditions() ? (stage.SelectNextStage(currentEntity) ?? stage.NextStageId) : stage.NextStageId;
            if (!nextStageId.HasValue) return;
            var entity = new Entity(currentEntity.LogicalName)
            {
                Id = currentEntity.Id,
                ["stageid"] = nextStageId.Value,
                ["processid"] = _process.Id,
                ["traversedpath"] = currentTraversedPath + $",{nextStageId.Value}"
            };
            _service.Update(entity);
        }


        /// <summary>
        /// Move to selected stage. This functions ignors all required fields. Works only if manager enabled from entity.
        /// </summary>
        /// <param name="stage">Selected stage</param>
        /// <returns>Move state enum</returns>
        public MoveState MoveTo(Stage stage)
        {
            if (_targetEntity == null) throw new Exception("Target entity is undefined");
            return MoveTo(_targetEntity.Id, _targetEntity.LogicalName, stage);
        }

        /// <summary>
        /// Move to selected stage. This functions ignors all required fields
        /// </summary>
        /// <param name="entityReference">Entity reference</param>
        /// <param name="stage">Selected stage</param>
        /// <returns>Move state enum</returns>
        public MoveState MoveTo(EntityReference entityReference, Stage stage)
        {
            return MoveTo(entityReference.Id, entityReference.LogicalName, stage);
        }

        /// <summary>
        /// Move to selected stage. This functions ignors all required fields
        /// </summary>
        /// <param name="entity">Entity record</param>
        /// <param name="stage">Selected stage</param>
        /// <returns>Move state enum</returns>
        public MoveState MoveTo(Entity entity, Stage stage)
        {
            return MoveTo(entity.Id, entity.LogicalName, stage);
        }

        /// <summary>
        /// Move to selected stage. This functions ignors all required fields
        /// </summary>
        /// <param name="entityId">Record id</param>
        /// <param name="entityLogicalName">Entity logical name</param>
        /// <param name="stage">Selected stage</param>
        /// <returns>Move state enum</returns>
        public MoveState MoveTo(Guid entityId, string entityLogicalName, Stage stage)
        {
            var currentTraversedPath = string.Join(",", Stages.Values.TakeWhile(x => x.StageId != stage.StageId));
            var entity = new Entity(entityLogicalName)
            {
                Id = entityId,
                ["stageid"] = stage,
                ["processid"] = _process.Id,
                ["traversedpath"] = currentTraversedPath + $",{stage.StageId}"
            };
            return MoveState.Success;
        }

        /// <summary>
        /// Moving record until it's possible. Only working if manager was enabled from entity.
        /// </summary>
        /// <returns>Move state enum</returns>
        public MoveState Move()
        {
            if (_targetEntity == null) throw new Exception("Target entity is undefined");
            return Move(_targetEntity);

        }

        /// <summary>
        /// Moving record until it's possible.
        /// </summary>
        /// <param name="entityReference">Entity reference</param>
        /// <returns>Move state enum</returns>
        public MoveState Move(EntityReference entityReference)
        {
            return Move(entityReference.Id, entityReference.LogicalName);
        }

        /// <summary>
        /// Moving record until it's possible.
        /// </summary>
        /// <param name="entity">Entity record</param>
        /// <returns>Move state enum</returns>
        public MoveState Move(Entity entity)
        {
            return Move(entity.Id, entity.LogicalName);
        }

        /// <summary>
        /// Moving record until it's possible.
        /// </summary>
        /// <param name="entityId">Record Id</param>
        /// <param name="entityLogicalName">Entity Logical Name</param>
        /// <returns>Move state enum</returns>
        public MoveState Move(Guid entityId, string entityLogicalName)
        {
            var moveState = MoveState.None;
            while (moveState != MoveState.RequiredFieldIsEmpty && moveState != MoveState.LastStage)
                moveState = NextStage(entityId, entityLogicalName);
            return moveState;
        }
    }
}
