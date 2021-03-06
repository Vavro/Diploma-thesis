﻿import router = require("plugins/router");

import dynamicHeightBindingHandler = require("common/dynamicHeightBindingHandler");
import aceEditorBindingHandler = require("common/aceEditorBindingHandler");
import autoCompleteBindingHandler = require("common/autoCompleteBindingHandler");

class shell {
    private router = router;

    constructor() {

        dynamicHeightBindingHandler.install();
        aceEditorBindingHandler.install();
        autoCompleteBindingHandler.install();
    }

    activate() : any {
        this.router.map([
            { route: "", title: "Home", moduleId: "viewmodels/home", nav: true },
            { route: "queries", title: "Queries", moduleId: "viewmodels/queries", nav: true },
                { route: "editQuery", title: "Edit Query Definition", moduleId: "viewmodels/editQueryDefinition", nav: false },
                { route: "viewQuery", title: "View Query Definition", moduleId: "viewmodels/viewQueryDefinition", nav: false },
                { route: "editIndex", title: "Edit Index Definition", moduleId: "viewmodels/editIndexDefinition", nav: false },
            { route: "search", title: "Search", moduleId: "viewmodels/search", nav: true},
            { route: "notifications", title: "", moduleId: "viewmodels/notifications", nav: false },
            { route: "viewDocument", title: "View Document", moduleId: "viewmodels/viewDocument", nav: false }
            
        ]).buildNavigationModel();

        return router.activate();
    }
}

export = shell;

// define(function (require) {
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
// });