/// <reference path="../models/dto.ts" />

import router = require("plugins/router");
import viewModelBase = require("viewmodels/viewModelBase");

import queryDefinitionWithStatus = require("models/queryDefinitionWithStatus");
import getQueryDefinitionWithStatusCommand = require("commands/getQueryDefinitionWithStatusCommand");
import getQueryDocumentsCommand = require("commands/getQueryDocumentsCommand");
import getQueryContextCommand = require("commands/getQueryContextCommand");
import processQueryCommand = require("commands/processQueryCommand");

import documentMetadata = require("models/documentMetadata");
import indexDefinitionMetadata = require("models/indexDefinitionMetadata");
import indexDefinitions = require("models/indexDefinitions");

import getIndexesCommand = require("commands/getIndexesCommand");
import getQueryIndexablePropertiesCommand = require("commands/getQueryIndexablePropertiesCommand");
import indexableProperties = require("models/indexableProperties");


enum Tabs {
    Documents,
    Context,
    Indexes,
    IndexableProperties
}

class viewQueryDefinition extends viewModelBase {


    queryDefinition: KnockoutObservable<queryDefinitionWithStatus> = ko.observable<queryDefinitionWithStatus>();
    queryId: KnockoutComputed<string>;
    canRun: KnockoutComputed<boolean>;

    isShowingDocuments: KnockoutComputed<Boolean>;
    isShowingContext : KnockoutComputed<Boolean>;
    isShowingIndexes: KnockoutComputed<Boolean>;
    isShowingIndexableProperties: KnockoutComputed<Boolean>;

    activeTab = ko.observable(Tabs.Documents);

    context = ko.observable<any>();

    documentsList: KnockoutObservableArray<documentMetadata> = ko.observableArray<documentMetadata>();
    idTemplate = $("#viewDocumentIdTemplate").html();

    documentsColumnList = ko.observableArray([
        { field: "id", displayName: "Id", cellTemplate: this.idTemplate }]);

    documentsListPagingOptions = {
        pageSizes: ko.observableArray([10, 20, 50]),
        pageSize: ko.observable(20),
        totalServerItems: ko.observable(0),
        currentPage: ko.observable(1)
    };

    indexDefinitions = ko.observable<indexDefinitions>();
    indexList = ko.observableArray<indexDefinitionMetadata>();
    indexColumnList = ko.observableArray([
        { field: "id", displayName: "Id", cellTemplate: this.idTemplate }]);

    indexListPagingOptions = {
        pageSizes: ko.observableArray([10, 20, 50]),
        pageSize: ko.observable(20),
        totalServerItems: ko.observable(0),
        currentPage: ko.observable(1)
    };

    indexableProperties = ko.observable<indexableProperties>();

    constructor() {
        super();
        this.queryId = ko.computed((): string => this.queryDefinition() ? this.queryDefinition().id() : "");
        this.canRun = ko.computed((): boolean =>
            this.queryDefinition() ?
            this.queryDefinition().status().status() == QueryStatus.ReadyToRun :
                false);

        this.isShowingContext = ko.computed((): boolean => this.activeTab() === Tabs.Context);
        this.isShowingDocuments = ko.computed((): boolean => this.activeTab() === Tabs.Documents);
        this.isShowingIndexes = ko.computed((): boolean => this.activeTab() === Tabs.Indexes);
        this.isShowingIndexableProperties = ko.computed((): boolean => this.activeTab() === Tabs.IndexableProperties);

        this.documentsListPagingOptions.pageSize.subscribe(newValue => {
            this.getDocumentsAsync(this.documentsListPagingOptions.currentPage(),
                this.documentsListPagingOptions.pageSize(), this.queryId());
        });
        this.documentsListPagingOptions.currentPage.subscribe(newValue => {
            this.getDocumentsAsync(this.documentsListPagingOptions.currentPage(),
                this.documentsListPagingOptions.pageSize(), this.queryId());
        });
    }

    compositionComplete(): void {
        console.log("viewQueryDefinition view attached");

        $(window).trigger("resize");
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
        // todo: remove - should be run upon save
        new processQueryCommand(this.queryId())
            .execute()
            .done((result: any): void => this.notifySuccess("runQuery() + " + result));
        // todo: start polling for status
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

        this.getDocumentsAsync(this.documentsListPagingOptions.currentPage(),
                this.documentsListPagingOptions.pageSize(), idToLoad)
            .always(() => this.getIndexesAsync(idToLoad))
            .always(() => this.getIndexablePropertiesAsync(idToLoad))
            .always(() => this.getContextAsync(idToLoad))
            .always((): void => {
            new getQueryDefinitionWithStatusCommand(idToLoad)
                .execute()
                .done((queryDefinition: queryDefinitionWithStatus): void => {
                this.queryDefinition(queryDefinition);
                canActivateResult.resolve({ can: true });
                this.notifySuccess("Query " + idToLoad + " loaded");
            })
                .fail((response: any): void => {
                // todo examine possible response and show it
                console.log("Could not get document " + idToLoad + " response: ");
                console.log(response);
                this.notifyWarning("Could not get document " + idToLoad);
                canActivateResult.reject(response);
            });

        });

        return canActivateResult;
    }

    private getDocumentsAsync(currentPage: number, pageSize: number, id: string): JQueryPromise<pagedDocumentMetadataDto> {
        var start = (currentPage - 1) * pageSize;
        return new getQueryDocumentsCommand(id, start, pageSize)
            .execute()
            .done(result => {
                this.documentsList.removeAll();
                result.Items.forEach(item => {
                    this.documentsList.push(new documentMetadata(id, item));
                });

                this.documentsListPagingOptions.totalServerItems(result.TotalItems);

                $(window).trigger("resize");
            });
    }

    private getIndexesAsync(id: string): JQueryPromise<indexDefinitions> {
        var command = new getIndexesCommand(id);
        return command
            .execute()
            .done(result => {
            this.indexDefinitions(result);

                $(window).trigger("resize");
            });
    }

    private getContextAsync(id: string): void {
        var command = new getQueryContextCommand(id);
        command
            .execute()
            .done(result => {
                this.context(this.stringify(result));
        });
    }
    private getIndexablePropertiesAsync(id: string): void {
        var command = new getQueryIndexablePropertiesCommand(id);
        command
            .execute()
            .done(result => {
                this.indexableProperties(result);
            });
    }

    public activateDocs(): void {
        this.activeTab(Tabs.Documents);
        $(window).trigger("resize");
    }

    public activateIndexes(): void {
        this.activeTab(Tabs.Indexes);
        $(window).trigger("resize");
    }

    public activateContext(): void {
        this.activeTab(Tabs.Context);
    }

    public activateIndexableProperties(): void {
        this.activeTab(Tabs.IndexableProperties);
    }

    
}

export = viewQueryDefinition;