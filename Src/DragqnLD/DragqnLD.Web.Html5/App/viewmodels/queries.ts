import getQueriesCommand = require("commands/getQueriesCommand");

class queries {
    queries = ko.observableArray();
    isAttached = ko.observable(false);
    activate(): void {
        new getQueriesCommand().execute().done(results => {
            //console.log(results);
            this.queries(results);
        });
    }

    compositionComplete(): void {
        console.log('queries view attached');
        this.isAttached(true);
        $(window).trigger('resize');
    }
}

export = queries;