class appUrl {
    static baseUrl: string = "http://localhost:2429/api";

    public static viewDocumentUrl(definitionId : string, documentId : string) : string {
        return "#viewDocument?definitionId=" + definitionId + "&documentId=" + documentId;
    }
}

export = appUrl;