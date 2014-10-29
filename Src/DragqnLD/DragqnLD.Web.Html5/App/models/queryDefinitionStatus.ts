import progress = require("models/progress");

class queryDefinitionStatus {
    status = ko.observable<queryStatus>();
    documentLoadProgress = ko.observable<progress>();

    constructor(dto: queryDefinitionStatusDto) {
        this.status(dto.Status);
        this.documentLoadProgress(new progress(dto.DocumentLoadProgress));
    }
}

export = queryDefinitionStatus;