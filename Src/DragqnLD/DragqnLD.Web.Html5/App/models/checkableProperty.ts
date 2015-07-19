class checkableProperty {
    isChecked = ko.observable<boolean>();
    propertyPath = ko.observable<string>();
    canFulltext = ko.observable<boolean>();

    constructor(isChecked: boolean, propertyPath: string, canFulltext: boolean) {
        this.isChecked(isChecked);
        this.propertyPath(propertyPath);
        this.canFulltext(canFulltext);

        this.canFulltext.subscribe((newValue: boolean) => {
            if (newValue) {
                this.isChecked(true);
            }
        });

        this.isChecked.subscribe((newValue: boolean) => {
            if (!newValue) {
                this.canFulltext(false);
            }
        });
    }
}

export = checkableProperty; 