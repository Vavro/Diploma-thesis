import router = require("plugins/router");
import viewModelBase = require("viewmodels/viewModelBase");

import queryDefinitionWithStatus = require("models/queryDefinitionWithStatus");
import getQueryDefinitionWithStatusCommand = require("commands/getQueryDefinitionWithStatusCommand");
import getQueryDocumentsCommand = require("commands/getQueryDocumentsCommand");
import processQueryCommand = require("commands/processQueryCommand");

import documentMetadata = require("models/documentMetadata");

class viewQueryDefinition extends viewModelBase {

    queryDefinition: KnockoutObservable<queryDefinitionWithStatus> = ko.observable<queryDefinitionWithStatus>();
    queryId: KnockoutComputed<string>;
    canRun: KnockoutComputed<boolean>;

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

    isShowingDocuments = ko.observable(true);

    constructor() {
        super();
        this.queryId = ko.computed((): string => this.queryDefinition() ? this.queryDefinition().id() : "");
        this.canRun = ko.computed((): boolean =>
            this.queryDefinition() ?
            this.queryDefinition().status().status() === queryStatus.ReadyToRun :
            false);

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
            .always(() : void => {
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
                // todo: find out why this "hack" is necessary to have right datagrid size
                $(window).trigger("resize");
            });
    }

    public activateDocs(): void {
        this.isShowingDocuments(true);
    }

    public activateIndexes(): void {
        this.isShowingDocuments(false);
    }
}

export = viewQueryDefinition;