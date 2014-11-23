import commandBase = require("commands/commandBase");

class searchEscapedCommand extends commandBase {

    constructor(private queryId: string, private escapedQueryText: string, private start?: number, private pageSize?: number) {
        super();
    }

    public execute(): JQueryPromise<pagedDocumentMetadataDto> {
        var url = "/" + this.queryId + "/searchEscaped";
        return this.query<pagedDocumentMetadataDto>(url, {escapedQuery: this.escapedQueryText, start: this.start, pageSize: this.pageSize })
            .fail((request: JQueryXHR, status: string, error: string): void => {
                this.notifyError("Failed to search documents for query definition " + this.queryId +
                    " with query text: " + this.escapedQueryText, request.responseText, request.statusText);
            });
    }
}

export = searchEscapedCommand;