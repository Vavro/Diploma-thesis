import router = require("plugins/router");

import queryDefinition = require("models/queryDefinition");
import getQueryDefinitionCommand = require("commands/getQueryDefinitionCommand");
import saveQueryDefinitionCommand = require("commands/saveQueryDefinitionCommand");

class editQueryDefinition {

    queryDefinition = ko.observable<queryDefinition>();
    editedQueryId: KnockoutComputed<string>;
    isCreatingNewQueryDefinition = ko.observable(false);
    errors = ko.validation.group(this.queryDefinition, { deep: true });
    canSaveQueryDefinition = ko.computed(() => this.queryDefinition.isValid());

    constructor() {
        this.editedQueryId = ko.computed(() : string => this.queryDefinition() ? this.queryDefinition().id() : "");
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

    saveQueryDefinition(): void {

        var queryDef = this.queryDefinition();

        var valid = this.errors.length == 0;
        if (!valid) {
            this.errors.showAllMessages();
            return;
        }
        
        var saveCommand = new saveQueryDefinitionCommand(queryDef);
        saveCommand
            .execute()
            .done(() => {

            }); // fail reseno v ramci commandus

    }
}

export = editQueryDefinition;