import checkableProperty = require("models/checkableProperty");
import indexableProperties = require("models/indexableProperties");

class propertiesToIndex {
    properties: KnockoutObservableArray<checkableProperty>;

    //don't think i need this
    //constructor(dto: propertiesToIndexDto) {
    //    this.properties = ko.observableArray<checkableProperty>(
    //        dto.PropertyPaths.map(
    //            (path: propertyToIndexDto): checkableProperty => new checkableProperty(false, path.PropertyPath, path.FulltextSearchable)));
    //}

    constructor(props: indexableProperties) {
        this.properties = ko.observableArray<checkableProperty>(
            props.properties().map(
                (path: string): checkableProperty => new checkableProperty(false, path, false)));

    }

    toDto(): propertiesToIndexDto {
        var selectedProps = this.properties().filter((prop: checkableProperty):
                boolean => prop.isChecked()).
            map((prop: checkableProperty): propertyToIndexDto => { return { PropertyPath: prop.propertyPath(), FulltextSearchable: prop.canFulltext() } });

        return {
            PropertyPaths: selectedProps
        };
    }

} 

export = propertiesToIndex;