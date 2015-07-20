import viewModelBase = require("viewmodels/viewModelBase");
import getQueriesCommand = require("commands/getQueriesCommand");
import searchEscapedCommand = require("commands/searchEscapedCommand");
import documentMetadata = require("models/documentMetadata");

import indexDefinitions = require("models/indexDefinitions");
import indexDefinitionMetadata = require("models/indexDefinitionMetadata");
import getIndexesCommand = require("commands/getIndexesCommand");

import proposeLuceneQueryFromSparqlCommand = require("commands/proposeLuceneQueryFromSparqlCommand");

class search extends viewModelBase {
    public definitions = ko.observableArray<queryDefinitionMetadataDto>();
    public selectedDefinition = ko.observable<string>();
    public definitionsExceptSelected: KnockoutComputed<queryDefinitionMetadataDto[]>;

    public indexes = ko.observableArray<indexDefinitionMetadata>();
    public selectedIndex = ko.observable<string>();
    public indexesExceptSelected: KnockoutComputed<indexDefinitionMetadata[]>;

    public searchText = ko.observable<string>("");
    public searchResults = ko.observableArray<documentMetadata>();

    private idTemplate = $("#viewDocumentIdTemplate").html();

    public searchResultsColumnList = ko.observableArray([
        { field: "id", displayName: "Id", cellTemplate: this.idTemplate }]);

    isProposingFromProperties = ko.observable<Boolean>(false);
    isProposingFromSparql = ko.observable<Boolean>(false);

    sparqlForPropose = ko.observable<string>("");

    public isAttached = ko.observable(false);

    constructor() {
        super();

        this.definitionsExceptSelected = ko.computed((): queryDefinitionMetadataDto[]=> {
            var allDefinitions = this.definitions();
            var selectedDefinition = this.selectedDefinition();

            if (!!selectedDefinition) {
                return allDefinitions.filter(
                    (definitionDto: queryDefinitionMetadataDto): boolean => definitionDto.Id !== selectedDefinition
                    );
            }
            return allDefinitions;
        });

        this.indexesExceptSelected = ko.computed((): indexDefinitionMetadata[] => {
            var allIndexes = this.indexes();
            var selectedIndex = this.selectedIndex();

            if (!!selectedIndex) {
                return allIndexes.filter((indexDefinition: indexDefinitionMetadata): boolean => indexDefinition.name() !== selectedIndex);
            }
        });

        this.selectedDefinition.subscribe((newValue) => {
            //read indexes
            var command = new getIndexesCommand(newValue);
            command.execute().done((result) => {
                this.indexes.removeAll();
                result.indexes().forEach(id => this.indexes.push(id));
                this.indexes.push(indexDefinitionMetadata.dynamic());

                var first = this.indexes()[0];
                this.selectedIndex(first.name());
                
            });
        });
    }

    public compositionComplete(): void {
        console.log("search view attached");
        this.isAttached(true);
        this.triggerResize();
    }

    public canActivate(args: any): JQueryDeferred<{}> {
        if (args && args.id) {
            this.init(args.id);
        } else {
            this.init();
        }
        return $.Deferred().resolve({ can: true });
    }

    private init(definitionId?: string): void {
        var queriesCommand = new getQueriesCommand();
        queriesCommand.execute().done(results => {
            this.definitions(results);
            if (definitionId) {
                results.forEach((item: queryDefinitionMetadataDto): void => {
                    if (item.Id === definitionId) {
                        this.selectedDefinition(item.Id);
                    }
                });
            } else {
                this.selectedDefinition(results[0].Id);
            }
        });
    }

    public search(): void {

        // todo: add paging
        var definitionId = this.selectedDefinition();
        var indexName = this.selectedIndex();
        var command = new searchEscapedCommand(definitionId, this.searchText(), indexName);
        command.execute().done(result => {
            this.searchResults.removeAll();
            result.Items.forEach(item => {
                this.searchResults.push(new documentMetadata(definitionId, item));
            });
        });
    }

    public setSelectedDefinition(definitionId: string): void {
        this.selectedDefinition(definitionId);
    }

    public setSelectedIndex(indexName: string): void {
        this.selectedIndex(indexName);
    }

    public toggleProposeFromSparql() {
        this.isProposingFromSparql(!this.isProposingFromSparql());
    }

    public proposeFromSparql() {
        var command = new proposeLuceneQueryFromSparqlCommand(this.selectedDefinition(), this.selectedIndex(), this.sparqlForPropose());
        command
            .execute()
            .done((result => {
            this.searchText(result);
            this.toggleProposeFromSparql();
        }));

        this.notifySuccess("propose from sparql");
    }
}

export = search;