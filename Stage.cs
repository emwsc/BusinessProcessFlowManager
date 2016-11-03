using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessProcessFlowManager
{
    public class Stage
    {
        public List<Step> Steps { get; }
        public Guid StageId;
        public Guid? NextStageId;

        public Stage(Guid stageId)
        {
            StageId = stageId;
            Steps = new List<Step>();
        }

        public Stage(Guid stageId, Guid nextStageId)
        {
            StageId = stageId;
            NextStageId = nextStageId;
            Steps = new List<Step>();
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
