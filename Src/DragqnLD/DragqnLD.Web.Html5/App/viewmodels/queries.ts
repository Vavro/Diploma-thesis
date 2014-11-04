import router = require("plugins/router");

import getQueriesCommand = require("commands/getQueriesCommand");

class queries {
    idTemplate = $("#idTemplate").html();
    queriesList = ko.observableArray();
    columnList = ko.observableArray([
        { field: "Id", displayName: "Id", cellTemplate: this.idTemplate },
        { field: "Name", displayName: "Name" },
        { field: "Description", displayName: "Description" }]);
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

    newQueryDefinition(): void {
        router.navigate("#editQuery");
    }
}

export = queries;