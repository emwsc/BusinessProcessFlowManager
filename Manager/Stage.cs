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


        private bool CheckCondition(Condition condition, Entity entity)
        {
            bool stageSelected = true;
            if (entity[condition.LogicalFieldName] == null)
            {
                stageSelected = false;
            }
            else if (entity[condition.LogicalFieldName] is EntityReference)
            {
                if (condition.Operator == Condition.ConditionOperator.Equal && !(entity.GetAttributeValue<EntityReference>(condition.LogicalFieldName).Id == Guid.Parse(condition.ConditionValue.ToString())))
                    stageSelected = false;
                else if (condition.Operator == Condition.ConditionOperator.NotEqual && (entity.GetAttributeValue<EntityReference>(condition.LogicalFieldName).Id == Guid.Parse(condition.ConditionValue.ToString())))
                    stageSelected = false;
            }
            else if (condition.Operator == Condition.ConditionOperator.Equal && !entity[condition.LogicalFieldName].Equals(condition.ConditionValue))
                stageSelected = false;
            else if (condition.Operator == Condition.ConditionOperator.NotEqual && entity[condition.LogicalFieldName].Equals(condition.ConditionValue))
                stageSelected = false;
            return stageSelected;
        }

        public Guid? SelectNextStage(Entity entity)
        {
            var conditionGroupsByStage = Conditions.GroupBy(x => x.NextStageId, x => x);
            Guid? nextStageId = null;
            foreach (var group in conditionGroupsByStage)
            {
                bool stageSelected = true;
                foreach (var condition in group)
                {
                    stageSelected = CheckCondition(condition, entity);
                    if(!stageSelected) break;
                }
                if (!stageSelected) continue;
                nextStageId = @group.Key;
                break;
            }
            if (!nextStageId.HasValue && NextStageId.HasValue)
                nextStageId = NextStageId;
            return nextStageId;
        }

        public bool IsLastStage() => NextStageId == null && Conditions == null;
        public bool HasRequiredFields() => Steps.Any(x => x.IsRequired);
        public bool HasConditions() => Conditions != null;
    }
}
