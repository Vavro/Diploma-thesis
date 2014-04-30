/// <reference path="../models/dto.ts" />

import commandBase = require("commands/commandBase");

class getQueriesCommand extends commandBase {
    
    execute(): JQueryPromise<queryDto[]> {
        var url = "/queries";
        return this.query<any>(url, null);
    }
}

export = getQueriesCommand;