using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessProcessFlowManager.Manager
{
    public class StepClasses
    {
        private static readonly string _conditionBranch = "ConditionStep:#Microsoft.Crm.Workflow.ObjectModel";
        private static readonly string _jsStep = "CustomJavascriptStep:#Microsoft.Crm.Workflow.ObjectModel";
        public static bool IsConditionBranch(string stepClassName) => string.Equals(_conditionBranch, stepClassName, StringComparison.InvariantCultureIgnoreCase);
        public static bool IsJSStep(string stepClassName) => string.Equals(_jsStep, stepClassName, StringComparison.InvariantCultureIgnoreCase);
    }
}
