using System;
using System.Collections.Generic;
using System.Linq;
using BusinessProcessFlowManager.Manager;
using Microsoft.Xrm.Sdk;

namespace BusinessProcessFlowManager
{
    public class Stage
    {

        public List<Step> Steps { get; }
        public Guid StageId;
        public Guid? NextStageId;
        public Guid? ParentStageId;
        public string Description;
        public List<Condition> Conditions;

        public Stage(string description, Guid stageId)
        {
            StageId = stageId;
            Steps = new List<Step>();
            Description = description;
        }

        public Stage(string description, Guid stageId, Guid nextStageId)
        {
            StageId = stageId;
            NextStageId = nextStageId;
            Steps = new List<Step>();
            Description = description;
        }

        public void AddStep(string stepName, string fieldName, bool isRequired)
        {
            var step = new Step() { FieldName = fieldName, IsRequired = isRequired, StepName = stepName };
            AddStep(step);
        }

        public void AddStep(Step step)
        {
            Steps.Add(step);
        }

        public void AddCondtion(Condition condition)
        {
            if (Conditions == null) Conditions = new List<Condition>();
            Conditions.Add(condition);
        }

        public Guid? SelectNextStage(Entity entity)
        {
            foreach (var condition in Conditions)
            {
                if (entity[condition.LogicalFieldName] == null) continue;
                if (entity[condition.LogicalFieldName] is EntityReference && entity.GetAttributeValue<EntityReference>(condition.LogicalFieldName).Id == Guid.Parse(condition.ConditionValue.ToString()))
                    return condition.NextStageId;
                if (entity[condition.LogicalFieldName] == condition.ConditionValue)
                    return condition.NextStageId;
            }
            return null;
        }

        public bool IsLastStage() => NextStageId == null && Conditions == null;
        public bool HasRequiredFields() => Steps.Any(x => x.IsRequired);
        public bool HasConditions() => Conditions != null;
    }
}
