(function (menuModule) {
    "use strict";

    menuModule.factory("menuModel", menuModel);

    menuModel.$inject = ["menuFactory", "errorFactory"];

    function menuModel(menuFactory, errorFactory) {
        var model = {};

        model.menus = {};

        model.getMenus = function () {
            //return menuFactory.getMenus()
            //.success(function (menus) {
            //    model.menus = menus
            //})
            //.error(function (response, status) {
            //    errorFactory.errorHandle(status);
            //})

            var menus = menuFactory.getMenus();

            model.menus = menus;

            return menus;
        };

        return model;
    }
})(angular.module("menuModule"));