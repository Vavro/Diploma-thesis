import commandBase = require("commands/commandBase");
import indexableProperties = require("models/indexableProperties");

class getQueryIndexablePropertiesCommand extends commandBase {

    constructor(private definitionId: string) {
        super();

        if (!definitionId) {
            throw new Error("Must specify ID");
        }
    }

    execute(): JQueryPromise<indexableProperties> {
        var url = "/" + this.definitionId + "/indexableProperties";
        var indexDefinitionsResult = $.Deferred();
        var queryResult = this.query<indexablePropertiesDto>(url, null, null);
        queryResult.fail(
            request => {
                this.notifyError("Failed to get indexes for query definition " + this.definitionId, request.responseText, request.statusText);
                indexDefinitionsResult.reject(request);
            });
        queryResult.done((result: indexablePropertiesDto, status: any, xhr: JQueryXHR): void => {
            var ids = new indexableProperties(result);
            indexDefinitionsResult.resolve(ids);
        });

        return indexDefinitionsResult;
    }
}

export = getQueryIndexablePropertiesCommand;  