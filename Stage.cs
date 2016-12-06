﻿using System;
using System.Collections.Generic;

namespace BusinessProcessFlowManager
{
    public class Stage
    {
        public List<Step> Steps { get; }
        public Guid StageId;
        public Guid? NextStageId;
        public string Description;

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
            Steps.Add(new Step() { FieldName = fieldName, IsRequired = isRequired, StepName = stepName });
        }

        public void AddStep(Step step)
        {
            Steps.Add(step);
        }
    }
}
