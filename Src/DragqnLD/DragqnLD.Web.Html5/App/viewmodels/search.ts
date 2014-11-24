import viewModelBase = require("viewmodels/viewModelBase");
import getQueriesCommand = require("commands/getQueriesCommand");
import searchEscapedCommand = require("commands/searchEscapedCommand");
import documentMetadata = require("models/documentMetadata");

class search extends viewModelBase {
    public definitions = ko.observableArray<queryDefinitionMetadataDto>();
    public selectedDefinition = ko.observable<string>();
    public definitionsExceptSelected: KnockoutComputed<queryDefinitionMetadataDto[]>;

    public searchText = ko.observable<string>("");
    public searchResults = ko.observableArray<documentMetadata>();

    private idTemplate = $("#viewDocumentIdTemplate").html();
    
    public searchResultsColumnList = ko.observableArray([
        { field: "id", displayName: "Id", cellTemplate: this.idTemplate }]);


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
    }

    public compositionComplete(): void {
        // todo: move to ancestor?
        console.log("search view attached");
        this.isAttached(true);
        $(window).trigger("resize");
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
        var command = new getQueriesCommand();
        command.execute().done(results => {
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
        var command = new searchEscapedCommand(definitionId, this.searchText());
        command.execute().done(result => {
            this.searchResults.removeAll();
            result.Items.forEach(item => {
                this.searchResults.push(new documentMetadata(definitionId, item));
            });
        });
    }

    public setSelectedDefinition(definitionId: string): void {
        this.selectedDefinition(definitionId);

        // todo: change link of this web page (add/change parameter)
    }
}

export = search;