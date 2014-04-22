class queries {
    queries = ko.observableArray();
    activate(): void {
        var self = this;

        $.getJSON("http://localhost:2429/api/queries", (data, textStatus, jqXHR) => {
            self.queries(data);
        });
    }
}

export = queries;