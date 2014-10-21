/// <reference path="../models/dto.ts" />

import commandBase = require("commands/commandBase");

class getQueriesCommand extends commandBase {

    execute(): JQueryPromise<queryDefinitionMetadataDto[]> {
        var url = "/queries";
        return this.query<queryDefinitionMetadataDto[]>(url, null);
    }
}

export = getQueriesCommand;