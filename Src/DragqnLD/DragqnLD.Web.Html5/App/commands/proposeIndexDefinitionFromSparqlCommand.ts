import commandBase = require("commands/commandBase");

import indexDefinition = require("models/indexDefinition");

class proposeIndexDefinitionFromSparqlCommand extends commandBase {

    constructor(private definitionId: string, private sparqlForPropose: string) {
        super();
    }

    public execute(): JQueryPromise<indexDefinition> {
        var url = "/" + this.definitionId + "/proposeIndexForSparql";
        var sparqlDto: sparqlDto = { SparqlSelectQuery: this.sparqlForPropose };
        
        //returns indexDefinitionDto
        var resultAction = $.Deferred();

        var postAction = this.post(url, JSON.stringify(sparqlDto))
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

export = proposeIndexDefinitionFromSparqlCommand;