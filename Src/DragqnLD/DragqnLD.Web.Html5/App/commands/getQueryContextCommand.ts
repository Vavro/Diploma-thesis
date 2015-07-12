import commandBase = require("commands/commandBase");

class getQueryContext extends commandBase {

    constructor(private definitionId: string) {
        super();
    }

    execute(): JQueryPromise<any> {
        var url = "/" + this.definitionId + "/context";
        return this.query<any>(url, null)
            .fail((request: JQueryXHR, status: string, error: string): void => {
            this.notifyError("Failed to get query context", request.responseText, request.statusText);
        });
    }
}

export = getQueryContext; 