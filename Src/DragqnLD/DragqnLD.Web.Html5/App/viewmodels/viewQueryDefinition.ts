import router = require("plugins/router");
import viewModelBase = require("viewmodels/viewModelBase");

import queryDefinitionWithStatus = require("models/queryDefinitionWithStatus");
import getQueryDefinitionWithStatusCommand = require("commands/getQueryDefinitionWithStatusCommand");

class viewQueryDefinition extends viewModelBase {

    queryDefinition: KnockoutObservable<queryDefinitionWithStatus> = ko.observable<queryDefinitionWithStatus>();
    queryId: KnockoutComputed<string>;
    canRun: KnockoutComputed<boolean>;

    constructor() {
        super();
        this.queryId = ko.computed((): string => this.queryDefinition() ? this.queryDefinition().id() : "");
        this.canRun = ko.computed((): boolean =>
            this.queryDefinition() ?
            this.queryDefinition().status().status() === queryStatus.ReadyToRun :
            false);

    }

    navigateToQueries(): void {
        var url = "/queries";
        router.navigate(url);
    }

    canActivate(args: any): JQueryDeferred<{}> {
        if (args && args.id) {
            return this.loadQuery(args.id);
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

    refresh(): void {
        this.loadQuery();
    }

    private loadQuery(id?: string): JQueryDeferred<{}> {
        var idToLoad = id ? id : this.queryId();
        var canActivateResult = $.Deferred();
        if (!idToLoad) {
            this.notifyError("No id to be loaded");
            return canActivateResult.reject("No id to be loaded");
        }
        new getQueryDefinitionWithStatusCommand(idToLoad)
            .execute()
            .done((queryDefinition: queryDefinitionWithStatus): void => {
                this.queryDefinition(queryDefinition);
                canActivateResult.resolve({ can: true });
                this.notifySuccess("Query " + idToLoad + " loaded");
            })
            .fail((response: any): void => {
                // todo examine possible response and show it
                console.log("Could not get document " + idToLoad + "response: ");
                console.log(response);
                this.notifyWarning("Could not get document " + idToLoad );
            });

        return canActivateResult;
    }
}

export = viewQueryDefinition;