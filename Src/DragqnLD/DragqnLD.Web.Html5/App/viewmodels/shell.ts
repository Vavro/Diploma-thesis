import router = require('plugins/router');

import dynamicHeightBindingHandler = require("common/dynamicHeightBindingHandler");

class shell {
    private router = router;

    constructor() {

        dynamicHeightBindingHandler.install();

    }

    activate() : any {
        router.map([
            { route: '', title: 'Home', moduleId: 'viewmodels/home', nav: true },
            { route: 'queries', title: 'Queries', moduleId: 'viewmodels/queries', nav: true },
            { route: 'editQuery', title: 'Edit Query Definition', moduleId: 'viewmodels/editQueryDefinition', nav: false },
        ]).buildNavigationModel();

        return router.activate();
    }
}

export = shell;

//define(function (require) {
//    var router = require('plugins/router');

//    return {
//        router: router,
//        activate: function () {
//            router.map([
//                { route: '', title: 'Home', moduleId: 'viewmodels/home', nav: true }
//            ]).buildNavigationModel();

//            return router.activate();
//        }
//    };
//});