import app = require("durandal/app");
import viewModelBase = require("viewmodels/viewModelBase");

class home extends viewModelBase {
    name = ko.observable();

    queryText = ko.observable("SELECT ?s ?p ?o WHERE {?s ?p ?o}");

sayHello() : void {
        app.showMessage("Hello " + this.name() + "! Nice to meet you.", "Greetings");
    }

    testNotif(): void {
        this.notifyWarning("smaug is here!");
    }
}

export = home;

// define(function (require) {
//    var app = require('durandal/app'),
//        ko = require('knockout');

//    return {
//        name: ko.observable(),
//        sayHello: function () {
//            app.showMessage('Hello ' + this.name() + '! Nice to meet you.', 'Greetings');
//        }
//    };
// });  