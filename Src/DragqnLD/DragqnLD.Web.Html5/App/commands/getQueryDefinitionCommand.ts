﻿import commandBase = require("commands/commandBase");
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
        queryResult.fail(xhr => queryDefinitionResult.fail(xhr));
        queryResult.done((result: queryDefinitionDto, status: any, xhr: JQueryXHR) : void => {
            var queryDefinition = new queryDefinition(result);
            queryDefinitionResult.resolve(queryDefinition);
        });

        return queryDefinitionResult;
    }
}

export = getQueryDefinitionCommand;