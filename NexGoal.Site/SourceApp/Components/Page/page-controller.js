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
    }
})(angular.module("pageModule"));