class indexedProperty {
    propertyPath = ko.observable<string>("");
    ravenMapName = ko.observable<string>("");
    analyzerName = ko.observable<string>("");   

    constructor() {
        
    }

    setPropertyPath(propertyPath: string) {
        this.propertyPath(propertyPath);
    }
}

export = indexedProperty;