import sparqlQueryInfo = require('models/sparqlQueryInfo')

class queryDefinition {
    id = ko.observable<string>().extend({ required: true });
    name = ko.observable<string>().extend({ required: true });
    description = ko.observable<string>().extend({ required: true });
    constructQueryUriParameterName = ko.observable<string>().extend({ required: true });
    constructQuery: sparqlQueryInfo;
    selectQuery: sparqlQueryInfo;

    constructor(dto: queryDefinitionDto) {

        this.id(dto.id);
        this.name(dto.name);
        this.description(dto.description);
        this.constructQueryUriParameterName(dto.constructQueryUriParameterName);
        this.constructQuery = new sparqlQueryInfo(dto.constructQuery);
        this.selectQuery = new sparqlQueryInfo(dto.selectQuery);
    }

    public static empty(): queryDefinition {
        return new queryDefinition({
            id: "",
            name: "",
            description: "",
            constructQueryUriParameterName: "",
            constructQuery: {
                query: "",
                defaultDataSet: "",
                sparqlEndpoint: ""
            },
            selectQuery: {
                query: "",
                defaultDataSet: "",
                sparqlEndpoint: ""
            }
        });
    }

    public toDto(): queryDefinitionDto {
        return {
            id: this.id(),
            name: this.name(),
            description: this.description(),
            constructQueryUriParameterName: this.constructQueryUriParameterName(),
            constructQuery: this.constructQuery.toDto(),
            selectQuery: this.selectQuery.toDto()
        }
    }
}

export = queryDefinition;