import commandBase = require("commands/commandBase");

import propertiesToIndex = require("models/propertiesToIndex");
import checkableProperty = require("models/checkableProperty");
import indexDefinition = require("models/indexDefinition");

class proposeIndexDefinitionFromPropertiesCommand extends commandBase {

    constructor(private definitionId: string, private propertiesToIndex: propertiesToIndex) {
        super();
    }

    public execute(): JQueryPromise<indexDefinition> {
        var url = "/" + this.definitionId + "/proposeIndex";
        var dto = this.propertiesToIndex.toDto();
        
        //returns indexDefinitionDto
        var resultAction = $.Deferred();

        var postAction = this.post(url, JSON.stringify(dto))
            .fail((request: JQueryXHR, status: string, error: string): void => {
                this.notifyError("Failed to propse index for query definition" + this.definitionId,
                    request.responseText, request.statusText);
                resultAction.reject(request);
            })
            .done((result: indexDefinitionDto) => {
                resultAction.resolve(new indexDefinition(result));
            });

        return resultAction;
    }
}

export = proposeIndexDefinitionFromPropertiesCommand;