import commandBase = require("commands/commandBase");
import indexDefinition = require("models/indexDefinition");

class getIndexCommand extends commandBase {

    constructor(private definitionId: string, private indexName: string) {
        super();

        if (!definitionId) {
            throw new Error("Must specify ID");
        }
    }

    execute(): JQueryPromise<indexDefinition> {
        var url = "/" + this.definitionId + "/index/" + this.indexName;
        var indexDefinitionsResult = $.Deferred();
        var queryResult = this.query<indexDefinitionDto>(url, null, null);
        queryResult.fail(
            request => {
                this.notifyError("Failed to get index detail " + this.indexName
                    + " for query definition " + this.definitionId, request.responseText, request.statusText);
                indexDefinitionsResult.reject(request);
            });
        queryResult.done((result: indexDefinitionDto, status: any, xhr: JQueryXHR): void => {
            var ids = new indexDefinition(result);
            indexDefinitionsResult.resolve(ids);
        });

        return indexDefinitionsResult;
    }
}

export = getIndexCommand; 