import commandBase = require("commands/commandBase");
import queryDefinitionWithStatus = require("models/queryDefinitionWithStatus");

class getQueryDefinitionCommand extends commandBase {
    constructor(private id: string) {
        super();

        if (!id) {
            throw new Error("Must specify ID");
        }
    }

    execute(): JQueryPromise<any> {
        var queryDefinitionResult = $.Deferred();
        var url = "/" + encodeURIComponent(this.id);
        var queryResult = this.query<queryDefinitionWithStatusDto>(url, null, null);
        queryResult.fail(
            xhr => queryDefinitionResult.reject(xhr));
        queryResult.done((result: queryDefinitionWithStatusDto, status: any, xhr: JQueryXHR): void => {
            var qD = new queryDefinitionWithStatus(result);
            queryDefinitionResult.resolve(qD);
        });

        return queryDefinitionResult;
    }
}

export = getQueryDefinitionCommand; 