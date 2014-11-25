import viewModelBase = require("viewmodels/viewModelBase");
import getDocumentCommand = require("commands/getDocumentCommand");
import router = require("plugins/router");

class viewDocument extends viewModelBase {
    public documentId = ko.observable<string>();
    public definitionId = ko.observable<String>();
    public document = ko.observable<any>();
    public navigateToQueriesUrl: KnockoutComputed<string>;
    public navigateToDefinitionUrl : KnockoutComputed<string>;

    constructor() {
        super();
        this.navigateToQueriesUrl = ko.computed(() => "#queries");
        this.navigateToDefinitionUrl = ko.computed(() => "#viewQuery?id=" + this.definitionId());
    }

    public canActivate(args: any) : JQueryDeferred<{}> {
        var can = $.Deferred();

        if (args && args.documentId && args.definitionId) {
            this.documentId(args.documentId);
            this.definitionId(args.definitionId);
            var command = new getDocumentCommand(args.definitionId, args.documentId);
            command.execute().done(result => {
                this.document(this.stringify(result));
            });

            can.resolve({ can: true });
        } else {
            can.resolve({ can: false });
        }

        return can;
    }


    private stringify(obj: any) : string {
        var prettifySpacing = 4;
        return JSON.stringify(obj, null, prettifySpacing);
    }
}

export = viewDocument;