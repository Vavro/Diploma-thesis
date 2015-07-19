class indexedProperty {
    propertyPath = ko.observable<String>("");
    ravenMapName = ko.observable<String>("");
    analyzerName = ko.observable<String>("");   

    constructor() {
        
    }

    setPropertyPath(propertyPath: string) {
        this.propertyPath(propertyPath);
    }
}

export = indexedProperty;