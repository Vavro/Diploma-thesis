import router = require("plugins/router");
import viewModelBase = require("viewmodels/viewModelBase");

import indexDefinition = require("models/indexDefinition");
import indexedProperty = require("models/indexedProperty");
import propertiesToIndex = require("models/propertiesToIndex");
import checkableProperty = require("models/checkableProperty");

import saveIndexDefinitionCommand = require("commands/saveIndexDefinitionCommand");
import getQueryIndexablePropertiesCommand = require("commands/getQueryIndexablePropertiesCommand");
import proposeIndexDefinitionFromPropertiesCommand = require("commands/proposeIndexDefinitionFromPropertiesCommand");
import proposeIndexDefinitionFromSparqlCommand = require("commands/proposeIndexDefinitionFromSparqlCommand");
import getIndexCommand = require("commands/getIndexCommand");

class editIndexDefinition extends viewModelBase {
    definitionId = ko.observable<string>();
    originalEditedIndexName = ko.observable<string>();
    isCreatingNewIndexDefinition = ko.observable(false);
    indexDefinition = ko.observable<indexDefinition>();
    errors: KnockoutValidationErrors; // needs to init after queryDefinition is set
    isValid: KnockoutComputed<boolean>;
    editedIndexDefinitionId: KnockoutComputed<string>;
    indexProperties = ko.observableArray<indexedProperty>();

    isProposingFromProperties = ko.observable<Boolean>(false);
    isProposingFromSparql = ko.observable<Boolean>(false);

    sparqlForPropose = ko.observable<string>("");
    proposedPropertiesToIndex = ko.observable<propertiesToIndex>();
    accessibleProperties = ko.observableArray<checkableProperty>();

    constructor() {
        super();
        this.editedIndexDefinitionId = ko.computed((): string => this.indexDefinition() ? this.indexDefinition().name() : "");

        this.proposedPropertiesToIndex.subscribe((newValue: propertiesToIndex): void => {
            this.accessibleProperties.removeAll();
            if (newValue) {
                newValue.properties().forEach((value) => this.accessibleProperties.push(value));
            }
        });

    }

    canActivate(args: any): JQueryDeferred<{}> {
        if (!args || !args.definitionId) {
            return $.Deferred().resolve({ can: false });
        }

        this.definitionId(args.definitionId);
        var actions = $.Deferred();
        var canActivateDeferred = $.Deferred();
        
        if (args && args.indexName) {
            
            this.originalEditedIndexName(args.indexName);
            
            var getIndexCommandInstance = new getIndexCommand(this.definitionId(), this.originalEditedIndexName());
            actions.then(() => getIndexCommandInstance
                .execute()
                .done((result) => {
                this.isCreatingNewIndexDefinition(false);
                this.setIndexDefinition(result);
            }));
        } else {
            //new index definition
            //created in activate
        }


        var getPropsCommand = new getQueryIndexablePropertiesCommand(this.definitionId());
        actions.then(() => getPropsCommand
                .execute()
                .done(result => this.proposedPropertiesToIndex(new propertiesToIndex(result)))
            )
            .done(() => canActivateDeferred.resolve({ can: true }));
        actions.resolve();

        return canActivateDeferred;
    }


    activate(navigationArgs: any): void {
        if (navigationArgs && navigationArgs.definitionId && navigationArgs.indexName) {
            // load done in canActivate
        } else if (navigationArgs && navigationArgs.definitionId) {
            this.editNewIndexDefinition();
        } else {
            //shouldn't happen
        }
    }

    private editNewIndexDefinition(): void {
        this.isCreatingNewIndexDefinition(true);
        this.setIndexDefinition(indexDefinition.empty());
    }

    private setIndexDefinition(indexDefinition: indexDefinition) {
        if (!this.indexDefinition()) {
            this.indexDefinition(indexDefinition);
            this.errors = ko.validation.group(this.indexDefinition, { deep: true });
            this.isValid = ko.computed({ owner: this, read: (): boolean => { return this.errors().length === 0; } });
            return;
        }

        this.indexDefinition().setFrom(indexDefinition);
    }

    public navigateToQueries(): void {
        var url = "/queries";
        router.navigate(url);
    }

    public saveIndexDefinition(): void {

        var indexDef = this.indexDefinition();

        if (this.errors().length !== 0) {
            this.errors.showAllMessages();
            this.notifyWarning("There are errors in the form, please check all values.");
        }

        var saveCommand = new saveIndexDefinitionCommand(this.definitionId(), indexDef);
        saveCommand
            .execute()
            .done(_ => {
                var url = "#viewQuery?id=" + this.definitionId();
                router.navigate(url);
            }); // fail reseno v ramci commands

    }

    public removeField(fieldIndex: number) {
        this.indexDefinition().fields.splice(fieldIndex, 1);
    }

    public addProperty() {
        if (this.errors().length !== 0) {
            this.errors.showAllMessages();
            this.notifyWarning("There are errors in the form, please check all values.");
        }

        this.indexDefinition().fields.push(new indexedProperty());
    }

    public toggleProposeFromProperties() {
        if (this.isProposingFromSparql()) {
            this.toggleProposeFromSparql();
        }
        this.isProposingFromProperties(!this.isProposingFromProperties());
    }

    public toggleProposeFromSparql() {
        if (this.isProposingFromProperties()) {
            this.toggleProposeFromProperties();
        }
        this.isProposingFromSparql(!this.isProposingFromSparql());
    }

    public proposeFromProperties() {
        var command = new proposeIndexDefinitionFromPropertiesCommand(this.definitionId(), this.proposedPropertiesToIndex());
        command
            .execute()
            .done((result) => {
                this.setIndexDefinition(result);
                this.toggleProposeFromProperties();
            });
    }

    public proposeFromSparql() {
        var command = new proposeIndexDefinitionFromSparqlCommand(this.definitionId(), this.sparqlForPropose());
        command
            .execute()
            .done((result => {
                this.setIndexDefinition(result);
                this.toggleProposeFromSparql();
            }));

        this.notifySuccess("propose from sparql");
    }
}

export = editIndexDefinition;