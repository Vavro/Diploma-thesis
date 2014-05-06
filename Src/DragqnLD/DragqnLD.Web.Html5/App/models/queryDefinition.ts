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
}

export = queryDefinition;