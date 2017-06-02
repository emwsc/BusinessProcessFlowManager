using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessProcessFlowManager.Manager
{
    public class Condition
    {

        public enum ConditionOperator
        {
            Equal, NotEqual
        }

        public Guid NextStageId;
        public string LogicalFieldName;
        public object ConditionValue;
        public ConditionOperator Operator;

        public static ConditionOperator SelectOperator(string strOperator)
        {
            return string.Equals(strOperator, "7") ? ConditionOperator.NotEqual : ConditionOperator.Equal;
        }
    }
}
