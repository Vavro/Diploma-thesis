import router = require("plugins/router");
import viewModelBase = require("viewmodels/viewModelBase");

import indexDefinition = require("models/indexDefinition");

import saveIndexDefinitionCommand = require("commands/saveIndexDefinitionCommand");

class editIndexDefinition extends viewModelBase {
    definitionId = ko.observable<string>();
    isCreatingNewIndexDefinition = ko.observable(false);
    indexDefinition = ko.observable<indexDefinition>();
    errors: KnockoutValidationErrors; // needs to init after queryDefinition is set
    isValid: KnockoutComputed<boolean>;
    editedIndexDefinitionId: KnockoutComputed<string>;

    constructor() {
        super();
        this.editedIndexDefinitionId = ko.computed((): string => this.indexDefinition() ? this.indexDefinition().name() : "");
    }

    canActivate(args: any): JQueryDeferred<{}> {
        if (!args || !args.definitionId) {
            return $.Deferred().resolve({ can: false });
        }

        this.definitionId(args.definitionId);

        if (args && args.definitionId) {
            //new index definition
        } else {
        }

        return $.Deferred().resolve({ can: true });
    }


    activate(navigationArgs: any): void {
        if (navigationArgs && navigationArgs.definitionId && navigationArgs.indexId) {
            // load done in canActivate
        } else if (navigationArgs && navigationArgs.definitionId) {
            this.editNewIndexDefinition();
        } else {
            //todo: error !!
        }
    }

    editNewIndexDefinition(): void {
        this.isCreatingNewIndexDefinition(true);
        this.indexDefinition(indexDefinition.empty());
        this.errors = ko.validation.group(this.indexDefinition, { deep: true });
        this.isValid = ko.computed({ owner: this, read: (): boolean => { return this.errors().length === 0; } });
    }

    navigateToQueries(): void {
        var url = "/queries";
        router.navigate(url);
    }

    saveIndexDefinition(): void {

        var indexDef = this.indexDefinition();

        if (this.errors().length !== 0) {
            this.errors.showAllMessages();
            this.notifyWarning("There are errors in the form, please check all values.");
        }

        var saveCommand = new saveIndexDefinitionCommand(this.definitionId(), indexDef);
        saveCommand
            .execute()
            .done(assignedId => {
            // todo: change target after edit save
            var url = "#viewQuery?id=" + assignedId;
            router.navigate(url);
        }); // fail reseno v ramci commands

    }
}

export = editIndexDefinition;