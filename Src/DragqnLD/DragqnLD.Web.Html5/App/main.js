﻿requirejs.config({
    paths: {
        text: '../Scripts/text',
        durandal: '../Scripts/durandal',
        plugins: '../Scripts/durandal/plugins',
        transitions: '../Scripts/durandal/transitions',
        knockout: '../Scripts/knockout-3.1.0',
        jquery: '../Scripts/jquery-1.9.1'
    }
});

define(function (require) {
    var system = require('durandal/system'),
        app = require('durandal/app'),
        viewLocator = require('durandal/viewLocator');

    system.debug(true);

    app.title = 'DragqnLD Management';

    app.configurePlugins({
        router: true,
        dialog: true
    });

    app.start().then(function () {
        //Replace 'viewmodels' in the moduleId with 'views' to locate the view.
        //Look for partial views in a 'views' folder in the root.
        viewLocator.useConvention();

        //Show the app by setting the root view model for our application with a transition.
        app.setRoot('viewmodels/shell');
    });
});