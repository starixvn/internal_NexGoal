(function (pageModule) {
    "use strict";

    pageModule.controller("PageController", pageController);

    pageController.$inject = ["$rootScope"];

    function pageController($rootScope) {
        var vm = this;

        vm.menuVisibility = localStorage.MenuVisibility === "true" || localStorage.MenuVisibility === undefined;
        $rootScope.menuVisibility = vm.menuVisibility;

        vm.modifyMenuVisibility = function () {
            localStorage.setItem("MenuVisibility", !vm.menuVisibility);
            $rootScope.menuVisibility = !vm.menuVisibility;
            vm.menuVisibility = !vm.menuVisibility;
        };

        vm.getSidebarCSS = function () {
            if (vm.menuVisibility) {
                return "sidebar";
            } else {
                return "sidebar-xs";
            }
        };

        vm.getMainCSS = function () {
            if (vm.menuVisibility) {
                return "main";
            } else {
                return "main-xs";
            }
        };

        vm.getTopNavigationCSS = function () {
            if (vm.menuVisibility) {
                return "left-bar";
            } else {
                return "left-bar-xs";
            }
        };
    }
})(angular.module("pageModule"));