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
        var url = "/process/" + encodeURIComponent(this.id);
        var queryResult = this.query<any>(url, null, null);
        queryResult.fail(
            xhr => response.reject(xhr));
        queryResult.done((result: any, status: any, xhr: JQueryXHR): void => {
            response.resolve(result);
        });

        return response;
    }
}

export = processQueryCommand; 