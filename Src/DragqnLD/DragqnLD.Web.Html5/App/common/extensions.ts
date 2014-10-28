class extensions {
    static install() : void {
        extensions.installArrayExtensions();

        // Want Intellisense for your extensions?
        // Go to extensionInterfaces.ts and add the function signature there.
    }

    private static installArrayExtensions() : void {
        // Array.remove
        var arrayPrototype: any = Array.prototype;
        arrayPrototype.remove = function (item : any) : number {
            var self: any[] = this;
            var index = self.indexOf(item);
            if (index >= 0) {
                self.splice(index, 1);
            }
            return index;
        };

        // Array.removeAll
        arrayPrototype.removeAll = function (items: Array<any>) : void {
            var self: Array<any> = this;
            for (var i = self.length - 1; i >= 0 && items.length > 0; i--) {
                var itemsIndex = items.indexOf(self[i]);
                if (itemsIndex >= 0) {
                    self.splice(i, 1);
                    items.splice(itemsIndex);
                }
            }
        };

        // Array.first
        arrayPrototype.first = function (filter?: (item : any) => boolean) : any {
            var self: any[] = this;
            if (self.length > 0) {
                if (filter) {
                    return ko.utils.arrayFirst(self, filter);
                } else if (self.length > 0) {
                    return self[0];
                }
            }

            return null;
        };

        // Array.last
        arrayPrototype.last = function (filter?: (item : any) => boolean) : any {
            var self: any[] = this;
            if (filter) {
                for (var i = self.length - 1; i > 0; i--) {
                    if (filter(self[i])) {
                        return self[i];
                    }
                }
            } else if (self.length > 0) {
                return self[self.length - 1];
            }

            return null;
        };

        // Array.pushAll
        arrayPrototype.pushAll = function (items: Array<any>) : void {
            this.push.apply(this, items);
        };

        // Array.contains
        arrayPrototype.contains = function (item: any) : boolean {
            var self: any[] = this;
            return self.indexOf(item) !== -1;
        };

        // Array.count
        arrayPrototype.count = function (filter?: (item : any) => boolean) : number {
            var self: any[] = this;
            if (filter) {
                var matches = 0;
                for (var i = 0; i < self.length; i++) {
                    if (filter(self[i])) {
                        matches++;
                    }
                }

                return matches;
            }

            return self.length;
        };

        // Array.count
        arrayPrototype.distinct = function () : Array<any> {
            var distinctElements = [];
            for (var i = 0; i < this.length; i++) {
                var element = this[i];
                if (!distinctElements.contains(element)) {
                    distinctElements.push(element);
                }
            }

            return distinctElements;
        };
    }
}

export = extensions;