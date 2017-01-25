using System.Runtime.Serialization;

namespace BusinessProcessFlowManager
{
    /// <summary>
    /// Generated from ClientData field of workflow entity
    /// </summary>
    class BusinessProcessFlowClientData
    {
        public string __class { get; set; }
        public string id { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public Steplabels stepLabels { get; set; }
        public Steps steps { get; set; }
        public string primaryEntityName { get; set; }
        public string nextStepIndex { get; set; }
        public bool isCrmUIWorkflow { get; set; }
        public string category { get; set; }
        public string businessProcessType { get; set; }
        public string mode { get; set; }
        public string title { get; set; }
        public string workflowEntityId { get; set; }
        public object formId { get; set; }
        public object[] argumentsArray { get; set; }
        public object[] variables { get; set; }
        public object[] inputs { get; set; }
        public object relationshipName { get; set; }
        public object attributeName { get; set; }
        public bool isClosedLoop { get; set; }
        public string stageId { get; set; }
        public string parentStageId { get; set; }
        public string nextStageId { get; set; }
        public string stageCategory { get; set; }
        public string labelId { get; set; }
        public int languageCode { get; set; }
        public string stepStepId { get; set; }
        public bool isProcessRequired { get; set; }
        public bool isHidden { get; set; }
        public string controlId { get; set; }
        public string classId { get; set; }
        public string dataFieldName { get; set; }
        public string systemStepType { get; set; }
        public bool isSystemControl { get; set; }
        public string parameters { get; set; }
        public string controlDisplayName { get; set; }
        public bool isUnbound { get; set; }
        public string controlType { get; set; }

        public Conditionexpression conditionExpression { get; set; }



        public class Steplabels
        {
            public object[] list { get; set; }
        }

        public class Steps
        {
            [DataMember(Name = "List")]
            public BusinessProcessFlowClientData[] list { get; set; }
        }


        public class Conditionexpression
        {
            public string __class { get; set; }
            public string type { get; set; }
            public bool typeSet { get; set; }
            public int behavior { get; set; }
            public string conditionOperatoroperator { get; set; }
            public Left left { get; set; }
            public Right[] right { get; set; }
        }

        public class Left
        {
            public string __class { get; set; }
            public string type { get; set; }
            public bool typeSet { get; set; }
            public int behavior { get; set; }
            public Entity entity { get; set; }
            public string attributeName { get; set; }
        }

        public class Entity
        {
            public string __class { get; set; }
            public string parameterName { get; set; }
            public string entityName { get; set; }
        }

        public class Right
        {
            public string __class { get; set; }
            public string type { get; set; }
            public bool typeSet { get; set; }
            public int behavior { get; set; }
            public string entityType { get; set; }
            public string label { get; set; }
            public Staticvalue staticValue { get; set; }
        }

        public class Staticvalue
        {
            public string __class { get; set; }
            public string type { get; set; }
            public bool typeSet { get; set; }
            public int behavior { get; set; }
            public string primitiveValue { get; set; }
        }
    }
}
