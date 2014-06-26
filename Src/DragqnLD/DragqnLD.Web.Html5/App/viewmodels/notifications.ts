import constants = require("common/constants");

class notifications {
    private c = new constants();

    constructor() {
        ko.postbox.subscribe<string>(this.c.topics.errors, this.showError);
        ko.postbox.subscribe<string>(this.c.topics.warnings, this.showWarning);
        ko.postbox.subscribe<string>(this.c.topics.success, this.showSuccess);
    }

    showWarning(message : string): void {
        toastr.warning(message);
    }

    showError(message: string): void {
        toastr.error(message);
    }

    showSuccess(message: string): void {
        toastr.success(message);
    }
}

export = notifications;