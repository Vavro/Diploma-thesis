interface Array<T> {
    remove(item: T): number;
    removeAll(items: T[]): void;
    first(filter?: (item: T) => boolean): T;
    last(filter?: (item: T) => boolean): T;
    pushAll(items: T[]): void;
    contains(item: T): boolean;
    count(filter?: (item: T) => boolean): number;
    distinct(): T[];
}
