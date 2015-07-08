import commandBase = require("commands/commandBase");

class processQueryCommand extends commandBase {
    constructor(private id: string) {
        super();

        if (!id) {
            throw new Error("Must specify ID");
        }
    }

    execute(): JQueryPromise<any> {
        var response = $.Deferred();
        var url = "/" + encodeURIComponent(this.id) + "/process";
        var queryResult = this.query<any>(url, null, null);
        queryResult.fail((r: JQueryXHR): void => {
            this.notifyError("Failed to launch processing of qurey " + this.id, r.responseText, r.statusText);
        });
        queryResult.done((result: any, status: any, xhr: JQueryXHR): void => {
            response.resolve(result);
        });

        return response;
    }
}

export = processQueryCommand; 