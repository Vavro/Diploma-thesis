import app = require('durandal/app');

class home {
    name = ko.observable();
    sayHello() : void {
        app.showMessage('Hello ' + this.name() + '! Nice to meet you.', 'Greetings');
    }
}

export = home;

//define(function (require) {
//    var app = require('durandal/app'),
//        ko = require('knockout');

//    return {
//        name: ko.observable(),
//        sayHello: function () {
//            app.showMessage('Hello ' + this.name() + '! Nice to meet you.', 'Greetings');
//        }
//    };
//});  