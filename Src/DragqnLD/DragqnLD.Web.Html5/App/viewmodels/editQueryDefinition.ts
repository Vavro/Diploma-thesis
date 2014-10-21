import router = require("plugins/router");
import viewModelBase = require("viewmodels/viewModelBase");

import queryDefinition = require("models/queryDefinition");
import getQueryDefinitionCommand = require("commands/getQueryDefinitionCommand");
import saveQueryDefinitionCommand = require("commands/saveQueryDefinitionCommand");

class editQueryDefinition extends viewModelBase {
    queryDefinition = ko.observable<queryDefinition>();
    editedQueryId: KnockoutComputed<string>;
    isCreatingNewQueryDefinition = ko.observable(false);
    errors: KnockoutValidationErrors; // needs to init after queryDefinition is set
    isValid: KnockoutComputed<boolean>;

    constructor() {
        super();
        this.editedQueryId = ko.computed((): string => this.queryDefinition() ? this.queryDefinition().id() : "");
    }

    canActivate(args: any): JQueryDeferred<{}> {
        if (args && args.id) {
            var canActivateResult = $.Deferred();
            new getQueryDefinitionCommand(args.id)
                .execute()
                .done((queryDefinition: queryDefinition) : void => {
                    this.queryDefinition(queryDefinition);
                    canActivateResult.resolve({ can: true });
                })
                .fail((response : any) : void => {
                    // todo examine possible response and show it
                    this.notifyWarning("Could not get document " + args.id);
                });

            return canActivateResult;
        } else {
            return $.Deferred().resolve({ can: true });
        }
    }


    activate(navigationArgs: any): void {
        if (navigationArgs && navigationArgs.id) {
            // load done in canActivate
        } else {
            this.editNewQueryDefinition();
        }
    }

    editNewQueryDefinition(): void {
        this.isCreatingNewQueryDefinition(true);
        this.queryDefinition(queryDefinition.empty());
        this.errors = ko.validation.group(this.queryDefinition, { deep: true });
        this.isValid = ko.computed({ owner: this, read: () : boolean => { return this.errors().length === 0; } });
    }

    navigateToQueries(): void {
        var url = "/queries";
        router.navigate(url);
    }

    saveQueryDefinition(): void {

        var queryDef = this.queryDefinition();

        if (this.errors().length !== 0) {
            this.errors.showAllMessages();
            this.notifyWarning("There are errors in the form, please check all values.");
        }

        var saveCommand = new saveQueryDefinitionCommand(queryDef);
        saveCommand
            .execute()
            .done(() : void => {

            }); // fail reseno v ramci commands

    }
}

export = editQueryDefinition;