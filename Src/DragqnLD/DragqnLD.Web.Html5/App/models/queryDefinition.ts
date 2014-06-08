class queryDefinition {
    id: string;
    name: string;
    description: string;
    constructQueryUriParameterName: string;
    constructQuery: sparqlQueryInfo;
    selectQuery: sparqlQueryInfo;

    constructor(dto?: queryDefinitionDto) {
        if (dto) {
            this.id = dto.id;
            this.name = dto.name;
            this.description = dto.description;
            this.constructQueryUriParameterName = dto.constructQueryUriParameterName;
            this.constructQuery = dto.constructQuery;
            this.selectQuery = dto.selectQuery;
        } else {
            this.id = "";
            this.name = "";
            this.description = "";
            this.constructQueryUriParameterName = "";
            this.constructQuery = { query: "", defaultDataSet: "", sparqlEndpoint: "" };
            this.selectQuery = { query: "", defaultDataSet: "", sparqlEndpoint: "" };
        }
    }

    public static empty(): queryDefinition {

        return new queryDefinition(null);
    }

    public toDto(): queryDefinitionDto {
        return {
            id: this.id,
            name: this.name,
            description: this.description,
            constructQueryUriParameterName: this.constructQueryUriParameterName,
            constructQuery: {
                query: this.constructQuery.query,
                sparqlEndpoint: this.constructQuery.sparqlEndpoint,
                defaultDataSet: this.constructQuery.defaultDataSet
            },
            selectQuery: {
                query: this.selectQuery.query,
                sparqlEndpoint: this.selectQuery.sparqlEndpoint,
                defaultDataSet: this.selectQuery.defaultDataSet
            }
        }
    }
}

export = queryDefinition;