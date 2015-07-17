 class indexableProperties {
     properties : KnockoutObservableArray<string>;

     constructor(dto: indexablePropertiesDto) {
         this.properties = ko.observableArray<string>(dto.Properties);
     }
}

 export = indexableProperties;