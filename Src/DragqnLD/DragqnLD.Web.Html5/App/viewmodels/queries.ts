class queries {
    queries = ko.observableArray();
    isAttached = ko.observable(false);
    activate(): void {
        var self = this;

        $.getJSON("http://localhost:2429/api/queries", (data, textStatus, jqXHR) => {
            self.queries(data);
        });
    }

    compositionComplete(): void {
        console.log('queries view attached');
        this.isAttached(true);
        $(window).trigger('resize');
    }
}

export = queries;