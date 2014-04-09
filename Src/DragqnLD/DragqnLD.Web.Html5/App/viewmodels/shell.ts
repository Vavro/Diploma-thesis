import router = require('plugins/router');

class shell {
    private router = router;

    activate() : any {
        router.map([
            { route: '', title: 'Home', moduleId: 'viewmodels/home', nav: true },
            { route: 'rainier', title: 'Mount Rainier', moduleId: 'viewmodels/rainier', nav:true}
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