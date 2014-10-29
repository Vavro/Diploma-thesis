﻿import constants = require("common/constants");

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
}

export = viewModelBase;