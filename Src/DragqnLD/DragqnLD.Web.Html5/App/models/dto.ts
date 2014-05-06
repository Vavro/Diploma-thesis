interface queryMetadataDto {
    id: string;
    name: string;
    description: string;
} 

interface queryDefinitionDto extends queryMetadataDto {
    constructQueryUriParameterName: string;
    constructQuery: sparqlQueryInfo;
    selectQuery : sparqlQueryInfo;
}

interface sparqlQueryInfo {
    query : string;
    sparqlEndpoint : string;
    defaultDataSet : string;
}