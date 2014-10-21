interface queryDefinitionMetadataDto {
    id: string;
    name: string;
    description: string;
}

interface queryDefinitionDto extends queryDefinitionMetadataDto {
    constructQueryUriParameterName: string;
    constructQuery: sparqlQueryInfoDto;
    selectQuery : sparqlQueryInfoDto;
}

interface sparqlQueryInfoDto {
    query : string;
    sparqlEndpoint : string;
    defaultDataSet : string;
}