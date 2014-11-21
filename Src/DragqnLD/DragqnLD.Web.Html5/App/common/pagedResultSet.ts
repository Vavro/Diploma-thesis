class pagedResultSet<T> {
    constructor(public items: Array<T>, public totalResultCount: number) {
    }
}

export = pagedResultSet;