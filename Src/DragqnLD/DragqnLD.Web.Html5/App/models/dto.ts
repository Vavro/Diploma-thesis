// ReSharper disable InconsistentNaming
interface dictionary<TValue> {
    [key: string]: TValue;
}

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
    Status: QueryStatus;
    DocumentLoadProgress: progressDto;
}

interface progressDto {
    TotalCount: number;
    CurrentItem: number;
}

enum QueryStatus {
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

// todo: when i get rid of Content delete this Dto
interface documentDto {
    Content : any;
}

interface indexDefinitionDto {
    Name: string;
    RavenMap: string;
    RavenAnalyzers: any;
    PropertyNameMap: any;
}

interface propertiesToIndexDto {
    PropertyPaths: string[];
}

interface indexDefinitionsDto {
    DefinitionId: string;
    Indexes : indexDefinitionMetadataDto[];
}

interface indexDefinitionMetadataDto {
    Name: string;
    IndexedFields: string[];
}

interface sparqlDto {
    SparqlSelectQuery: string;
}

interface indexablePropertiesDto {
    Properties: string[];
}
// ReSharper restore InconsistentNaming