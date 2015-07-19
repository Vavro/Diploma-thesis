/// <reference path="../models/dto.ts" />

import constants = require("common/constants");

class viewModelBase {
    public c = new constants();

    public notifyWarning(message : string) : void {
        ko.postbox.publish(this.c.topics.warnings, message);
    }

    public notifyError(message: string): void {
        ko.postbox.publish(this.c.topics.errors, message);
    }

    public notifySuccess(message: string): void {
        ko.postbox.publish(this.c.topics.success, message);
    }

    public stringify(obj: any): string {
        var prettifySpacing = 4;
        return JSON.stringify(obj, null, prettifySpacing);
    }

    public triggerResize() : void {
        window["dragqnLDWindowHeight"](1);
        $(window).trigger('resize');
    }
}

export = viewModelBase;