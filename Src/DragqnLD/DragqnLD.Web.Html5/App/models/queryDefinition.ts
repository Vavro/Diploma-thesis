import sparqlQueryInfo = require("models/sparqlQueryInfo");

class queryDefinition {
    id = ko.observable<string>().extend({ required: true });
    name = ko.observable<string>().extend({ required: true });
    description = ko.observable<string>().extend({ required: true });
    constructQueryUriParameterName = ko.observable<string>().extend({ required: true });
    constructQuery: sparqlQueryInfo;
    selectQuery: sparqlQueryInfo;

    constructor(dto: queryDefinitionDto) {

        this.id(dto.Id);
        this.name(dto.Name);
        this.description(dto.Description);
        this.constructQueryUriParameterName(dto.ConstructQueryUriParameterName);
        this.constructQuery = new sparqlQueryInfo(dto.ConstructQuery);
        this.selectQuery = new sparqlQueryInfo(dto.SelectQuery);
    }

    public static empty(): queryDefinition {
        return new queryDefinition({
            Id: "",
            Name: "",
            Description: "",
            ConstructQueryUriParameterName: "",
            ConstructQuery: {
                Query: "",
                DefaultDataSet: "",
                SparqlEndpoint: ""
            },
            SelectQuery: {
                Query: "",
                DefaultDataSet: "",
                SparqlEndpoint: ""
            }
        });
    }

    public toDto(): queryDefinitionDto {
        return {
            Id: this.id(),
            Name: this.name(),
            Description: this.description(),
            ConstructQueryUriParameterName: this.constructQueryUriParameterName(),
            ConstructQuery: this.constructQuery.toDto(),
            SelectQuery: this.selectQuery.toDto()
        };
    }
}

export = queryDefinition;