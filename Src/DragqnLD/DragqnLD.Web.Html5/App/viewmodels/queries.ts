import router = require("plugins/router");

import getQueriesCommand = require("commands/getQueriesCommand");

class queries {
    queriesList = ko.observableArray();
    isAttached = ko.observable(false);
    activate(): void {
        new getQueriesCommand().execute().done(results => {
            // console.log(results);
            this.queriesList(results);
        });
    }

    compositionComplete(): void {
        console.log("queries view attached");
        this.isAttached(true);
        $(window).trigger("resize");
    }

    newQueryDefinition() : void {
        router.navigate("#editQuery");
    }
}

export = queries;