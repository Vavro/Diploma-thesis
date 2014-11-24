import commandBase = require("commands/commandBase");

class getDocumentCommand extends commandBase {

    constructor(private definitionId: string, private documentId: string) {
        super();
    }

    public execute(): JQueryPromise<any> {
        var url = "/" + this.definitionId + "/document";
        return this.query<any>(url, { documentId: this.documentId })
            .fail((request: JQueryXHR, status: string, error: string): void => {
                this.notifyError("Failed to get document " + this.documentId + " for query definition " + this.definitionId,
                    request.responseText, request.statusText);

            });
    }
}

export = getDocumentCommand;