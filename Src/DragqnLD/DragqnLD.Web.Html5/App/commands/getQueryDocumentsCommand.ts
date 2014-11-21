import commandBase = require("commands/commandBase");

class getQueryDocumentsCommand extends commandBase {
    constructor(private queryId: string, private start: number, private pageSize : number) {
        super();
    }

    public execute(): JQueryPromise<pagedDocumentMetadataDto> {
        //todo: think up a better way to create these urls
        var url = "/" + this.queryId + "/documents";
        return this.query<pagedDocumentMetadataDto>(url, { start: this.start, pageSize: this.pageSize })
            .fail((request: JQueryXHR, status: string, error: string): void => {
                this.notifyError("Failed to get documents for query definition " + this.queryId, request.responseText, request.statusText);
            });
    }
}

export = getQueryDocumentsCommand;