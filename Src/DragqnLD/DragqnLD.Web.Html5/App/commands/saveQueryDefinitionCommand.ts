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
        saveTask.done(() => {
           
            
        });

        saveTask.fail((response: JQueryXHR) => {
            
        });

        return saveTask;
    }
}

export = saveQueryDefinitionCommnad;