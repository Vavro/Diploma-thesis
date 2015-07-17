class checkableProperty {
    isChecked = ko.observable<boolean>();
    propertyPath = ko.observable<string>();

    constructor(isChecked: boolean, propertyPath: string) {
        this.isChecked(isChecked);
        this.propertyPath(propertyPath);
    }
}

class propertiesToIndex {
    properties: KnockoutObservableArray<checkableProperty>;

    constructor(dto: propertiesToIndexDto) {
        this.properties = ko.observableArray<checkableProperty>(
            dto.PropertyPaths.map(
                (path: string): checkableProperty => new checkableProperty(false, path)));
    }

    toDto(): propertiesToIndexDto {
        return {
            PropertyPaths: this.properties().filter((prop: checkableProperty):
                boolean => prop.isChecked()).
                map((prop: checkableProperty): string => prop.propertyPath())
        };
    }

} 