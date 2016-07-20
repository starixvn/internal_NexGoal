"use strict";

(function (menuModule) {

    menuModule.controller("MenuController", menuController);

    menuController.$inject = ["menuModel"];

    function menuController(menuModel) {
        var vm = this;

        vm.menus = {};

        menuModel.getMenus().then(function () {
            vm.menus = menuModel.menus;
        });
    }

})(angular.module("menuModule"));