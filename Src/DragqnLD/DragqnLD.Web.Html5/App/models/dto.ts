interface queryMetadataDto {
    id: string;
    name: string;
    description: string;
} 

interface queryDefinitionDto extends queryMetadataDto {
    constructQueryUriParameterName: string;
    constructQuery: sparqlQueryInfoDto;
    selectQuery : sparqlQueryInfoDto;
}

interface sparqlQueryInfoDto {
    query : string;
    sparqlEndpoint : string;
    defaultDataSet : string;
}