import app = require('durandal/app');

class Shell {
    name = ko.observable();
    sayHello() : void {
        app.showMessage('Hello ' + this.name() + '! Nice to meet you.', 'Greetings');
    }
}

export = Shell;

//define(function (require) {
    
//    return {
//        name: ko.observable(),
//        sayHello: function () {
//            app.showMessage('Hello ' + this.name() + '! Nice to meet you.', 'Greetings');
//        }
//    };
//}); 