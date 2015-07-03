import commandBase = require("commands/commandBase");
import queryDefinition = require("models/queryDefinition");

class saveQueryDefinitionCommnad extends commandBase {
    constructor(private newQueryDefinition: queryDefinition) {
        super();
    }

    execute(): JQueryPromise<string> {
        var args = JSON.stringify(this.newQueryDefinition.toDto());
        var url = "/queries";
        var saveTask = this.post(url, args);
        saveTask.done((response: string) : string => {
            this.notifySuccess("Saved " + this.newQueryDefinition.id() + "/n response:" + response);
            return response;
        });
        saveTask.fail((response: JQueryXHR) : void => {
            this.notifyError("Failed to save " + this.newQueryDefinition.id(), response.responseText, response.statusText);
        });

        return saveTask;
    }
}

export = saveQueryDefinitionCommnad;