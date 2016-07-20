(function (menuModule) {
    "use strict";

    menuModule.controller("MenuController", menuController);

    menuController.$inject = ["menuModel"];

    function menuController(menuModel) {
        var vm = this;

        this.menus = menuModel.getMenus();
    }

})(angular.module("menuModule"));