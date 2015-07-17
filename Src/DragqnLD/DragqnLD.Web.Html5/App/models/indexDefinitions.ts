import indexDefinitionMetadata = require("models/indexDefinitionMetadata");


class indexDefinitions {
    definitionId = ko.observable<String>().extend({ required: true });
    indexes: KnockoutObservableArray<indexDefinitionMetadata>;

    constructor(dto: indexDefinitionsDto) {
        this.definitionId(dto.DefinitionId);
        this.indexes = ko.observableArray(
            dto.Indexes.map(
                (idmDto: indexDefinitionMetadataDto): indexDefinitionMetadata =>
                    new indexDefinitionMetadata(idmDto))
            );
    }
}

export = indexDefinitions; 