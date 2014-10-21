import constants = require("common/constants");

class viewModelBase {
    public c = new constants();

    public notifyWarning(message : string) : void {
        ko.postbox.publish(this.c.topics.warnings, message);
    }
}

export = viewModelBase;