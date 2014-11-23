import viewModelBase = require("viewmodels/viewModelBase");
import getQueriesCommand = require("commands/getQueriesCommand");

class search extends viewModelBase {
    public definitions = ko.observableArray<queryDefinitionMetadataDto>();
    public selectedDefinition = ko.observable<string>();
    public definitionsExceptSelected: KnockoutComputed<queryDefinitionMetadataDto[]>;

    public searchText = ko.observable<String>("");
    public searchResults = ko.observableArray<any>();

    private idTemplate = $("#viewDocumentIdTemplate").html();
    public searchResultsColumnList = ko.observableArray([
        { field: "Id", displayName: "Id", cellTemplate: this.idTemplate }]);

    public isAttached = ko.observable(false);

    constructor() {
        super();

        this.definitionsExceptSelected = ko.computed(() => {
            var allDefinitions = this.definitions();
            var selectedDefinition = this.selectedDefinition();

            if (!!selectedDefinition) {
                return allDefinitions.filter((definitionDto: queryDefinitionMetadataDto) => definitionDto.Id != selectedDefinition);
            }
            return allDefinitions;
        });
    }

    public compositionComplete(): void {
        //todo: move to ancestor?
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

    private init(definitionId? : string) {
        var command = new getQueriesCommand();
        command.execute().done(results => {
            this.definitions(results);
            if (definitionId) {
                results.forEach((item) => {
                    if (item.Id == definitionId) {
                        this.selectedDefinition(item.Id);
                    }
                });
            } else {
                this.selectedDefinition(results[0].Id);
            }
        });
    }

    private i = 0;
    public search() : void {
        this.searchResults.push({ Id: "Test/" + this.i });
        this.i++;
    }

    public setSelectedDefinition(definitionId: string) {
        this.selectedDefinition(definitionId);

        //todo: change link of this web page (add/change parameter)
    }
}

export = search;