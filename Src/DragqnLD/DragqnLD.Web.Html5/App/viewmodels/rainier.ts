import http = require('plugins/http');

var url = 'http://api.flickr.com/services/feeds/photos_public.gne';
var qs = { tags: 'mount ranier', tagmode: 'any', format: 'json' };

class rainier {
    

    images = ko.observableArray([]);
    activate() {
        var that = this;
        if (this.images().length > 0) {
            return;
        }

        return http.jsonp(url, qs, 'jsoncallback').then((response) => {
            that.images(response.items);
        });
    }



 }

 export = rainier;