import commandBase = require("commands/commandBase");

import indexDefinition = require("models/indexDefinition");

class proposeLuceneQueryFromSparqlCommand extends commandBase {

    constructor(private definitionId: string, private indexName : string, private sparqlForPropose: string) {
        super();
    }

    public execute(): JQueryPromise<string> {
        //todo: !!! alter

        var url = "/" + this.definitionId + "/translate";
        if (this.indexName !== "dynamic") {
            url += "/" + this.indexName;
        }

        var args = { sparqlQuery: this.sparqlForPropose };
        
        //returns indexDefinitionDto
        var resultAction = $.Deferred();

        var query = this.query(url, args)
            .fail((request: JQueryXHR, status: string, error: string): void => {
            this.notifyError("Failed to translate sparql to lucene " + this.sparqlForPropose,
                request.responseText, request.statusText);
            resultAction.reject(request);
        })
            .done((result: string) => {
            resultAction.resolve(result);
        });

        return resultAction;
    }
}

export = proposeLuceneQueryFromSparqlCommand;