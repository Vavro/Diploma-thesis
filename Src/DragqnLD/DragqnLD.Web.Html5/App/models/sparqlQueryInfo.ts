class sparqlQueryInfo {
    query = ko.observable<string>().extend({required : true});
    sparqlEndpoint = ko.observable<string>().extend({required : true});
    defaultDataSet = ko.observable<string>().extend({required : true});

    constructor(dto: sparqlQueryInfoDto) {
        this.query(dto.Query);
        this.sparqlEndpoint(dto.SparqlEndpoint);
        this.defaultDataSet(dto.DefaultDataSet);
    }

    public static empty(): sparqlQueryInfo {
        return new sparqlQueryInfo({ Query: "", SparqlEndpoint: "", DefaultDataSet: "" });
    }

    public toDto(): sparqlQueryInfoDto {
        return {
            Query: this.query(),
            SparqlEndpoint: this.sparqlEndpoint(),
            DefaultDataSet: this.defaultDataSet()
        };
    }
}

export = sparqlQueryInfo;