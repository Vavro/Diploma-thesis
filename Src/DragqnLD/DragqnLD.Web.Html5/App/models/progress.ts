class progress {
    totalCount = ko.observable<number>();
    currentItem = ko.observable<number>();

    constructor(dto: progressDto) {
        this.totalCount(dto.TotalCount);
        this.currentItem(dto.CurrentItem);
    }
}

export = progress;