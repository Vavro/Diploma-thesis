import router = require("plugins/router");
import viewModelBase = require("viewmodels/viewModelBase");

import queryDefinitionWithStatus = require("models/queryDefinitionWithStatus");
import getQueryDefinitionCommand = require("commands/getQueryDefinitionCommand");

class viewQueryDefinition extends viewModelBase {

    queryDefinition = ko.observable<queryDefinitionWithStatus>();
    queryId: KnockoutComputed<string>;
    canRun: KnockoutComputed<boolean>;

    constructor() {
        super();
        this.queryId = ko.computed((): string => this.queryDefinition().id());
        this.canRun = ko.computed((): boolean => this.queryDefinition().status().status() === queryStatus.ReadyToRun);

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
                .done((queryDefinition: queryDefinitionWithStatus): void => {
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

    runQuery(): void {
        this.notifySuccess("runQuery()");
    }


}

export = viewQueryDefinition;