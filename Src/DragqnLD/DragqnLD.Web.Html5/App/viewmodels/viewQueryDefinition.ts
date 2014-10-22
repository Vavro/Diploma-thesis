import router = require("plugins/router");
import viewModelBase = require("viewmodels/viewModelBase");

import queryDefinition = require("models/queryDefinition");
import getQueryDefinitionCommand = require("commands/getQueryDefinitionCommand");

class viewQueryDefinition extends viewModelBase {
    title = ko.observable("Query Definition detail");

    navigateToQueries(): void {
        var url = "/queries";
        router.navigate(url);
    }
}

export = viewQueryDefinition;