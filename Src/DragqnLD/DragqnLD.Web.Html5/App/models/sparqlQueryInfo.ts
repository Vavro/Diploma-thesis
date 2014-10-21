class sparqlQueryInfo {
    query = ko.observable<string>().extend({required : true});
    sparqlEndpoint = ko.observable<string>().extend({required : true});
    defaultDataSet = ko.observable<string>().extend({required : true});

    constructor(dto: sparqlQueryInfoDto) {
        this.query(dto.query);
        this.sparqlEndpoint(dto.sparqlEndpoint);
        this.defaultDataSet(dto.defaultDataSet);
    }

    public static empty(): sparqlQueryInfo {
        return new sparqlQueryInfo({ query: "", sparqlEndpoint: "", defaultDataSet: "" });
    }

    public toDto(): sparqlQueryInfoDto {
        return {
            query: this.query(),
            sparqlEndpoint: this.sparqlEndpoint(),
            defaultDataSet: this.defaultDataSet()
        };
    }
}

export = sparqlQueryInfo;