class indexDefinitionMetadata {
    definitionId = ko.observable<String>();
    name = ko.observable<String>();
    indexedFields = ko.observable<string>();
    constructor(dto: indexDefinitionMetadataDto) {
        this.name(dto.Name);
        this.indexedFields(dto.IndexedFields.join(", "));
    }
}

export = indexDefinitionMetadata;