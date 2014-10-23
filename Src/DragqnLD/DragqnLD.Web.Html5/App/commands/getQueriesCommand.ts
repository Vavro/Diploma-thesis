/// <reference path="../models/dto.ts" />

import commandBase = require("commands/commandBase");

class getQueriesCommand extends commandBase {

    execute(): JQueryPromise<queryDefinitionMetadataDto[]> {
        var url = "/queries";
        return this.query<queryDefinitionMetadataDto[]>(url, null)
            .fail((request : JQueryXHR, status : string, error : string): void => {
                this.notifyError("Failed to get queries definitions", request.responseText, request.statusText);
        });
    }
}

export = getQueriesCommand;