// ReSharper disable InconsistentNaming
interface queryDefinitionMetadataDto {
    Id: string;
    Name: string;
    Description: string;
}

interface queryDefinitionDto extends queryDefinitionMetadataDto {
    ConstructQueryUriParameterName: string;
    ConstructQuery: sparqlQueryInfoDto;
    SelectQuery: sparqlQueryInfoDto;
}

interface queryDefinitionWithStatusDto extends queryDefinitionDto {
    Status: queryDefinitionStatusDto;
    StoredDocumentCount : number;
}

interface queryDefinitionStatusDto {
    Status: queryStatus;
    DocumentLoadProgress: progressDto;
}

interface progressDto {
    TotalCount: number;
    CurrentItem: number;
}

enum queryStatus {
    ReadyToRun = 0,
    LoadingSelectResult = 1,
    LoadingDocuments = 2,
    Loaded = 3
}

interface sparqlQueryInfoDto {
    Query: string;
    SparqlEndpoint: string;
    DefaultDataSet: string;
}

interface documentMetadataDto {
    Id : string;
}

interface pagedDocumentMetadataDto {
    Items: documentMetadataDto[];
    TotalItems: number;
}
// ReSharper restore InconsistentNaming