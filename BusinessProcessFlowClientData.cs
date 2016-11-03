using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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



        public class Steplabels
        {
            public object[] list { get; set; }
        }

        public class Steps
        {
            public List[] list { get; set; }
        }

        public class List
        {
            public string __class { get; set; }
            public string id { get; set; }
            public string description { get; set; }
            public string name { get; set; }
            public Steplabels1 stepLabels { get; set; }
            public Steps1 steps { get; set; }
            public object relationshipName { get; set; }
            public object attributeName { get; set; }
            public bool isClosedLoop { get; set; }
        }

        public class Steplabels1
        {
            public object[] list { get; set; }
        }

        public class Steps1
        {
            public List1[] list { get; set; }
        }

        public class List1
        {
            public string __class { get; set; }
            public string id { get; set; }
            public string description { get; set; }
            public string name { get; set; }
            public Steplabels2 stepLabels { get; set; }
            public Steps2 steps { get; set; }
            public string stageId { get; set; }
            public string nextStageId { get; set; }
            public string stageCategory { get; set; }
        }

        public class Steplabels2
        {
            public List2[] list { get; set; }
        }

        public class List2
        {
            public string labelId { get; set; }
            public int languageCode { get; set; }
            public string description { get; set; }
        }

        public class Steps2
        {
            public List3[] list { get; set; }
        }

        public class List3
        {
            public string __class { get; set; }
            public string id { get; set; }
            public string description { get; set; }
            public object name { get; set; }
            public Steplabels3 stepLabels { get; set; }
            public Steps3 steps { get; set; }
            public string stepStepId { get; set; }
            public bool isProcessRequired { get; set; }
            public bool isHidden { get; set; }
        }

        public class Steplabels3
        {
            public List4[] list { get; set; }
        }

        public class List4
        {
            public string labelId { get; set; }
            public int languageCode { get; set; }
            public string description { get; set; }
        }

        public class Steps3
        {
            public List5[] list { get; set; }
        }

        public class List5
        {
            public string __class { get; set; }
            public string id { get; set; }
            public string description { get; set; }
            public string name { get; set; }
            public Steplabels4 stepLabels { get; set; }
            public string controlId { get; set; }
            public string classId { get; set; }
            public string dataFieldName { get; set; }
            public string systemStepType { get; set; }
            public bool isSystemControl { get; set; }
            public string parameters { get; set; }
            public string controlDisplayName { get; set; }
            public bool isUnbound { get; set; }
            public string controlType { get; set; }
        }

        public class Steplabels4
        {
            public object[] list { get; set; }
        }


    }
}
