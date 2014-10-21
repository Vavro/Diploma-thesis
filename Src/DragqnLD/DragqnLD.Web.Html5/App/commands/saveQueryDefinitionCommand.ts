import commandBase = require("commands/commandBase");
import queryDefinition = require("models/queryDefinition");

class saveQueryDefinitionCommnad extends commandBase {
    constructor(private newQueryDefinition: queryDefinition) {
        super();
    }

    execute(): JQueryPromise<any> {
        var args = JSON.stringify(this.newQueryDefinition.toDto());
        var url = "/queries";
        var saveTask = this.post(url, args);
        saveTask.done(() : void => {
            this.notifySuccess("Saved " + this.newQueryDefinition.id());
        });
        saveTask.fail((response: JQueryXHR) : void => {
            this.notifyError("Failed to save " + this.newQueryDefinition.id(), response.responseText, response.statusText);
        });

        return saveTask;
    }
}

export = saveQueryDefinitionCommnad;