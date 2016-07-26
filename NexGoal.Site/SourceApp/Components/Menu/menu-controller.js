"use strict";

(function (menuModule) {

    menuModule.controller("MenuController", menuController);

    menuController.$inject = ["$location", "menuModel"];

    function menuController($location, menuModel) {
        var vm = this;

        vm.menus = {};

        vm.redirect = function (link) {
            $location.path(link);
        };

        menuModel.getMenus().then(function () {
            vm.menus = menuModel.menus;
        });
    }

})(angular.module("menuModule"));