import constants = require("common/constants");

/// Commands encapsulate a read or write operation to the server and common AJAX related functionality.
class commandBase {

    private c = new constants();
    private baseUrl = "http://localhost:2429/api";

    execute<T>(): JQueryPromise<T> {
        throw new Error("Execute must be overidden.");
    }

    query<T>(relativeUrl: string, args: any, resultsSelector?: (results: any) => T): JQueryPromise<T> {
        var ajax = this.ajax(relativeUrl, args, "GET");
        if (resultsSelector) {
            var task = $.Deferred();
            /* tslint:disable:typedef */
            ajax.done((results, status, xhr) => {
                var transformedResults = resultsSelector(results);
                task.resolve(transformedResults);
            });
            ajax.fail((request, status, error) => {
                task.reject(request, status, error);
            /* tslint:enable:typedef */
            });
            return task;
        } else {
            return ajax;
        }
    }

    put(relativeUrl: string, args: any, options?: JQueryAjaxSettings): JQueryPromise<any> {
        return this.ajax(relativeUrl, args, "PUT", options);
    }

    reset(relativeUrl: string, args: any, options?: JQueryAjaxSettings): JQueryPromise<any> {
        return this.ajax(relativeUrl, args, "RESET", options);
    }

    /*
     * Performs a DELETE rest call.
    */
    del(relativeUrl: string, args: any, options?: JQueryAjaxSettings): JQueryPromise<any> {
        return this.ajax(relativeUrl, args, "DELETE", options);
    }

    post(relativeUrl: string, args: any, options?: JQueryAjaxSettings): JQueryPromise<any> {
        return this.ajax(relativeUrl, args, "POST", options);
    }

    private ajax(relativeUrl: string, args: any, method: string, options?: JQueryAjaxSettings): JQueryPromise<any> {
        // ContentType:
        //
        // Can't use application/json in cross-domain requests, otherwise it 
        // issues OPTIONS preflight request first, which doesn't return proper 
        // headers(e.g.Etag header, etc.)
        // 
        // So, for GETs, we issue text/plain requests, which skip the OPTIONS
        // request and goes straight for the GET request.
        var contentType = method === "GET" ?
            "text/plain; charset=utf-8" :
            "application/json; charset=utf-8";
        var defaultOptions = {
            cache: false,
            url: this.baseUrl + relativeUrl,
            data: args,
            dataType: "json",
            contentType: contentType,
            type: method,
            headers: undefined
        };

        if (options) {
            for (var prop in options) {
                if (options.hasOwnProperty(name)) {
                    defaultOptions[prop] = options[prop];
                }
            }
        }

        return $.ajax(defaultOptions);
    }

    /*
     * Notifications
     */
    notifyError(title: string, details?: string, httpStatusText?: string) : void {
        ko.postbox.publish(this.c.topics.errors, "<p>" + title + "</p><p>" + details + "</p>");
        if (console && console.log && typeof console.log === "function") {
            console.log("Error during command execution", title, details, httpStatusText);
        }
    }

    notifySuccess(text: string) : void {
        ko.postbox.publish(this.c.topics.success, text);
    }
}

export = commandBase;