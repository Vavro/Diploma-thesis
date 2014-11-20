class documentMetadata {
    id = ko.observable<string>();
    constructor(dto: documentMetadataDto) {
        this.id(dto.Id);
    }
}

export = documentMetadata;