class indexDefinition {
    name = ko.observable<string>().extend({ required: true });
    ravenMap = ko.observable<string>().extend({ required: true });
    

    constructor(dto: indexDefinitionDto) {
        this.name(dto.Name);
        this.ravenMap(dto.RavenMap);
        this.ravenAnalyzers = dto.RavenAnalyzers;
        this.propertyNameMap = dto.PropertyNameMap;
    }

    ravenAnalyzers: any;
    propertyNameMap: any;

    static empty() {
        return new indexDefinition({
            Name : "",
            RavenMap : "",
            PropertyNameMap : {
            },
            RavenAnalyzers : {}
        });
    }

    public toDto(): indexDefinitionDto {
        return {
            Name : this.name(),
            PropertyNameMap : this.propertyNameMap,
            RavenMap : this.ravenMap(),
            RavenAnalyzers : this.ravenAnalyzers
        };
    }
}

export = indexDefinition;