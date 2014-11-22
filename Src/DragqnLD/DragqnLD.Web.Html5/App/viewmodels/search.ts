import viewModelBase = require("viewmodels/viewModelBase");
import getQueriesCommand = require("commands/getQueriesCommand");

class search extends viewModelBase {
    public definitions = ko.observableArray<queryDefinitionMetadataDto>();
    public selectedDefinition = ko.observable<queryDefinitionMetadataDto>();
    public queryText = ko.observable<String>("");
    public searchResults = ko.observableArray<any>();

    constructor() {
        super();
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
                        this.selectedDefinition(item);
                    }
                });
            }
        });
    }

    public search() : void {
        
    }
}

export = search;