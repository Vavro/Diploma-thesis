class documentMetadata {
    id = ko.observable<string>();
    definitionId = ko.observable<String>();
    constructor(definitionId : string, dto: documentMetadataDto) {
        this.id(dto.Id);
        this.definitionId(definitionId);
    }
}

export = documentMetadata;