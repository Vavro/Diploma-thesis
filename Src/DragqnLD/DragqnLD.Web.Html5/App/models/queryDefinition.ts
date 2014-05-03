class queryDefinition {
    id: string;

    constructor(dto?: queryDefinitionDto) {
        if (dto) {
            this.id = dto.Id;
        }
    }

    public static empty(): queryDefinition {

        return new queryDefinition(null);
    }
}

export = queryDefinition;