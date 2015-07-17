import commandBase = require("commands/commandBase");
import indexDefinition = require("models/indexDefinition");

class saveIndexDefinitionCommnad extends commandBase {
    constructor(private definitionId : string, private newIndexDefinition: indexDefinition) {
        super();
    }

    execute(): JQueryPromise<string> {
        var args = JSON.stringify(this.newIndexDefinition.toDto());
        var url = "/query/" + this.definitionId + "/index";
        var saveTask = this.post(url, args);
        saveTask.done((response: string): string => {
            this.notifySuccess("Saved " + this.newIndexDefinition.name() + "/n response:" + response);
            return response;
        });
        saveTask.fail((response: JQueryXHR): void => {
            this.notifyError("Failed to save " + this.newIndexDefinition.name(), response.responseText, response.statusText);
        });

        return saveTask;
    }
}

export = saveIndexDefinitionCommnad; 