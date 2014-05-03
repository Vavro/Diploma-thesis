import router = require("plugins/router");

import queryDefinition = require("models/queryDefinition");
import getQueryDefinitionCommand = require("commands/getQueryDefinitionCommand");

class editQueryDefinition {

    queryDefinition = ko.observable<queryDefinition>();
    editedQueryId: KnockoutComputed<string>;
    isCreatingNewQueryDefinition = ko.observable(false);

    constructor() {
        this.editedQueryId = ko.computed(() : string => this.queryDefinition() ? this.queryDefinition().id : "");
    }

    canActivate(args: any) : JQueryDeferred<{}> {
        if (args && args.id) {
            var canActivateResult = $.Deferred();
            new getQueryDefinitionCommand(args.id)
                .execute()
                .done((queryDefinition : queryDefinition) => {
                    this.queryDefinition(queryDefinition);
                    canActivateResult.resolve({ can: true });
                })
                .fail(() => {
                    // todo: fail notification
                    
                });

            return canActivateResult;
        } else {
            return $.Deferred().resolve({ can: true });
        }
    }


    activate(navigationArgs: any) : void {
        if (navigationArgs && navigationArgs.id) {
            // todo something? 
        } else {
            this.editNewQueryDefinition();
        }
    }

    editNewQueryDefinition() : void {
        this.isCreatingNewQueryDefinition(true);
        this.queryDefinition(queryDefinition.empty());
    }

    navigateToQueries() : void {
        var url = "/queries";
        router.navigate(url);
    }
}

export = editQueryDefinition;