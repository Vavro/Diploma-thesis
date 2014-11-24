import viewModelBase = require("viewmodels/viewModelBase");
import getDocumentCommand = require("commands/getDocumentCommand");

class viewDocument extends viewModelBase {
    public document = ko.observable<any>();

    public canActivate(args: any) : JQueryDeferred<{}> {
        var can = $.Deferred();

        if (args && args.documentId && args.definitionId) {
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