class indexDefinitionMetadata {
    definitionId = ko.observable<string>();
    name = ko.observable<string>();
    indexedFields = ko.observable<string>();
    constructor(dto: indexDefinitionMetadataDto) {
        this.name(dto.Name);
        this.indexedFields(dto.IndexedFields.join(", "));
    }

    static dynamic() {
        return new indexDefinitionMetadata({
            Name: "dynamic",
            IndexedFields : [] 
        });
    }
}

export = indexDefinitionMetadata;