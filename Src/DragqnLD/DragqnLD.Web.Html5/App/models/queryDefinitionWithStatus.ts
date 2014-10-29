import queryDefinition = require("models/queryDefinition");
import queryDefinitionStatus = require("models/queryDefinitionStatus");

class queryDefinitionWithStatus extends queryDefinition {
    status = ko.observable<queryDefinitionStatus>();
    storedDocumentCount = ko.observable<number>();

    constructor(dto: queryDefinitionWithStatusDto) {
        super(dto);
        this.status(new queryDefinitionStatus(dto.Status));
        this.storedDocumentCount(dto.StoredDocumentCount);
    }
}

export = queryDefinitionWithStatus;