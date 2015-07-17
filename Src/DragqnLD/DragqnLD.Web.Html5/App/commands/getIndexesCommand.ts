import commandBase = require("commands/commandBase");
import indexDefinitions = require("models/indexDefinitions");

class getIndexesCommand extends commandBase {

    constructor(private definitionId: string) {
        super();

        if (!definitionId) {
            throw new Error("Must specify ID");
        }
    }

    execute(): JQueryPromise<indexDefinitions> {
        var url = "/" + this.definitionId + "/indexes";
        var indexDefinitionsResult = $.Deferred();
        var queryResult = this.query<indexDefinitionsDto>(url, null, null);
        queryResult.fail(
            request => {
                this.notifyError("Failed to get indexes for query definition " + this.definitionId, request.responseText, request.statusText);
                indexDefinitionsResult.reject(request);
            });
        queryResult.done((result: indexDefinitionsDto, status: any, xhr: JQueryXHR): void => {
            var ids = new indexDefinitions(result);
            indexDefinitionsResult.resolve(ids);
        });

        return indexDefinitionsResult;
    }
}

export = getIndexesCommand; 