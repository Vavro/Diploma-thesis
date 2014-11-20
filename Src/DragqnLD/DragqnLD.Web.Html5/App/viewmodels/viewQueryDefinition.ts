﻿import router = require("plugins/router");
import viewModelBase = require("viewmodels/viewModelBase");

import queryDefinitionWithStatus = require("models/queryDefinitionWithStatus");
import documentMetadata = require("models/documentMetadata");
import getQueryDefinitionWithStatusCommand = require("commands/getQueryDefinitionWithStatusCommand");
import getQueryDocumentsCommand = require("commands/getQueryDocumentsCommand");
import processQueryCommand = require("commands/processQueryCommand");

class viewQueryDefinition extends viewModelBase {

    queryDefinition: KnockoutObservable<queryDefinitionWithStatus> = ko.observable<queryDefinitionWithStatus>();
    queryId: KnockoutComputed<string>;
    canRun: KnockoutComputed<boolean>;

    documentsList: KnockoutObservableArray<documentMetadataDto> = ko.observableArray<documentMetadataDto>();
    idTemplate = $("#viewDocumentIdTemplate").html();
    documentsColumnList = ko.observableArray([
        { field: "Id", displayName: "Id", cellTemplate: this.idTemplate }]);

    isShowingDocuments = ko.observable(true);

    constructor() {
        super();
        this.queryId = ko.computed((): string => this.queryDefinition() ? this.queryDefinition().id() : "");
        this.canRun = ko.computed((): boolean =>
            this.queryDefinition() ?
            this.queryDefinition().status().status() === queryStatus.ReadyToRun :
            false);

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
            .done((result : any) : void => this.notifySuccess("runQuery() + " + result));
        //todo: start polling for status
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
                this.notifyWarning("Could not get document " + idToLoad);
                canActivateResult.reject(response);
            });

        new getQueryDocumentsCommand(idToLoad)
            .execute()
            .done(results => {
                this.documentsList(results);
                //todo: find out why this "hack" is necessary to have right datagrid size
                $(window).trigger("resize"); 
            });

        return canActivateResult;
    }

    public activateDocs(): void {
        this.isShowingDocuments(true);
    }

    public activateIndexes(): void {
        this.isShowingDocuments(false);
    }
}

export = viewQueryDefinition;