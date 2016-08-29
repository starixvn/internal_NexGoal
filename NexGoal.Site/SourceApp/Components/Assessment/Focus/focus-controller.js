(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("FocusController", focusController);

    //focusController.$inject = [];

    function focusController() {
        var vm = this;

        vm.dataTab = {
            paFocus: true,
            chart: false
        };

        vm.tabPAFocusSelected = function () {
            vm.dataTab.paFocus = true;
            vm.dataTab.chart = false;
        };

        vm.tabChartSelected = function () {
            vm.dataTab.paFocus = false;
            vm.dataTab.chart = true;
        };

        return vm;
    }
})(angular.module("assessmentModule"));