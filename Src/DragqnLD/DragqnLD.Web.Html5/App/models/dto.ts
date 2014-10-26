// ReSharper disable InconsistentNaming
interface queryDefinitionMetadataDto {
    Id: string;
    Name: string;
    Description: string;
}

interface queryDefinitionDto extends queryDefinitionMetadataDto {
    ConstructQueryUriParameterName: string;
    ConstructQuery: sparqlQueryInfoDto;
    SelectQuery : sparqlQueryInfoDto;
}

interface sparqlQueryInfoDto {
    Query : string;
    SparqlEndpoint : string;
    DefaultDataSet : string;
}
// ReSharper restore InconsistentNaming