import commandBase = require("commands/commandBase");
import queryDefinition = require("models/queryDefinition");

class getQueryDefinitionCommand extends commandBase {
    constructor(private id: string) {
        super();

        if (!id) {
                throw new Error("Must specify ID");
        }
    }

    execute(): JQueryPromise<any> {
        var queryDefinitionResult = $.Deferred();
        var url = "/queries/" + encodeURIComponent(this.id);
        var queryResult = this.query<queryDefinitionDto>(url, null, null);
        queryResult.fail(
            xhr => queryDefinitionResult.reject(xhr));
        queryResult.done((result: queryDefinitionDto, status: any, xhr: JQueryXHR) : void => {
            var qD = new queryDefinition(result);
            queryDefinitionResult.resolve(qD);
        });

        return queryDefinitionResult;
    }
}

export = getQueryDefinitionCommand;