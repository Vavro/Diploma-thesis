import viewModelBase = require("viewmodels/viewModelBase");
import getQueriesCommand = require("commands/getQueriesCommand");

class search extends viewModelBase {
    public definitions = ko.observableArray<queryDefinitionMetadataDto>();
    public selectedDefinition = ko.observable<string>();
    public definitionsExceptSelected: KnockoutComputed<queryDefinitionMetadataDto[]>;
    public queryText = ko.observable<String>("");
    public searchResults = ko.observableArray<any>();

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

    public search() : void {
        
    }

    public setSelectedDefinition(definitionId: string) {
        this.selectedDefinition(definitionId);

        //todo: change link
    }
}

export = search;