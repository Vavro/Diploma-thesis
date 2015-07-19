import indexedProperty = require("models/indexedProperty");

class indexDefinition {
    name = ko.observable<string>().extend({ required: true });
    ravenMap = ko.observable<string>().extend({ required: true });
    

    constructor(dto: indexDefinitionDto) {
        this.name(dto.Name);
        this.ravenMap(dto.RavenMap);

        for (var propertyName in dto.PropertyNameMap) {
            var prop = new indexedProperty();
            prop.propertyPath(propertyName);
            var mappedName = dto.PropertyNameMap[propertyName];
            prop.ravenMapName(mappedName);
            prop.analyzerName(dto.RavenAnalyzers[mappedName]);
            this.fields.push(prop);
        }
    }

    public fields = ko.observableArray<indexedProperty>();
    
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
        var propNameMap: any = new Object();
        var ravenAnalyzers: any = new Object();
        
        //todo convert fields back to those two objects
        this.fields().forEach((field) => {
            var propName = field.propertyPath();
            var fieldName = field.ravenMapName();
            propNameMap[propName] = fieldName;

            var analyzerName = field.analyzerName();
            if (analyzerName) {
                ravenAnalyzers[fieldName] = analyzerName;
            }
        });
        
        return {
            Name : this.name(),
            PropertyNameMap : propNameMap,
            RavenMap : this.ravenMap(),
            RavenAnalyzers : ravenAnalyzers
        };
    }

    public setFrom(indexDefinition: indexDefinition) {
        this.name(indexDefinition.name());
        this.ravenMap(indexDefinition.ravenMap());
        this.fields.removeAll();
        indexDefinition.fields().forEach((field) => { this.fields.push(field); });
    }
}

export = indexDefinition;