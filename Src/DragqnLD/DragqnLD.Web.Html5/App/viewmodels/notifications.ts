import constants = require("common/constants");

class notifications {
    private c = new constants();

    constructor() {
        ko.postbox.subscribe<string>(this.c.topics.errors, this.showError);
        ko.postbox.subscribe<string>(this.c.topics.warnings, this.showWarning);

    }

    showWarning(message : string): void {
        toastr.warning(message);
    }

    showError(message: string): void {
        toastr.error(message);
    }
}

export = notifications;