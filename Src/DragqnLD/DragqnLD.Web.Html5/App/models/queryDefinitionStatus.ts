import progress = require("models/progress");

class queryDefinitionStatus {
    status = ko.observable<QueryStatus>();
    documentLoadProgress = ko.observable<progress>();

    statusText: KnockoutComputed<string>;

    constructor(dto: queryDefinitionStatusDto) {
        this.status(dto.Status);
        this.documentLoadProgress(new progress(dto.DocumentLoadProgress));

        this.statusText = ko.computed((): string => {
            var s = this.status();

            switch (s) {
                case QueryStatus.ReadyToRun:
                    return "Ready to be run";
                case QueryStatus.LoadingSelectResult:
                    return "Loading select results";
                case QueryStatus.LoadingDocuments:
                    return "Loading documents: " + this.documentLoadProgress().currentItem() + "/" + this.documentLoadProgress().totalCount();
                case QueryStatus.Loaded:
                    return "Documents loaded";
                default:
                    return "Unknown";
            }
        });
    }


}

export = queryDefinitionStatus;