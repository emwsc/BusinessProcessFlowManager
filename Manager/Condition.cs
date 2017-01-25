using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessProcessFlowManager.Manager
{
    public class Condition
    {
        public Guid NextStageId;
        public string LogicalFieldName;
        public object ConditionValue;
    }
}
