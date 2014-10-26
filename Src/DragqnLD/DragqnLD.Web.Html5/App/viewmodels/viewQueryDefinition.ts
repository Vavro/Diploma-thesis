import router = require("plugins/router");
import viewModelBase = require("viewmodels/viewModelBase");

import queryDefinition = require("models/queryDefinition");
import getQueryDefinitionCommand = require("commands/getQueryDefinitionCommand");

class viewQueryDefinition extends viewModelBase {
    title = ko.observable("Query Definition detail");

    queryDefinition = ko.observable<queryDefinition>();
    queryId: KnockoutComputed<string>;

    constructor() {
        super();
        this.queryId = ko.computed((): string => this.queryDefinition() ? this.queryDefinition().id() : "");
    }

    navigateToQueries(): void {
        var url = "/queries";
        router.navigate(url);
    }

    canActivate(args: any): JQueryDeferred<{}> {
        if (args && args.id) {
            var canActivateResult = $.Deferred();
            new getQueryDefinitionCommand(args.id)
                .execute()
                .done((queryDefinition: queryDefinition): void => {
                    this.queryDefinition(queryDefinition);
                    canActivateResult.resolve({ can: true });
                })
                .fail((response: any): void => {
                    // todo examine possible response and show it
                    this.notifyWarning("Could not get document " + args.id);
                });

            return canActivateResult;
        } else {
            return $.Deferred().resolve({ can: false });
        }
    }


    activate(navigationArgs: any): void {
        if (navigationArgs && navigationArgs.id) {
            // load done in canActivate
        } else {
            // should not happen as 
            this.notifyWarning("viewQueryDefinition was activated without id, but shouldn't");
        }
    }
}

export = viewQueryDefinition;