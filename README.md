# BusinessProcessFlowManager
This library for Dynamics CRM allows you to move your entity records through stages of Business Process Flows. You don't need to know anything about stages. Library automatically checks all required steps and if all of them completed it will move record to next stage.

# TODO
* Refactore BusinessProcessFlowClientData class because it was basically generated from raw json
* Add branching support
* More useful functions

# Example

```
Entity entity = (Entity)context.InputParameters["Target"];
var stageManager = BusinessProcessFlowManager.BusinessProcessFlowManager.InitForEntity(service, entity);
stageManager.NextStage(entity);
```
